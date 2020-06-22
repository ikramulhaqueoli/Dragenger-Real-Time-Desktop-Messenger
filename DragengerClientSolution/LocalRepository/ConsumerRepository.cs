using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using EntityLibrary;
using System.Drawing;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Data.SqlServerCe;

namespace LocalRepository
{
    public class ConsumerRepository : DatabaseAccess, IConsumerRepository
    {
        public long? Insert(Consumer consumer)
        {
            string profileImgId = "NULL", gender = "0", birthdate = "NULL", phone = "NULL", last_active = "NULL";
            if(consumer.ProfileImageId != null && consumer.ProfileImageId.Length > 5) profileImgId = "'" + consumer.ProfileImageId + "'";
            gender = "" + consumer.GenderIndex;
            if (consumer.Birthdate != null) birthdate = "'" + consumer.Birthdate.DbFormat +"'";
            if (consumer.Phone != null && consumer.Phone.Length > 5) phone = "'" + consumer.Phone +"'";
            if (consumer.LastActive != null) last_active = "'" + consumer.LastActive.DbFormat +"'";
            string sql = "INSERT INTO Consumers (User_ID,Username,Name,Email,Profile_img_ID,Birthdate,Phone,Gender,Last_Active) values ('" + consumer.Id + "','"+consumer.Username+"','" + consumer.Name + "','" + consumer.Email + "',"+profileImgId+","+birthdate+","+phone+","+gender+","+last_active+");";
            int? success = this.ExecuteSqlCeQuery(sql);
            if (success == null || success == 0) return null;
            return consumer.Id;
        }

        public bool? Update(Consumer consumer)
        {
            string sql = "DELETE from Consumers WHERE User_ID = " + consumer.Id;
            this.ExecuteSqlCeQuery(sql);
            long? id = Insert(consumer);
            if (id == null) return null;
            if (id > 0) return true;
            return false;
        }

        public bool? Delete(long primarykey)
        {
            throw new NotImplementedException();
        }

        private Consumer GetConsumerByQuery(string query)
        {
            SqlCeDataReader data = this.ReadSqlCeData(query);
            if (data == null) return null;
            while (data.Read())
            {
                Consumer consumer = new Consumer(long.Parse(data["User_Id"].ToString()), data["username"].ToString(), data["email"].ToString(), data["name"].ToString());
                string ProfileImgId = data["Profile_img_ID"].ToString();
                string birthdate = data["birthdate"].ToString();
                string genderIndex = data["gender"].ToString();
                this.CloseConnection();
                if (ProfileImgId.Length > 5) consumer.SetProfileImage(ProfileImgId);
                if (birthdate.Length > 0) consumer.Birthdate = new Time(Convert.ToDateTime(birthdate));
                if (genderIndex.Length > 0) consumer.GenderIndex = int.Parse(genderIndex);
                return consumer;
            }
            return null;
        }

        public Consumer Get(long userId)
        {
            string query = "SELECT * from Consumers where User_ID = " + userId;
            return GetConsumerByQuery(query);
        }

