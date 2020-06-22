using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLibrary;

namespace Interfaces
{
    public interface IConversationRepository : IRepository<Conversation>
    {
        List<Nuntias> GetAllNuntias(Conversation conversation);
    }
}
