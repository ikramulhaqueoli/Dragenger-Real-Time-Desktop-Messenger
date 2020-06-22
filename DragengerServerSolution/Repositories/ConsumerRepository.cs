using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using EntityLibrary;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using Newtonsoft.Json.Linq;
using Display;

namespace Repositories
{
    public class ConsumerRepository : DatabaseAccess, IConsumerRepository
    {
        public long? Insert(Consumer consumer)
        {
            int? success = this.ExecuteSqlQuery("INSERT INTO Consumers (User_ID,Name,Email) values ('" + consumer.Id + "','" + consumer.Name + "','" + consumer.Email + "')");
            if (success == null) return null;
            if (success > 0) return consumer.Id;
            return null;
        }

        public bool? Update(Consumer consumer)
        {
            if (consumer == null) return null;
            string gender = "0", birthdate = "NULL", profileImgId = "NULL", phone = "NULL";
            if (consumer.Birthdate != null) birthdate = "'" + consumer.Birthdate.DbFormat + "'";
            if (consumer.GenderIndex > 0) gender = "" + consumer.GenderIndex;
            if (consumer.ProfileImageId != null && consumer.ProfileImageId.Length > 0) profileImgId = "'" + consumer.ProfileImageId + "'";
            if (consumer.Phone != null && consumer.Phone.Length > 0) phone = "'" + consumer.Phone + "'";
            string query = "UPDATE Consumers SET Name = '" + consumer.Name + "', Email = '" + consumer.Email + "', Birthdate = " + birthdate + ", Gender = " + consumer.GenderIndex + ", Phone = "+phone+" WHERE User_Id = " + consumer.Id + ";";
            query += "UPDATE Users SET Username = '"+ consumer.Username +"' WHERE Id = " + consumer.Id + ";";
            string success = this.ExecuteSqlScalar(query);
            if (success == null) return null;
            return true;
        }

        public bool? Delete(long primarykey)
        {
            throw new NotImplementedException();
        }

        private Consumer GetConsumerByQuery(string query)
        {
            SqlDataReader data = this.ReadSqlData(query);
            if (data == null) return null;
            while (data.Read())
            {
                Consumer consumer = new Consumer(long.Parse(data["User_Id"].ToString()), data["username"].ToString(), data["password"].ToString(), data["email"].ToString(), data["name"].ToString());
                string ProfileImgId = data["Profile_img_ID"].ToString();
                string birthdate = data["birthdate"].ToString();
                string genderIndex = data["gender"].ToString();
                string phone = data["phone"].ToString();
                this.CloseConnection();
                if (ProfileImgId.Length > 5) consumer.ProfileImageId = ProfileImgId;
                if (birthdate.Length > 0) consumer.Birthdate = new Time(Convert.ToDateTime(birthdate));
                if (phone.Length > 0) consumer.Phone = phone;
                if (genderIndex.Length > 0) consumer.GenderIndex = int.Parse(genderIndex);
                return consumer;
            }
            return null;
        }

        public Consumer Get(long userId)
        {
            string query = "SELECT U.*, C.* from Users U, Consumers C where C.User_ID = U.Id and C.User_ID = " + userId;
            return GetConsumerByQuery(query);
        }

        private List<Consumer> ConsumerListFromQuery(string query)
        {
            SqlDataReader data = this.ReadSqlData(query);
            if (data == null) return null;
            List<Consumer> consumerList = new List<Consumer>();
            List<string> profileImgIdList = new List<string>(), birthdateList = new List<string>(), genderIndexList = new List<string>();
            while (data.Read())
            {
                string userId = "", username = "", password = "", email = "", name = "", profileImgId = "", birthDate = "", gender = "";
                try { userId = data["Id"].ToString();} catch { }
                try { username = data["username"].ToString();} catch { }
                try { password = data["password"].ToString();} catch { }
                try { email = data["email"].ToString();} catch { }
                try { name = data["name"].ToString();} catch { }
                try { profileImgId = data["Profile_img_ID"].ToString();} catch { }
                try { birthDate = data["birthdate"].ToString();} catch { }
                try { gender = data["gender"].ToString();} catch { }
                Consumer consumer = new Consumer(long.Parse(userId), username, password, email, name);
                profileImgIdList.Add(profileImgId);
                birthdateList.Add(birthDate);
                genderIndexList.Add(gender);
                consumerList.Add(consumer);
            }
            this.CloseConnection();
            for (int i = 0; i < profileImgIdList.Count; i++)
            {
                if (profileImgIdList[i].Length > 5) consumerList[i].ProfileImageId = profileImgIdList[i];
                if (birthdateList[i].Length > 0) consumerList[i].Birthdate = new Time(Convert.ToDateTime(birthdateList[i]));
                if (genderIndexList[i].Length > 0) consumerList[i].GenderIndex = int.Parse(genderIndexList[i]);
            }
            return consumerList;
        }

