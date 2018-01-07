using System.ServiceModel;
using System.Windows;
using Quorra.Library;

namespace Quorra.App
{
    // ZDROJ: http://www.dispatchertimer.com/wcf/wcf-duplex-communication-tutorial/
    // ZDROJ: https://www.youtube.com/watch?v=Hv6-J7otz10
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class ClientCallback : IClient
    {
        public void ShowMessage(Message message)
        {
            // Novu spravu pridam na koniec zoznamu v aktualne otvorenom okne klienta
            if (Application.Current.MainWindow != null)
                ((MainWindow) Application.Current.MainWindow).AppendMessageToChat(message);
        }
    }
}