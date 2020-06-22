using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using System.Data.SqlClient;
using EntityLibrary;
//using FileIOAccess;
using System.Drawing;
using Display;

namespace Repositories
{
    public class NuntiasRepository : DatabaseAccess, INuntiasRepository
    {
        public long? Insert(Nuntias nuntias)
        {
            try
            {
                /*  query pattern
                DECLARE @nuntias_id BIGINT;
                DECLARE @output_table table (nuntias_id BIGINT)
                INSERT INTO Nuntii (text,Sender_id,Sent_time,Conversation_id) OUTPUT Inserted.Id into @output_table values ('oli',8,'2020-03-09 11:51:28',101) 
                select @nuntias_id = nuntias_id from @output_table
                select @nuntias_id inserted_id
	            UPDATE Conversations SET Last_nuntias_id = @nuntias_id where Id = 101
                
                DECLARE @id_list TABLE(id bigint)
                BEGIN

                (select Member_Id_1 as id into id_list from Duet_Conversations where Conversation_Id = 100 union all
                select Member_Id_2 AS id from Duet_Conversations where Conversation_Id = 100 union all
                select Member_Id as id from Group_Member_Map where Conversation_Id = 100)

                DECLARE @MyCursor CURSOR;
                DECLARE @MyField bigint;
                BEGIN
                    SET @MyCursor = CURSOR FOR
                    select id from id_list

                    OPEN @MyCursor 
                    FETCH NEXT FROM @MyCursor 
                    INTO @MyField

                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                      INSERT INTO Nuntius_Owner_map (Nuntias_Id,Owner_Id) values (@nuntias_id,@MyField);
                      FETCH NEXT FROM @MyCursor 
                      INTO @MyField 
                    END; 

                    CLOSE @MyCursor ;
                    DEALLOCATE @MyCursor;
                END;
	                drop table id_list;
                END
                */
                string sql = "DECLARE @nuntias_id BIGINT; DECLARE @output_table table (nuntias_id BIGINT); ";
                string nuntiasText = "NULL", contentFileId = "NULL";
                if (nuntias.Text != null && nuntias.Text.Length > 0) nuntiasText = "'"+nuntias.Text+"'";
                if (nuntias.ContentFileId != null && nuntias.ContentFileId.Length > 0) contentFileId = "'"+nuntias.ContentFileId+"'";
                sql += "INSERT INTO Nuntii (text,Sender_id,Sent_time,Conversation_id,Content_Id) OUTPUT Inserted.Id into @output_table values (" + nuntiasText + "," + nuntias.SenderId + ",'" + nuntias.SentTime.DbFormat + "'," + nuntias.NativeConversationID + "," + contentFileId + "); ";
                sql += "select @nuntias_id = nuntias_id from @output_table; select @nuntias_id inserted_id; ";
                //sql += "UPDATE Conversations SET Last_nuntias_id = @nuntias_id where Id = " + nuntias.NativeConversationID + "; ";
                sql += "DECLARE @id_list TABLE(id bigint); ";
                sql += "BEGIN ";

                sql += "(select Member_Id_1 as id into id_list from Duet_Conversations where Conversation_Id = " + nuntias.NativeConversationID + " union all ";
                sql += "select Member_Id_2 AS id from Duet_Conversations where Conversation_Id = " + nuntias.NativeConversationID + " union all ";
                sql += "select Member_Id as id from Group_Member_Map where Conversation_Id = " + nuntias.NativeConversationID  + "); ";

                sql += "DECLARE @MyCursor CURSOR; ";
                sql += "DECLARE @MyField bigint; ";
                sql += "BEGIN ";
                sql += "    SET @MyCursor = CURSOR FOR ";
                sql += "    select id from id_list ";

                sql += "    OPEN @MyCursor ";
                sql += "    FETCH NEXT FROM @MyCursor ";
                sql += "    INTO @MyField ";

                sql += "    WHILE @@FETCH_STATUS = 0 ";
                sql += "    BEGIN ";
                sql += "      INSERT INTO Nuntius_Owner_map (Nuntias_Id,Owner_Id) values (@nuntias_id,@MyField); ";
                sql += "      FETCH NEXT FROM @MyCursor ";
                sql += "      INTO @MyField ";
                sql += "    END; ";

                sql += "    CLOSE @MyCursor ; ";
                sql += "    DEALLOCATE @MyCursor; ";
                sql += "END; ";
	            sql += "    drop table id_list; ";
                sql += "END ";
                string NuntiasID = this.ExecuteSqlScalar(sql).ToString();
                return long.Parse(NuntiasID);
            }
            catch 
            {
                return null;
            }
        }

