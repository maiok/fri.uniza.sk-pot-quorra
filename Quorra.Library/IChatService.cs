using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Quorra.Library
{
    [ServiceContract(CallbackContract = typeof(IClient))]
    public interface IChatService
    {
        [OperationContract]
        List<Message> GetMessages();

        [OperationContract(IsOneWay = true)]
        void SendMessage(string fromUser, string toUser, string text);

        [OperationContract]
        bool Login(string userName);

        [OperationContract]
        bool IsUserLogged();

        [OperationContract]
        bool GetUserByName(string userName);

        [OperationContract]
        List<string> GetUserNames();

        [OperationContract]
        bool Logout(string username);

        [OperationContract]
        void GetAllPublicMessages();
    }
}
