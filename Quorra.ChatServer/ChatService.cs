using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Quorra.Library;

namespace Quorra.ChatServer
{
    /// <summary>
    /// Sme v kontexte Single, pretoze chcem mat len jeden server na ktory sa mi budu klienti pripajat.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatService
    {
        // Thread-Safe dictionary, musi byt nastavene ConcurrencyMode.Multiple
        private readonly ConcurrentDictionary<string, ConnectedClient> _connectedClients =
            new ConcurrentDictionary<string, ConnectedClient>();

        // Evidencia vsetkych sprav
        private readonly List<Message> _messages = new List<Message>();

        public void SendMessage(string fromUser, string toUser, string text)
        {
            var message = new Message
            {
                FromUser = fromUser,
                ToUser = toUser,
                Text = text
            };

            _messages.Add(message);

            foreach (var client in _connectedClients)
            {
                // podmienka ze neodoslem sebe, resp. tomu kto odosiela
                if (string.Equals(client.Key, fromUser, StringComparison.CurrentCultureIgnoreCase)) continue;
                // podmienka odoslania sukromnej spravy len adresatovi alebo verejnu spravu vsetkym
                if (toUser != null && toUser != client.Key) continue;
                try
                {
                    client.Value.Connection.ShowMessage(message);
                }
                catch (Exception)
                {
                    // nastalo vtedy ked som neodhlasil klienta po zatvoreni okna
                }
            }
        }

        public bool Login(string userName)
        {
            // Je uz niekto pod takymto uzivatelom prihlaseny?
            foreach (var client in _connectedClients)
            {
                if (string.Equals(client.Key, userName, StringComparison.CurrentCultureIgnoreCase))
                {
                    // Ano
                    return false;
                }
            }

            var establishedUserConnection = OperationContext.Current.GetCallbackChannel<IClient>();
//            ((IContextChannel) establishedUserConnection).OperationTimeout = TimeSpan.FromMinutes(2);

            var newClient = new ConnectedClient
            {
                Connection = establishedUserConnection,
                UserName = userName
            };

            _connectedClients.TryAdd(userName, newClient);

            // Nie, je zalozeny novy uzivatel pre chat
            return true;
        }

        public List<string> GetUserNames()
        {
            return _connectedClients.Select(client => client.Key).ToList();
        }

        public bool Logout(string username)
        {
            var key = (from client in _connectedClients where client.Key == username select client.Key)
                .FirstOrDefault();
            if (key == null) return false;
            _connectedClients.TryRemove(key, out _);
            return true;
        }

        public void GetAllPublicMessages()
        {
            var client = OperationContext.Current.GetCallbackChannel<IClient>();
            foreach (var message in _messages)
            {
                if (message.ToUser != null) continue;
                client.ShowMessage(message);
            }
        }
    }
}