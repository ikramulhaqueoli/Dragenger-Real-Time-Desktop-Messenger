using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using EntityLibrary;
using System.Data.SqlServerCe;

namespace LocalRepository
{
    public class DuetConversationRepository : ConversationRepository
    {
        new public long? Insert(Conversation item)
        {
            Console.WriteLine(item);
            long? insertedId = base.Insert(item);
            this.CloseConnection();
            if (insertedId == null) return null;
            DuetConversation duetItem = (DuetConversation)item;
            string sql = "INSERT INTO Duet_Conversations (Conversation_Id, Member_Id_1, Member_Id_2) values (" + insertedId + ",'" + duetItem.Member1.Id + "','" + duetItem.Member2.Id + "')";
            int? success = this.ExecuteSqlCeQuery(sql);
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
			string sql = "SELECT member_id_1, member_id_2 FROM Duet_Conversations where Conversation_Id = " + conversationID;
            DatabaseAccess instance = DatabaseAccess.Instance;
            SqlCeDataReader data = instance.ReadSqlCeData(sql);
            if (data == null) return null;
            while (data.Read())
            {
                long memberId1 = long.Parse(data["member_id_1"].ToString());
				long memberId2 = long.Parse(data["member_id_2"].ToString());
				instance.Dispose();
				Consumer user1 = ConsumerRepository.Instance.Get(memberId1);
				Consumer user2 = ConsumerRepository.Instance.Get(memberId2);
				return new DuetConversation(conversationID, user1, user2);
            }
            return null;
        }

        public long? GetDuetConversationId(long userId1, long userId2)
        {
            string query = "SELECT C.Id, C.Last_nuntias_id from Conversations C, Duet_Conversations D where C.Id = D.Conversation_Id and (D.Member_Id_1 = " + userId1 + " and D.Member_Id_2 = " + userId2 + ") or (D.Member_Id_1 = " + userId2 + " and D.Member_Id_2 = " + userId1 + ")";
            SqlCeDataReader data = DatabaseAccess.Instance.ReadSqlCeData(query);
            if (data == null) return null;
            while (data.Read())
            {
                return long.Parse(data["Id"].ToString());
            }
            return null;
        }

        public List<Nuntias> GetNuntiasListByConversationId(long conversationId, long userId)
        {
            string query = "SELECT * FROM Nuntii where Conversation_id = " + conversationId;
            //here user id have come to update seen time
            return NuntiasRepository.Instance.GetNuntiasListByQuery(query);
        }

        new public static DuetConversationRepository Instance
        {
            get { return new DuetConversationRepository(); }
        }
    }
}