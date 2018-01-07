using System;
using System.ServiceModel;
using Quorra.Model;

namespace Quorra.ChatServer
{
    class ProgramServer
    {
        static void Main(string[] args)
        {
            var selfHost = new ServiceHost(typeof(ChatService));
            try
            {
                selfHost.Open();
                Console.WriteLine(@"Služba chatu je spustená. Stlač <ENTER> pre ukončenie služby.");
                Console.ReadLine();
                selfHost.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine(@"Neočakávaná chyba pri spustení služby chatu: {0}", ce.Message);
                selfHost.Abort();
            }
        }
    }
}