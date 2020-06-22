using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using EntityLibrary;
using System.Drawing;
using System.Data.SqlServerCe;
using System.Data;

namespace LocalRepository
{
    public class NuntiasRepository : DatabaseAccess, INuntiasRepository
    {
        public long? Insert(Nuntias nuntias)
        {
            string sentTimeStr = "NULL", deliveryTimeStr = "NULL", seenTimeStr = "NULL", nuntiasText = "NULL", contentFileId = "NULL"; ;
            if (nuntias.SentTime != null) sentTimeStr = "'" + nuntias.SentTime.DbFormat + "'";
            if (nuntias.DeliveryTime != null) deliveryTimeStr = "'" + nuntias.DeliveryTime.DbFormat + "'";
            if (nuntias.SeenTime != null) seenTimeStr = "'" + nuntias.SeenTime.DbFormat + "'";
            if (nuntias.Text != null && nuntias.Text.Length > 0) nuntiasText = "'" + nuntias.Text + "'";
            if (nuntias.ContentFileId != null && nuntias.ContentFileId.Length > 0) contentFileId = "'" + nuntias.ContentFileId + "'";
            string sql = "INSERT INTO Nuntii (Id, text,Sender_id,Sent_time,Delivery_time,Seen_time,Conversation_id,Content_Id) values (" + nuntias.Id + "," + nuntiasText + "," + nuntias.SenderId + "," + sentTimeStr + "," + deliveryTimeStr + "," + seenTimeStr + "," + nuntias.NativeConversationID + ","+contentFileId+");";
            int? result = this.ExecuteSqlCeQuery(sql);
            if (result == null) return null;
            sql = "UPDATE Conversations SET Last_nuntias_id = " + nuntias.Id + " where Id = " + nuntias.NativeConversationID + ";";
            result = this.ExecuteSqlCeQuery(sql);
            if (result == null) return null;
            return nuntias.Id;
        }

        public bool? Update(Nuntias item)
        {
            string deliveryTimeStr = "NULL", seenTimeStr = "NULL", contentFileId = "NULL";
            if (item.DeliveryTime != null) deliveryTimeStr = "'" + item.DeliveryTime.DbFormat + "'";
            if (item.SeenTime != null) seenTimeStr = "'" + item.SeenTime.DbFormat + "'";
            if (item.ContentFileId != null && item.ContentFileId.Length > 0) contentFileId = "'" + item.ContentFileId + "'";
            string query = "UPDATE Nuntii SET Text = '" + item.Text + "', Delivery_time = " + deliveryTimeStr + ", Seen_time = " + seenTimeStr + ", Content_Id = " + contentFileId + " where Id = " + item.Id;
            int? success = this.ExecuteSqlCeQuery(query);
            if (success == null) return null;
            return (success != 0);
        }

        public bool? Delete(long nuntiasID)
        {
            string query = "DELETE FROM Nuntii WHERE Id = " + nuntiasID;
            int? success = this.ExecuteSqlCeQuery(query);
            if (success == null) return null;
            return (success != 0);
        }