        public List<Consumer> SearchTop20FriendRequestsByKeyword(long userId, string keyword)
        {
            keyword = keyword.ToLower();
            string query = "SELECT TOP(20) U.Id, U.Username, U.Last_Active, C.Name, C.Profile_img_ID FROM Users U, Consumers C (LOWER(U.username)='" + keyword + "' or LOWER(C.Name) like '%" + keyword + "%' or LOWER(C.Email) = '" + keyword + "') and U.Id = C.User_Id and U.Id IN ( SELECT sender_Id FROM FriendRequests_map WHERE receiver_Id = " + userId + " or sender_Id = " + userId + ")";
            return ConsumerListFromQuery(query);
        }

        public List<Consumer> SearchTop20NonfriendPersonsByKeyword(long userId, string keyword)
        {
            keyword = keyword.ToLower();
            string query = "SELECT TOP (20) U.Id, U.Username, U.Last_Active, C.Name, C.Profile_img_ID FROM Users U, Consumers C where (LOWER(U.username)='" + keyword + "' or LOWER(C.Name) like '%" + keyword + "%' or LOWER(C.Email) = '" + keyword + "') and U.Id = C.User_Id and U.Id NOT IN ( SELECT User_Id_2 FROM Friendship_Map WHERE User_Id_1 = " + userId + " union all SELECT User_Id_1 FROM Friendship_Map WHERE User_Id_2 = " + userId + " ) and U.Id <> " + userId;
            List<Consumer> fetchedList = ConsumerListFromQuery(query);
            return fetchedList;
        }

        public List<Consumer> GetFriendRequestListOf(long userId, string keyword)
        {
            keyword = keyword.ToLower();
            string query = "SELECT U.Id, U.Username, U.Last_Active, C.Name, C.Profile_img_ID FROM Users U, Consumers C where (LOWER(U.username)='" + keyword + "' or LOWER(C.Name) like '%" + keyword + "%' or LOWER(C.Email) = '" + keyword + "') and U.Id = C.User_Id and U.Id IN ( SELECT Receiver_Id FROM Friend_request_Map WHERE Sender_Id = " + userId + " union all SELECT Sender_Id FROM Friend_request_Map WHERE Receiver_Id = " + userId + " ) and U.Id <> " + userId;
            List<Consumer> fetchedList = ConsumerListFromQuery(query);
            return fetchedList;
        }

        public List<Consumer> GetTop20FriendListOf(long userId, string keyword)
        {
            keyword = keyword.ToLower();
            string query = "SELECT TOP (20) U.Id, U.Username, U.Last_Active, C.Name, C.Profile_img_ID, C.Email FROM Users U, Consumers C where (LOWER(U.username)='" + keyword + "' or LOWER(C.Name) like '%" + keyword + "%' or LOWER(C.Email) = '" + keyword + "') and U.Id = C.User_Id and U.Id IN ( SELECT User_Id_2 FROM Friendship_Map WHERE User_Id_1 = " + userId + " union all SELECT User_Id_1 FROM Friendship_Map WHERE User_Id_2 = " + userId + " ) and U.Id <> " + userId;
            List<Consumer> fetchedList = ConsumerListFromQuery(query);
            return fetchedList;
        }

        public Consumer GetConsumerByMacAddress(string deviceMac)
        {
            string sql = "SELECT U.*, C.* FROM Users U, Consumers C WHERE U.Id = C.User_ID AND U.Id = (SELECT User_Id from Devices_Bind_Map where Device_Mac = '" + deviceMac + "')";
            return this.GetConsumerByQuery(sql);
        }

