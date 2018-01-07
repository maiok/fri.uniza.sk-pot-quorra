using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Quorra.Library;
using Quorra.Model;

namespace Quorra.ChatServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatService
    {
        private readonly QuorraContext _dbContext = new QuorraContext();

        // Thread-Safe dictionary, musi byt nastavene ConcurrencyMode.Multiple
        private readonly ConcurrentDictionary<string, ConnectedClient> _connectedClients =
            new ConcurrentDictionary<string, ConnectedClient>();


        public List<Message> GetMessages()
        {
            throw new System.NotImplementedException();
        }

        public void SendMessage(string fromUser, string toUser, string text)
        {
            var message = new Message
            {
                FromUser = fromUser,
                ToUser = toUser,
                Text = text
            };

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
                    // vynimka nastava zvycajne, ked nejaky prihlaseny uzviatel zatvori svoje okno (zrusi klienta)
                    // todo uvolnit userName
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

        public bool IsUserLogged()
        {
            throw new System.NotImplementedException();
        }

        public bool GetUserByName(string userName)
        {
            throw new System.NotImplementedException();
        }

        public List<string> GetUserNames()
        {
            return _connectedClients.Select(client => client.Key).ToList();
        }

        public bool Logout(string username)
        {
            var key = (from client in _connectedClients where client.Key == username select client.Key).FirstOrDefault();
            if (key == null) return false;
            _connectedClients.TryRemove(key, out _);
            return true;
        }
    }
}