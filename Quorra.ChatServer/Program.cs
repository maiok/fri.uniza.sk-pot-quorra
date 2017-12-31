using Quorra.Library;
using Quorra.Model;

namespace Quorra.ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            QuorraContext _dbContext = new QuorraContext();
            QUser user = new QUser();
            user.Name = "Marko";
            user.UserRole = QUserRole.BackendDeveloper;
            _dbContext.InsertUser(user);
        }
    }
}
