using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using EntityLibrary;
using System.Data.SqlClient;
//using FileIOAccess;
//using ResourceLibrary;

namespace Repositories
{
    public class DuetConversationRepository : ConversationRepository
    {
        new public long? Insert(Conversation item)
        {
            long? insertedId = base.Insert(item);
            this.CloseConnection();
            if (insertedId == null) return null;
            DuetConversation duetItem = (DuetConversation)item;
            string sql = "INSERT INTO Duet_Conversations (Conversation_Id, Member_Id_1, Member_Id_2) values (" + insertedId + ",'" + duetItem.Member1.Id + "','" + duetItem.Member2.Id + "')";
            int? success = this.ExecuteSqlQuery(sql);
            if (success != null && success > 0) return insertedId;
            return null;
        }

        new public bool? Update(Conversation item)
        {
            throw new NotImplementedException();
        }

        new public bool? Delete(long id)
        {
            throw new NotImplementedException();
        }

        new public Conversation Get(long conversationID)
        {
            string sql = "SELECT * FROM DuetConversations where Id = " + conversationID;
            DatabaseAccess instance = DatabaseAccess.Instance;
            SqlDataReader data = instance.ReadSqlData(sql);
            if (data == null) return null;
            string conversation_ID = null, username_1 = null, username_2 = null, lastNuntiasId = null;
            while (data.Read())
            {
                conversation_ID = data["ID"].ToString();
                username_1 = data["username_1"].ToString();
                username_2 = data["username_2"].ToString();
                lastNuntiasId = data["Last_Nuntias_id"].ToString();
                break;
            }
            instance.Dispose();
            if (conversation_ID == null) return null;
            //Consumer user1 = ConsumerRepository.Instance.GetConsumerByUsername(username_1);
            //Consumer user2 = ConsumerRepository.Instance.GetConsumerByUsername(username_2);
            Nuntias lastNuntias = NuntiasRepository.Instance.Get(long.Parse(lastNuntiasId));
            return null; //new DuetConversation(conversation_ID, user1, user2, lastNuntias);
        }

        public long? GetDuetConversationId(long userId1, long userId2)
        {
            string query = "SELECT C.Id from Conversations C, Duet_Conversations D where C.Id = D.Conversation_Id and (D.Member_Id_1 = " + userId1 + " and D.Member_Id_2 = " + userId2 + ") or (D.Member_Id_1 = " + userId2 + " and D.Member_Id_2 = " + userId1 + ")";
            SqlDataReader data = DatabaseAccess.Instance.ReadSqlData(query);
            if (data == null) return null;
            while (data.Read())
            {
                return long.Parse(data["Id"].ToString());
            }
            return null;
        }

        new public static DuetConversationRepository Instance
        {
            get { return new DuetConversationRepository(); }
        }
    }
}