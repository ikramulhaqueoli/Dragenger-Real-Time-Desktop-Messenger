using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using EntityLibrary;
using System.Data.SqlServerCe;
using Newtonsoft.Json.Linq;
//using FileIOAccess;
//using ResourceLibrary;

namespace LocalRepository
{
    public class ConversationRepository : DatabaseAccess, IConversationRepository
    {
        public long? Insert(Conversation item)
        {
            string query = "INSERT INTO Conversations (Id, Type) values (" + item.ConversationID + ",'" + item.Type + "')";
            string success = this.ExecuteSqlCeScalar(query);
            if (success == null) return null;
            return item.ConversationID;
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
            return null;
        }

        public bool? UpdateLastNuntias(Conversation conversation, Nuntias lastNuntias)
        {
            string sql = "UPDATE DuetConversations set Last_Nuntias_id = " + lastNuntias.Id + " where Id = " + conversation.ConversationID;
            int? success = this.ExecuteSqlCeQuery(sql);
            if (success == null) return null;
            if (success > 0) return true;
            return false;
        }

        public List<Nuntias> GetNewerNuntias(Conversation conversation)
        {
            if (conversation == null) return null;
            long lastNuntiasId = 0;
            string query = "select * from Nuntii where Conversation_id = " + conversation.ConversationID + " and Id > " + lastNuntiasId;
            List<Nuntias> nuntiasList = NuntiasRepository.Instance.GetNuntiasListByQuery(query);
            return nuntiasList;
        }

        public List<Nuntias> GetLastNuntias(Conversation conversation)
        {
            string query = "select * from Nuntii where Conversation_id = " + conversation.ConversationID + " order by Id;";
            List<Nuntias> nuntiasList = NuntiasRepository.Instance.GetNuntiasListByQuery(query);
            return nuntiasList;
        }

        public List<Nuntias> GetPendingNuntii(Conversation conversation)
        {
            string query = "select * from Nuntii_to_be_sent where Conversation_id = " + conversation.ConversationID + " order by Temp_Id;";
            List<Nuntias> nuntiasList = NuntiasRepository.Instance.GetPendingNuntiasListByQuery(query);
            return nuntiasList;
        }

        public List<JObject> GetConversationsHeaderJson()
        {
			try
			{
				string sql = "";
				SqlCeDataReader data = null;
				sql = "SELECT Id, Type FROM Conversations where Id in (SELECT DISTINCT Conversation_id FROM Nuntii);";
				data = this.ReadSqlCeData(sql);
				if (data == null) return null;
                List<KeyValuePair<long, string>> conversationIdAndTypeList = new List<KeyValuePair<long, string>>();
                while (data.Read()) conversationIdAndTypeList.Add(new KeyValuePair<long, string>((long)data["Id"], data["Type"].ToString()));
                List<JObject> conversationHeaderJsonList = new List<JObject>();
				foreach(var conversationIdAndType in conversationIdAndTypeList)
				{
                    long conversationId = conversationIdAndType.Key;
                    string type = conversationIdAndType.Value;

                    string conversationName = null, conversationIconFileId = null;
					string lastText = null, lastTextHasContent = null, lastTextTime = null; // lastTextStatus = null;

					JObject conversationHeaderJson = new JObject();
					conversationHeaderJson["id"] = conversationId;
					conversationHeaderJson["type"] = type;

					SqlCeDataReader ldata = null;
					if (type == "duet")
					{
						sql = "select Member_Id_1 as member_id from Duet_Conversations where Conversation_Id = " + conversationId + " and Member_Id_2 = " + Consumer.LoggedIn.Id;
						string lsql = null;
						ldata = this.ReadSqlCeData(sql);
						long? otherMemberId = null;
						if (ldata.Read())
						{
							otherMemberId = (long)ldata["member_id"];
						}
						else
						{
							sql = "select Member_Id_2 as member_id from Duet_Conversations where Conversation_Id = " + conversationId + " and Member_Id_1 = " + Consumer.LoggedIn.Id;
							ldata = this.ReadSqlCeData(sql);
							if (ldata.Read())
							{
								otherMemberId = (long)ldata["member_id"];
							}
						}
						if (otherMemberId == null) return null;
						conversationHeaderJson["other_member_id"] = otherMemberId;
						lsql = "select Name, Profile_img_ID from Consumers where User_ID = " + otherMemberId;
						ldata = this.ReadSqlCeData(lsql);
						if (ldata.Read())
						{
							conversationName = ldata["Name"].ToString();
                            conversationIconFileId = ldata["Profile_img_ID"].ToString();
						}
					}
					else if (type == "group")
					{
						sql = "select Group_name from Group_conversations where Conversation_Id = " + conversationId + " ; ";
						ldata = this.ReadSqlCeData(sql);
						if (ldata.Read())
						{
							conversationName = ldata["Group_name"].ToString();
                            //conversationIconFileId = ldata["Profile_img_ID"].ToString();    //it is not implemented yet
                        }
					}
                    sql = "SELECT Text, Sent_time, Content_Id from Nuntii_to_be_sent WHERE Temp_Id in (SELECT MAX(Temp_Id) as max_id FROM Nuntii_to_be_sent WHERE Conversation_id = " + conversationId + ");";
                    ldata = this.ReadSqlCeData(sql);
					if (ldata.Read())
					{
						lastText = ldata["Text"].ToString();
                        lastTextHasContent = (ldata["Content_Id"].ToString().Length > 0).ToString();
						lastTextTime = (new Time(ldata["Sent_time"].ToString())).Time12;
					}
                    else
                    {
                        sql = "SELECT Text, Sent_time, Content_Id from Nuntii WHERE Id in (SELECT MAX(Id) as max_id FROM Nuntii WHERE Conversation_id = " + conversationId + ");";
                        ldata = this.ReadSqlCeData(sql);
                        if (ldata.Read())
                        {
                            lastText = ldata["Text"].ToString();
                            lastTextHasContent = (ldata["Content_Id"].ToString().Length > 0).ToString();
                            lastTextTime = (new Time(ldata["Sent_time"].ToString())).Time12;
                        }
                    }
					conversationHeaderJson["name"] = conversationName;
                    conversationHeaderJson["icon_file_id"] = conversationIconFileId;
                    conversationHeaderJson["last_text"] = lastText;
                    conversationHeaderJson["last_text_has_content"] = lastTextHasContent;
                    conversationHeaderJson["last_text_sent_time"] = lastTextTime;
					conversationHeaderJsonList.Add(conversationHeaderJson);
				}
				return conversationHeaderJsonList;
			}
            catch(Exception e)
			{
				Console.WriteLine("Error: " + e.StackTrace + " " + e.Message);
				return null;
			}
        }

        public bool? ExistsConversation(long conversationId)
        {
            string sql = "SELECT Id from Conversations where Id = " + conversationId;
            SqlCeDataReader data = this.ReadSqlCeData(sql);
            if (data == null) return null;
            return data.Read();
        }

        new public static DuetConversationRepository Instance
        {
            get { return new DuetConversationRepository(); }
        }

    }
}