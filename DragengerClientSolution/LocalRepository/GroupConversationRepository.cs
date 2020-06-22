using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using EntityLibrary;
using System.Data.SqlServerCe;
//using FileIOAccess;
//using ResourceLibrary;

namespace LocalRepository
{
    public class GroupConversationRepository : ConversationRepository
    {
        new public long? Insert(Conversation item)
        {
            long? insertedId = base.Insert(item);
            this.CloseConnection();
            if (insertedId == null) return null;
            GroupConversation groupItem = (GroupConversation)item;
            string sql = "INSERT INTO Group_Conversations (Conversation_Id, Group_name) values (" + insertedId + ",'" + groupItem.ConversationName + "');";
            if (groupItem.MemberList != null)
            {
                foreach (Consumer member in groupItem.MemberList)
                {
                    sql += "GO INSERT INTO Group_Member_Map (Conversation_Id, Member_Id) values (" + insertedId + "," + member.Id + ");";
                }
            }
            string success = this.ExecuteSqlCeScalar(sql);
            if (success != null) return insertedId;
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
            SqlCeDataReader data = instance.ReadSqlCeData(sql);
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

        new public static DuetConversationRepository Instance
        {
            get { return new DuetConversationRepository(); }
        }
    }
}