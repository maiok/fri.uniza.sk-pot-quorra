using System.Collections.Generic;
using System.ServiceModel;

namespace Quorra.Library
{
    /// <summary>
    /// Rohranie sluziace ako kostra pre proces servera chatu.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IClient))]
    public interface IChatService
    {
        /// <summary>
        /// Metoda implementujuca akciu pre obsluhu sprav od klienta ostatnym klientom
        /// </summary>
        /// <param name="fromUser"></param>
        /// <param name="toUser"></param>
        /// <param name="text"></param>
        [OperationContract(IsOneWay = true)]
        void SendMessage(string fromUser, string toUser, string text);

        /// <summary>
        /// Metoda pre prihlasenie uzivatela do chatu.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [OperationContract]
        bool Login(string userName);

        // Metoda ktora vracia mena vsetkych prihlasenych.
        [OperationContract]
        List<string> GetUserNames();

        /// <summary>
        /// Metoda odhlasuje uzivatela z chatu.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [OperationContract]
        bool Logout(string username);

        /// <summary>
        /// Metoda mi posle vsetky verejne spravy novo prihlasenemu uzivatelovi.
        /// EDIT: nepouzivam ju nakolko mi z neznamych dovodov takmer zamrzla aplikacia.
        /// </summary>
        [OperationContract]
        void GetAllPublicMessages();
    }
}