        internal Nuntias GetNuntiasByQyery(string query)
        {
            SqlCeDataReader reader = this.ReadSqlCeData(query);
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
                    if(reader["Content_Id"].ToString().Length > 0) nuntias.ContentFileId = reader["Content_Id"].ToString();
                }
                catch { }
                this.CloseConnection();
                return nuntias;
            }
            return null;
        }

        public Nuntias Get(long nuntiasID)
        {
            string query = "select * from Nuntias where id = " + nuntiasID;
            return this.GetNuntiasByQyery(query);
        }

        internal List<Nuntias> GetNuntiasListByQuery(string query)
        {
            SqlCeDataReader reader = this.ReadSqlCeData(query);
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
                    if (reader["Content_Id"].ToString().Length > 0) nuntias.ContentFileId = reader["Content_Id"].ToString();
                    nuntiasList.Add(nuntias);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            this.CloseConnection();
            return nuntiasList;
        }

        internal List<Nuntias> GetPendingNuntiasListByQuery(string query)
        {
            SqlCeDataReader reader = this.ReadSqlCeData(query);
            if (reader == null) return null;
            List<Nuntias> nuntiasList = new List<Nuntias>();
            while (reader.Read())
            {
                try
                {
                    long? tmpId = null, senderId = null, conversationId = null;
                    string text = null, contentPath = null;
                    Time sentTime = null, deliveryTime = null, seenTime = null;
                    Nuntias nuntias = null;
                    tmpId = long.Parse(reader["Temp_id"].ToString());
                    text = reader["Text"].ToString();
                    senderId = long.Parse(reader["Sender_id"].ToString());
                    conversationId = long.Parse(reader["Conversation_id"].ToString());
                    if (reader["Sent_time"].ToString().Length > 0) sentTime = new Time(DateTime.Parse(reader["Sent_time"].ToString()));
                    nuntias = new Nuntias(0, text, (long)senderId, sentTime, deliveryTime, seenTime, contentPath, (long)conversationId);
                    nuntias.Id = -tmpId ?? 0;
                    if (reader["Content_Id"].ToString().Length > 0) nuntias.ContentFileId = reader["Content_Id"].ToString();
                    nuntiasList.Add(nuntias);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            this.CloseConnection();
            return nuntiasList;
        }

        new public static NuntiasRepository Instance
        {
            get { return new NuntiasRepository(); }
        }

        public long? StoreTmpNuntias(Nuntias nuntias)
        {
            string nuntiasText = "NULL", contentFileId = "NULL";
            if (nuntias.Text != null && nuntias.Text.Length > 0) nuntiasText = "'" + nuntias.Text + "'";
            if (nuntias.ContentFileId != null && nuntias.ContentFileId.Length > 0) contentFileId = "'" + nuntias.ContentFileId + "'";
            string sql = "INSERT INTO Nuntii_to_be_sent (text,Sender_id,Sent_time,Conversation_id,Content_Id) values (" + nuntiasText + "," + nuntias.SenderId + ",'" + nuntias.SentTime.DbFormat + "'," + nuntias.NativeConversationID + "," + contentFileId + ");";
            int? result = this.ExecuteSqlCeQuery(sql);
            if (result == null || result == 0) return null;
            SqlCeDataReader data = this.ReadSqlCeData("SELECT MAX(Temp_Id) as id FROM Nuntii_to_be_sent;");
            long? nuntiasLocalTmpId = null;
            //nuntias.Id is given negative value so that is can be identified as localId.
            if (data.Read())
            {
                nuntiasLocalTmpId = -(long.Parse(data["id"].ToString()));
            }
            return nuntiasLocalTmpId;
        }

        public List<Nuntias> GetAllPendingPendingNuntiiList()
        {
            string query = "SELECT * from Nuntii_to_be_sent";
            return this.GetPendingNuntiasListByQuery(query);
        }

        public int? DeleteTmpNuntias(long nuntiasTmpId)
        {
            List<long> tmpIdList = new List<long>();
            tmpIdList.Add(nuntiasTmpId);
            return DeleteTmpNuntii(tmpIdList);
        }

        public int? DeleteTmpNuntii(List<long> tmpIdList)
        {
            if (tmpIdList == null || tmpIdList.Count == 0) return 0;
            string query = "DELETE FROM Nuntii_to_be_sent WHERE Temp_id in (";
            query += Math.Abs(tmpIdList[0]);
            for (int i = 1; i < tmpIdList.Count; i++ )
            {
                query += "," + Math.Abs(tmpIdList[i]);
            }
            query += ')';
            return this.ExecuteSqlCeQuery(query);
        }

        public long? LastLocalNuntiasId 
        { 
            get
            {
                string query = "SELECT MAX(Id) max_id from Nuntii";
                SqlCeDataReader data = this.ReadSqlCeData(query);
                if (data == null) return null;
                if (data.Read())
                {
                    try
                    {
                        return long.Parse(data["max_id"].ToString());
                    }
                    catch
                    {
                        return 0;
                    }
                }
                return 0;
            }
        }
    }
}