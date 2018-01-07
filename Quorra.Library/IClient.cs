using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Quorra.Library
{
    public interface IClient
    {
        // Metoda, ktoru vola server aby informovalo klienta o novej sprave v chate
        // Property IsOneWay = true ak som spravne pochopil, ked tuto metodu vola
        // server, tak nebude ocakavat odpoved, skratka sa spolieha na to, ze sa sprava
        // nestrati v casopriestore
        [OperationContract(IsOneWay = true)]
        void ShowMessage(Message message);
    }
}