        public List<long> NuntiasOwnerIdList(long nuntiasId)
        {
            string sql = "SELECT Id FROM Users WHERE Id IN (SELECT Owner_Id FROM Nuntius_Owner_map WHERE Nuntias_Id = " + nuntiasId + ");";
            SqlDataReader data = this.ReadSqlData(sql);
            List<long> idList = new List<long>();
            if (data == null) return idList;
            while(data.Read())
            {
                idList.Add(long.Parse(data["Id"].ToString()));
            }
            return idList;
        }

        public string InsertContentedNuntias(Nuntias newNuntias, string file_id)
        {
            string sql = "INSERT INTO Nuntias (text,sender_username,sent_time,Content_file_id,conversation_id) OUTPUT Inserted.Id values ('" + newNuntias.Text + "','" + newNuntias.SenderId + "','" + newNuntias.SentTime.DbFormat + "','" + file_id + "','" + newNuntias.NativeConversationID + "')";
            string NuntiasID = this.ExecuteSqlScalar(sql).ToString();
            return NuntiasID;
        }

        public bool? UpdateNuntiasStatusTimes(Nuntias item, long userId)
        {
            string deliveryTimeStr = "NULL", seenTimeStr = "NULL";
            if (item.DeliveryTime != null) deliveryTimeStr = "'" + item.DeliveryTime.DbFormat + "'";
            if (item.SeenTime != null) seenTimeStr = "'" + item.SeenTime.DbFormat + "'";
            string query = "UPDATE Nuntii SET Delivery_time = " + deliveryTimeStr + ", Seen_time = " + seenTimeStr + " where Id = " + item.Id + ";";
            query += "UPDATE Nuntius_Owner_map SET Delivery_Status = 1 where Delivery_Status = 0 and Owner_Id = " + userId + " and Nuntias_Id = " + item.Id + ";";
            string success = this.ExecuteSqlScalar(query);
            if (success == null) return false;
            return true;
        }

        public bool? Delete(long NuntiasID)
        {
            throw new NotImplementedException();
        }

        internal Nuntias GetNuntiasByQyery(string query)
        {
            SqlDataReader reader = this.ReadSqlData(query);
            if (reader == null) return null;
            long? id = null, senderId = null, conversationId = null;
            string text = null, contentPath = null;
            Time sentTime = null, deliveryTime = null, seenTime = null;
            Nuntias nuntias = null;
            while (reader.Read())
            {
                try
                {
                    id = long.Parse(reader["id"].ToString());
                    text = reader["Text"].ToString();
                    senderId = long.Parse(reader["Sender_id"].ToString());
                    conversationId = long.Parse(reader["Conversation_id"].ToString());
                    if (reader["Sent_time"].ToString().Length > 0) sentTime = new Time(DateTime.Parse(reader["Sent_time"].ToString()));
                    if (reader["delivery_time"].ToString().Length > 0) deliveryTime = new Time(DateTime.Parse(reader["delivery_time"].ToString()));
                    if (reader["seen_time"].ToString().Length > 0) seenTime = new Time(DateTime.Parse(reader["seen_time"].ToString()));
                    nuntias = new Nuntias((long)id, text, (long)senderId, sentTime, deliveryTime, seenTime, contentPath, (long)conversationId);
                    if (reader["Content_ID"].ToString().Length > 0) nuntias.ContentFileId = reader["Content_ID"].ToString();
                }
                catch { }
                this.CloseConnection();
                return nuntias;
            }
            return null;
        }

        public Nuntias Get(long nuntiasID)
        {
            string query = "select * from Nuntii where Id = " + nuntiasID;
            return this.GetNuntiasByQyery(query);
        }