        public bool? EmailAlreadyExists(string email)
        {
            SqlDataReader reader = this.ReadSqlData("SELECT Email FROM Consumers WHERE Email = '" + email + "'");
            if (reader == null) return null;
            return reader.Read();
        }

        public bool SetFriendRequest(long senderId, long receiverId)
        {
            string query = "INSERT INTO Friend_request_Map (Sender_Id, Receiver_Id) VALUES (" + senderId + ", " + receiverId + ")";
            int? success = this.ExecuteSqlQuery(query);
            if (success == null || success == 0) return false;
            return true;
        }

        public bool DeleteFriendRequest(long userId1, long userId2)
        {
            string query = "DELETE FROM Friend_request_Map where (Sender_Id = " + userId1 + " and Receiver_Id = " + userId2 + ") or (Receiver_Id = " + userId1 + " and Sender_Id = " + userId2 + ")";
            int? result = this.ExecuteSqlQuery(query);
            if (result == null || result == 0) return false;
            return true;
        }

        public bool AcceptFriendRequest(long senderId, long receiverId)
        {
            string query = "INSERT INTO Friendship_Map (User_id_1, User_id_2) OUTPUT 'success' values (" + senderId + ", " + receiverId + "); DELETE FROM Friend_request_Map where (Sender_Id = " + senderId + " and Receiver_Id = " + receiverId + ") or (Receiver_Id = " + senderId + " and Sender_Id = " + receiverId + ")";
            Object result = this.ExecuteSqlScalar(query);
            if (result == null) return false;
            if (result.ToString() == "success") return true;
            return false;
        }

        public bool Unfriend(Consumer consumer)
        {
            string query = "DELETE FROM FRIENDSHIPS WHERE (username_1 = '" + null + "' and username_2 = '" + null + "') or (username_1 = '" + null + "' and username_2 = '" + null + "')";
            int? success = this.ExecuteSqlQuery(query);
            if (success == null || success == 0) return false;
            return true;
        }

        public bool? IsFriend(Consumer consumer)
        {
            string query = "SELECT username_1 FROM FRIENDSHIPS WHERE (username_1 = '" + null + "' and username_2 = '" + null + "') or (username_1 = '" + null + "' and username_2 = '" + null + "')";
            SqlDataReader reader = this.ReadSqlData(query);
            if (reader == null) return null;
            return reader.Read();
        }

        public Time FriendAddedTime(Consumer consumer)
        {
            string query = "SELECT adding_date FROM FRIENDSHIPS WHERE (username_1 = '" + null + "' and username_2 = '" + null + "') or (username_1 = '" + null + "' and username_2 = '" + null + "')";
            SqlDataReader reader = this.ReadSqlData(query);
            if (reader == null) return null;
            if(reader.Read())
            {
                return new Time((DateTime)reader.GetValue(0));
            }
            return null;
        }

        public List<JObject> SetFriendRequestStatusOfPersonList(long userId, List<JObject> metchedList)
        {
            Dictionary<long, JObject> consumerDictionary = new Dictionary<long,JObject>();
            foreach (JObject consumerJson in metchedList) consumerDictionary[(long)consumerJson["id"]] = consumerJson;
            string senderListQuery = "select sender_id from Friend_request_Map where Receiver_Id = " + userId + " and sender_id in (";
            string receiverListQuery = "select Receiver_Id from Friend_request_Map where Sender_Id = " + userId + " and Receiver_Id in (";
            int comaFlag = 0;
            foreach(JObject consumerJson in metchedList)
            {
                long id = (long)consumerJson["id"];
                senderListQuery += id;
                receiverListQuery += id;
                comaFlag++;
                if (comaFlag < metchedList.Count)
                {
                    senderListQuery += ',';
                    receiverListQuery += ',';
                }
            }
            senderListQuery += ')';
            receiverListQuery += ')';
            SqlDataReader data = ReadSqlData(senderListQuery);
            while(data.Read())
            {
                long foundId = (long)data["sender_id"];
                consumerDictionary[foundId]["frequest_status"] = "r_sender";
            }
            this.CloseConnection();

            data = ReadSqlData(receiverListQuery);
            while (data.Read())
            {
                long foundId = (long)data["receiver_id"];
                consumerDictionary[foundId]["frequest_status"] = "r_receiver";
            }
            this.CloseConnection();
            return metchedList;
        }

        new public static ConsumerRepository Instance
        {
            get { return new ConsumerRepository(); }
        }

