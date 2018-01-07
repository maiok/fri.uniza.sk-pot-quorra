using System.ServiceModel;

namespace Quorra.Library
{
    /// <summary>
    /// Rozhranie reprezentujuce klienta (t.j. kazde nove aplikacie predstavuje noveho klienta, ktory
    /// sa napoji na existujuci server.)
    /// </summary>
    [ServiceContract]
    public interface IClient
    {
        /// <summary>
        /// Metoda, ktoru vola server aby informovalo klienta o novej sprave v chate
        /// Property IsOneWay = true ak som spravne pochopil, ked tuto metodu vola
        /// server, tak nebude ocakavat odpoved, skratka sa spolieha na to, ze sa sprava
        /// nestrati v casopriestore
        ///</summary>
        [OperationContract(IsOneWay = true)]
        void ShowMessage(Message message);
    }
}
