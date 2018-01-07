using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Quorra.Library;

namespace Quorra.App
{
    // http://www.dispatchertimer.com/wcf/wcf-duplex-communication-tutorial/
    // https://www.youtube.com/watch?v=Hv6-J7otz10
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class ClientCallback : IClient
    {
        public void ShowMessage(Message message)
        {
            if (Application.Current.MainWindow != null)
                ((MainWindow) Application.Current.MainWindow).AppendMessageToChat(message);
        }
    }
}