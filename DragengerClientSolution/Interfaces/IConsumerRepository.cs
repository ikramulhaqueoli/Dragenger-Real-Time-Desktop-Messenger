using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLibrary;
namespace Interfaces
{
    public interface IConsumerRepository : IRepository<Consumer>
    {
        bool? EmailAlreadyExists(string email);
        long UserIdByDeviceMac(string deviceMac);
    }
}