        public List<long> ConversationMemberIdList(long conversationId)
        {
            /*  query pattern
                DECLARE @conversation_type NVARCHAR(10);
                SET @conversation_type = (SELECT type FROM Conversations WHERE Id = 201)
                IF @conversation_type = 'duet'
                BEGIN
	                SELECT member_id from Duet_Conversations unpivot
	                (
	                  member_id
	                  FOR col in (Member_Id_1, Member_Id_2)
	                ) un WHERE Conversation_Id = 201
                END
                ELSE IF @conversation_type = 'group'
                BEGIN
	                SELECT Member_Id from Group_Member_Map where Conversation_Id = 201;
                END
            */

            string query = "DECLARE @conversation_type NVARCHAR(10);\n";
            query += "SET @conversation_type = (SELECT type FROM Conversations WHERE Id = "+conversationId+")\n";
            query += "IF @conversation_type = 'duet'\n";
            query += "BEGIN\n";
	        query += "    SELECT member_id from Duet_Conversations unpivot\n";
	        query += "    (\n";
	        query += "      member_id\n";
	        query += "      FOR col in (Member_Id_1, Member_Id_2)\n";
	        query += "    ) un WHERE Conversation_Id = "+conversationId+" \n";
            query += "END\n";
            query += "ELSE IF @conversation_type = 'group'\n";
            query += "BEGIN\n";
            query += "    SELECT member_Id from Group_Member_Map where Conversation_Id = " + conversationId + ";\n";
            query += "END\n";
            SqlDataReader data = this.ReadSqlData(query);
            List<long> memberIdList = new List<long>();
            while(data.Read())
            {
                memberIdList.Add(long.Parse(data["member_Id"].ToString()));
            }
            return memberIdList;
        }

        public string DeleteConsumerAccount(string macAdress, string password)
        {
            string query = "SELECT U.Password FROM Users U, Consumers C WHERE U.Id = (SELECT User_Id FROM Devices_Bind_Map WHERE Device_Mac = '" + macAdress + "') and U.Id = C.User_Id";
            SqlDataReader data = this.ReadSqlData(query);
            if (data == null) return "Error encountered while\r\nfetching account information.";
            if (data.Read())
            {
                if(!data["Password"].ToString().Equals(password))
                {
                    return "Incorrect Current Password!";
                }
            }
            else
            {
                return "Consumer Account not Found!";
            }
            this.CloseConnection();
            query = "DECLARE @user_id bigint; SELECT @user_id = User_Id FROM Devices_Bind_Map WHERE Device_Mac = '" + macAdress + "';";
            query += "DELETE FROM Block_List_Map WHERE Blocker_ID = @user_id or Blocked_ID = @user_id;";
            query += "DELETE FROM Devices_Bind_Map WHERE User_id = @user_id;";
            query += "DELETE FROM Friend_request_Map WHERE Sender_Id = @user_id or Receiver_Id = @user_id;";
            query += "DELETE FROM Friendship_Map WHERE User_id_1 = @user_id or User_id_2 = @user_id;";
            query += "DELETE FROM Group_Member_Map WHERE Member_Id = @user_id;";
            query += "DELETE FROM Nuntius_Owner_map WHERE Owner_Id = @user_id;";
            query += "UPDATE Consumers SET Name = 'Dragenger User', Profile_img_ID = NULL, Birthdate = NULL, Phone = NULL, Gender = 0, Email = CONCAT(@user_id,'@dragenger.com') WHERE User_Id = @user_id;";
            query += "UPDATE Users SET Username = CONCAT(@user_id,'dragenger_user'), Password = NULL, Last_Active = NULL WHERE Id = @user_id;";
            string result = this.ExecuteSqlScalar(query);
            if (result == null) return "Unknown error encountered!";
            return "success";
        }

        public long UserIdByDeviceMac(string deviceMac)
        {
            throw new NotImplementedException();
        }

        public string ReassignConsumerProfileImgId(long userId, string newProfileImgId)
        {
            string query = "SELECT Profile_img_ID FROM Consumers WHERE User_ID = " + userId + "; UPDATE Consumers set Profile_img_ID = '" + newProfileImgId + "' WHERE User_ID = " + userId;
            return this.ExecuteSqlScalar(query);
        }
    }
}