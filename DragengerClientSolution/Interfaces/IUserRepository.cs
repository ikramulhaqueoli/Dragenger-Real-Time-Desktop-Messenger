using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLibrary;

namespace Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        bool? UserNameAlreadyExists(string username);
        bool? PasswordIsSet(string deviceID);
        string GetPassword(long userId);
        Time GetLastActive(string deviceID);
    }
}