        private List<Consumer> ConsumerListFromQuery(string query)
        {
            SqlCeDataReader data = this.ReadSqlCeData(query);
            if (data == null) return null;
            List<Consumer> consumerList = new List<Consumer>();
            List<string> profileImgIdList = new List<string>(), birthdateList = new List<string>(), genderIndexList = new List<string>();
            while (data.Read())
            {
                string userId = "", username = "", email = "", name = "", profileImgId = "", birthDate = "", gender = "";
                try { userId = data["Id"].ToString();} catch { }
                try { username = data["username"].ToString();} catch { }
                try { email = data["email"].ToString();} catch { }
                try { name = data["name"].ToString();} catch { }
                try { profileImgId = data["Profile_img_ID"].ToString();} catch { }
                try { birthDate = data["birthdate"].ToString();} catch { }
                try { gender = data["gender"].ToString();} catch { }
                Consumer consumer = new Consumer(long.Parse(userId), username, email, name);
                profileImgIdList.Add(profileImgId);
                birthdateList.Add(birthDate);
                genderIndexList.Add(gender);
                consumerList.Add(consumer);
            }
            this.CloseConnection();
            for (int i = 0; i < profileImgIdList.Count; i++)
            {
                if (profileImgIdList[i].Length > 0) consumerList[i].SetProfileImage(profileImgIdList[i]);
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
            string query = "SELECT TOP (20) U.Id, U.Username, U.Last_Active, C.Name, C.Profile_img_ID FROM Users U, Consumers C where (LOWER(U.username)='" + keyword + "' or LOWER(C.Name) like '%" + keyword + "%' or LOWER(C.Email) = '" + keyword + "') and U.Id = C.User_Id and U.Id NOT IN ( SELECT User_Id_2 FROM Friendship_Map WHERE User_Id_1 = " + userId + " ) and U.Id NOT IN ( SELECT User_Id_1 FROM Friendship_Map WHERE User_Id_2 = " + userId + " ) and U.Id <> " + userId;
            List<Consumer> fetchedList = ConsumerListFromQuery(query);
            return fetchedList;
        }

        public List<Consumer> GetFriendListOf(long userId)
        {
            string query = "SELECT U.Id, U.Username, U.Last_Active, C.Name, C.Profile_img_ID FROM Users U, Consumers C where U.Id = C.User_id and U.Id in ((SELECT User_id_1 from Friendship_Map where User_id_2 = " + userId + ") union (SELECT User_id_2 from Friendship_Map where User_id_1 = " + userId + "))";
            List<Consumer> fetchedList = ConsumerListFromQuery(query);
            return fetchedList;
        }

        public List<Consumer> SearchFriendsByKeyword(string keyword)
        {
            keyword = keyword.ToLower();
            string query = "SELECT U.*, L.Secret_key FROM Consumers U,Logins L where (LOWER(U.username)='" + keyword + "' or LOWER(U.Name) like '" + keyword + "%' or LOWER(U.Name) like '% " + keyword + "%' or LOWER(U.Email) = '" + keyword + "') and U.Login_Id = L.Device_Id and u.username IN ( SELECT USERNAME_1 FROM Friendships WHERE username_2 = '" + null + "' ) or u.username IN ( SELECT USERNAME_2 FROM Friendships WHERE username_1 = '" + null + "' ) and u.username <> '" + null + "'";
            return ConsumerListFromQuery(query);
        }

        public bool? EmailAlreadyExists(string email)
        {
            SqlCeDataReader reader = this.ReadSqlCeData("SELECT Email FROM Consumers WHERE Email = '" + email + "'");
            if (reader == null) return null;
            return reader.Read();
        }

        public bool SetFriendRequest(long senderId, long receiverId)
        {
            string query = "INSERT INTO Friend_request_Map (Sender_Id, Receiver_Id) VALUES (" + senderId + ", " + receiverId + ")";
            int? success = this.ExecuteSqlCeQuery(query);
            if (success == null || success == 0) return false;
            return true;
        }

        public bool DeleteFriendRequest(long userId1, long userId2)
        {
            string query = "DELETE FROM Friend_request_Map where (Sender_Id = " + userId1 + " and Receiver_Id = " + userId2 + ") or (Receiver_Id = " + userId1 + " and Sender_Id = " + userId2 + ")";
            int? result = this.ExecuteSqlCeQuery(query);
            if (result == null || result == 0) return false;
            return true;
        }

        public bool AcceptFriendRequest(long senderId, long receiverId)
        {
            string query = "INSERT INTO Friendship_Map (User_id_1, User_id_2) OUTPUT 'success' values (" + senderId + ", " + receiverId + "); DELETE FROM Friend_request_Map where (Sender_Id = " + senderId + " and Receiver_Id = " + receiverId + ") or (Receiver_Id = " + senderId + " and Sender_Id = " + receiverId + ")";
            Object result = this.ExecuteSqlCeScalar(query);
            if (result == null) return false;
            if (result.ToString() == "success") return true;
            return false;
        }

        public string UpdateProfileImageTimeStamp(string timestamp, string consumerLoginId)
        {
            string query = null;
            if (timestamp != null) query = "UPDATE Consumers set profile_img_timestamp = '" + timestamp + "' Output Deleted.profile_img_timestamp where Login_Id = '" + consumerLoginId + "'";
            else query = "UPDATE Consumers set profile_img_timestamp = NULL Output Deleted.profile_img_timestamp where Login_Id = '" + consumerLoginId + "'";
            string oldTimeStamp = this.ExecuteSqlCeScalar(query);
            return oldTimeStamp;
        }

        public string DeleteConsumerIfPasswordMatch(string deviceId, string inputPassword)
        {
            if (inputPassword == null || inputPassword.Length == 0) return "Login-key not entered!";
            string query = "SELECT U.username, L.Secret_key FROM Consumers U, Logins L WHERE l.device_id = '" + deviceId + "' and u.login_id = l.device_id";
            SqlCeDataReader data = this.ReadSqlCeData(query);
            if (data == null) return "Error encountered while\r\nfetching account information.";
            string secretKey = null, username = null;
            if (data.Read())
            {
                secretKey = data["Secret_key"].ToString();
                username = data["username"].ToString();
                this.CloseConnection();
            }
            if (secretKey == null || secretKey != inputPassword) return "Login-key is incorrect!";
            query = "DELETE FROM Friendships where username_1 = '" + username + "' or username_2 = '" + username + "';";
            query += "DELETE FROM FriendRequests where sender_username = '" + username + "' or receiver_username = '" + username + "';";
            this.ExecuteSqlCeQuery(query);
            this.CloseConnection();
            List<string> conversationIdList = new List<string>();
            query = "SELECT Id from DuetConversations where username_1 = '" + username + "' or username_2 = '" + username + "';";
            data = this.ReadSqlCeData(query);
            if (data != null)
                while (data.Read())
                {
                    conversationIdList.Add(data["Id"].ToString());
                }
            this.CloseConnection();
            query = "";
            if (conversationIdList.Count > 0)
            {
                query = "delete from DuetConversations where Id in (";
                for (int i = 0; i < conversationIdList.Count - 1; i++)
                {
                    query += conversationIdList[i] + ",";
                }
                query += conversationIdList[conversationIdList.Count - 1] + ");";
                query += "delete from Nuntias where Conversation_Id in (";
                for (int i = 0; i < conversationIdList.Count - 1; i++)
                {
                    query += conversationIdList[i] + ",";
                }
                query += conversationIdList[conversationIdList.Count - 1] + ");";
            }
            query += "DELETE FROM USERS WHERE login_id = '" + deviceId + "';";
            query += "DELETE FROM Logins WHERE device_id = '" + deviceId + "';";
            int? success = this.ExecuteSqlCeQuery(query);
            if (success != null && success > 0) return null;
            return "Unknown error encountered!";
        }

        public bool? UpdateConsumerInfo(Consumer consumer)
        {
            if (consumer == null) return null;
            string query = "UPDATE USERS SET Name = '" + consumer.Name + "', Email = '" + consumer.Email + "', birthdate = '" + consumer.Birthdate.DbFormat + "', gender = " + consumer.GenderIndex + " where username = '" + null + "'";
            int? success = this.ExecuteSqlCeQuery(query);
            if (success == null) return null;
            return (success > 0);
        }

        public bool Unfriend(Consumer consumer)
        {
            string query = "DELETE FROM FRIENDSHIPS WHERE (username_1 = '" + null + "' and username_2 = '" + null + "') or (username_1 = '" + null + "' and username_2 = '" + null + "')";
            int? success = this.ExecuteSqlCeQuery(query);
            if (success == null || success == 0) return false;
            return true;
        }

        public bool? IsFriend(Consumer consumer)
        {
            string query = "SELECT username_1 FROM FRIENDSHIPS WHERE (username_1 = '" + null + "' and username_2 = '" + null + "') or (username_1 = '" + null + "' and username_2 = '" + null + "')";
            SqlCeDataReader reader = this.ReadSqlCeData(query);
            if (reader == null) return null;
            return reader.Read();
        }

        public Time FriendAddedTime(Consumer consumer)
        {
            string query = "SELECT adding_date FROM FRIENDSHIPS WHERE (username_1 = '" + null + "' and username_2 = '" + null + "') or (username_1 = '" + null + "' and username_2 = '" + null + "')";
            SqlCeDataReader reader = this.ReadSqlCeData(query);
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
            SqlCeDataReader data = ReadSqlCeData(senderListQuery);
            while(data.Read())
            {
                long foundId = (long)data["sender_id"];
                consumerDictionary[foundId]["frequest_status"] = "r_sender";
            }
            this.CloseConnection();

            data = ReadSqlCeData(receiverListQuery);
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
            SqlCeDataReader data = this.ReadSqlCeData(query);
            List<long> memberIdList = new List<long>();
            while(data.Read())
            {
                memberIdList.Add(long.Parse(data["member_Id"].ToString()));
            }
            return memberIdList;
        }


        public long UserIdByDeviceMac(string deviceMac)
        {
            throw new NotImplementedException();
        }
    }
}