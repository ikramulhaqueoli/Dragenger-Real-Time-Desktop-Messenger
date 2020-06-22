using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using EntityLibrary;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Display;
//using FileIOAccess;
//using ResourceLibrary;

namespace Repositories
{
    public class ConversationRepository : DatabaseAccess, IConversationRepository
    {
        public long? Insert(Conversation item)
        {
            string query = "INSERT INTO Conversations (Type) output Inserted.Id values ('" + item.Type + "')";
            string conversationID = this.ExecuteSqlScalar(query);
            return long.Parse(conversationID);
        }

        public bool? Update(Conversation item)
        {
            throw new NotImplementedException();
        }

        public bool? Delete(long id)
        {
            throw new NotImplementedException();
        }

        public Conversation Get(long conversationID)
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

        public bool? UpdateLastNuntias(Conversation conversation, Nuntias lastNuntias)
        {
            string sql = "UPDATE DuetConversations set Last_Nuntias_id = " + lastNuntias.Id + " where Id = " + conversation.ConversationID;
            int? success = this.ExecuteSqlQuery(sql);
            if (success == null) return null;
            if (success > 0) return true;
            return false;
        }

        public List<Nuntias> GetAllNuntias(Conversation conversation)
        {
            string query = "select * from Nuntias where Conversation_id = " + conversation.ConversationID;
            List<Nuntias> nuntiasList = NuntiasRepository.Instance.GetNuntiasListByQuery(query);
            return nuntiasList;
        }

        public List<JObject> GetConversations(List<long> conversationList)
        {
            List<JObject> conversationJsonList = new List<JObject>();
            string idString = "(";
            foreach (long id in conversationList)
            {
                idString += id + ",";
            }
            idString = idString.Substring(0, idString.Length - 1) + ')';
            string sql = "SELECT c.Id, c.Type, g.Group_name, g.Icon_ID from Conversations c, Group_conversations g where g.Conversation_Id = c.Id and g.Conversation_Id in " + idString + ";";
            SqlDataReader data = this.ReadSqlData(sql);
            while(data.Read())
            {
                JObject conversationJson = new JObject();
                conversationJson["id"] = long.Parse(data["Id"].ToString());
                conversationJson["type"] = data["Type"].ToString();
                conversationJson["name"] = data["Group_name"].ToString();
                if (data["Icon_ID"].ToString().Length > 0) conversationJson["icon_id"] = data["Icon_ID"].ToString();
                sql = "SELECT Member_Id FROM Group_Member_Map WHERE Conversation_Id = " + conversationJson["id"] + ";";
                data = this.ReadSqlData(sql);
                int counter = 0;
                while (data.Read())
                {
                    conversationJson["member_id_" + ++counter] = (long)data["Member_Id"];
                }
                conversationJson["member_count"] = counter;
                conversationJsonList.Add(conversationJson);
            }
			this.CloseConnection();
            sql = "SELECT c.Id, c.Type, d.Member_Id_1, d.Member_Id_2 from Conversations c, Duet_Conversations d where d.Conversation_Id = c.Id and d.Conversation_Id in " + idString + ";";
            data = this.ReadSqlData(sql);
			Output.ShowLog(sql);
			if(data == null) return null;
            while (data.Read())
            {
                JObject conversationJson = new JObject();
                conversationJson["id"] = long.Parse(data["Id"].ToString());
                conversationJson["type"] = data["Type"].ToString();
                conversationJson["member_id_1"] = long.Parse(data["Member_Id_1"].ToString());
                conversationJson["member_id_2"] = long.Parse(data["Member_Id_2"].ToString());
                conversationJson["member_count"] = 2;
                conversationJsonList.Add(conversationJson);
            }
            return conversationJsonList;
        }

        new public static ConversationRepository Instance
        {
            get { return new ConversationRepository(); }
        }
    }
}