        internal List<Nuntias> GetNuntiasListByQuery(string query)
        {
            SqlDataReader reader = this.ReadSqlData(query);
            if (reader == null) return null;
            List<Nuntias> nuntiasList = new List<Nuntias>();
            while (reader.Read())
            {
                try
                {
                    long? id = null, senderId = null, conversationId = null;
                    string text = null, contentPath = null;
                    Time sentTime = null, deliveryTime = null, seenTime = null;
                    Nuntias nuntias = null;
                    id = long.Parse(reader["id"].ToString());
                    text = reader["Text"].ToString();
                    senderId = long.Parse(reader["Sender_id"].ToString());
                    conversationId = long.Parse(reader["Conversation_id"].ToString());
                    if (reader["Sent_time"].ToString().Length > 0) sentTime = new Time(DateTime.Parse(reader["Sent_time"].ToString()));
                    if (reader["delivery_time"].ToString().Length > 0) deliveryTime = new Time(DateTime.Parse(reader["delivery_time"].ToString()));
                    if (reader["seen_time"].ToString().Length > 0) seenTime = new Time(DateTime.Parse(reader["seen_time"].ToString()));
                    nuntias = new Nuntias((long)id, text, (long)senderId, sentTime, deliveryTime, seenTime, contentPath, (long)conversationId);
                    if (reader["Content_ID"].ToString().Length > 0) nuntias.ContentFileId = reader["Content_ID"].ToString();
                    nuntiasList.Add(nuntias);
                }
                catch (Exception ex)
                {
                    Output.ShowLog(ex.Message);
                }
            }
            this.CloseConnection();
            return nuntiasList;
        }

        public bool DeleteNuntias(long ownerId, long nuntiasId, bool forBoth)
        {
            if(forBoth)
            {
                string sql = "DECLARE @username nvarchar(20); SELECT @username = Username FROM Users WHERE Id = " + ownerId + "; IF ((SELECT DATEDIFF(second, (SELECT Sent_time FROM Nuntii WHERE Id = " + nuntiasId + "), SYSDATETIME())) <= 3000)\n";
                sql += "BEGIN UPDATE Nuntii SET Text = CONCAT('Deleted by ',@username), Content_Id = 'deleted' WHERE Id = " + nuntiasId + "; SELECT 'ok' as result; END\n";
                sql += "ELSE BEGIN SELECT 'timeout' as result; END";
                string result = this.ExecuteSqlScalar(sql).ToString();
                if (result == "ok") return true;
                return false;
            }
            else
            {
                string query = "DELETE FROM Nuntius_Owner_map WHERE Nuntias_Id = " + nuntiasId + " and Owner_Id = " + ownerId;
                int? result = this.ExecuteSqlQuery(query);
                if (result == null || result == 0) return false;
                return true;
            }
        }

        public List<Nuntias> GetNuntiasListOfUser(long userId, long lastLocalNuntiasId)
        {
            string query = "UPDATE Nuntii SET Delivery_time = '" + Time.CurrentTime.DbFormat + "' where Delivery_time is null and Sender_Id <> " + userId + " and id in (SELECT Nuntias_Id FROM Nuntius_Owner_map WHERE Owner_Id = " + userId + " and Nuntias_Id > " + lastLocalNuntiasId + "); ";
            query += "UPDATE Nuntius_Owner_map SET Delivery_Status = 1 where Delivery_Status = 0 and Owner_Id = " + userId + " and Nuntias_Id > " + lastLocalNuntiasId + ";";
            this.ExecuteSqlScalar(query);
            query = "SELECT * FROM Nuntii WHERE id in (SELECT Nuntias_Id FROM Nuntius_Owner_map WHERE Owner_Id = " + userId + " and Nuntias_Id > " + lastLocalNuntiasId + ")";
            return GetNuntiasListByQuery(query);
        }

        new public static NuntiasRepository Instance
        {
            get { return new NuntiasRepository(); }
        }


        public bool? Update(Nuntias item)
        {
            throw new NotImplementedException();
        }
    }
}