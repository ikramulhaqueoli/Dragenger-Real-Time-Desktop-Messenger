using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Repositories;
using EntityLibrary;
using ResourceLibrary;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System;
using Display;
using System.Threading.Tasks;
using System.Drawing;
using FileRepositories;
using System.IO;
using System.ComponentModel;

namespace ServerConnections
{
    [HubName("ServerHub")]
    public class ServerHub : Hub
    {
        public ServerHub()
        {
            ServerHub.WorkingHubInstance = this;
        }
        public static ServerHub WorkingHubInstance
        {
            set;
            get;
        }
        //connection establishing formalities with client
        public override Task OnConnected()
        {
            Output.ShowLog("Connected device: " + Context.Headers["mac_address"] + " cid: " + Context.ConnectionId);
            string macAddress = Context.Headers["mac_address"];
            long? userId = UserRepository.Instance.GetUserIdByMacAddress(macAddress);
            if (userId != null) ClientManager.Instance.RegisterConnectionWithConsumerDevice((long)userId, macAddress, Context.ConnectionId);
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            Output.ShowLog("Disconnected device: " + Context.Headers["mac_address"] + " cid: " + Context.ConnectionId);
            try
            {
                string macAddress = Context.Headers["mac_address"];
                long? userId = UserRepository.Instance.GetUserIdByMacAddress(macAddress);
                if (userId != null) ClientManager.Instance.DeregisterConnectionWithConsumerDevice((long)userId, macAddress, Context.ConnectionId);
                UserRepository.Instance.SetUserLastActiveTimeStamp((long)userId);
                ClientManager.Instance.UpdateUsersActivityToFriends((long)userId, Time.CurrentTime.TimeStampString);
                return base.OnDisconnected(stopCalled);
            }
            catch(Exception ex)
            {
                Output.ShowLog("Exception in OnDisconnected() => " + ex.Message + " " + Context.Headers["mac_address"]);
                return base.OnDisconnected(stopCalled);
            }
        }
        public override Task OnReconnected()
        {
            //Output.ShowLog("Reconnected device: " + Context.Headers["mac_address"] + " cid: " + Context.ConnectionId);
            //string macAddress = Context.Headers["mac_address"];
            //long? userId = UserRepository.Instance.GetUserIdByMacAddress(macAddress);
            //if (userId != null) ClientManager.RegisterConnectionWithConsumerDevice((long)userId, macAddress, Context.ConnectionId);
            return base.OnReconnected();
        }
        public List<double> LoginWithEncryptedData(List<double> encryptedLoginData)          //receives encrypted json string
        {
            try
            {
                JObject credentials = JObject.Parse(MatrixCryptography.Decrypt(encryptedLoginData));
                string macAddress = credentials["mac_address"].ToString();
                string password = credentials["password"].ToString();
                bool isDataSourceFromCookie = bool.Parse(credentials["from_cookie"].ToString());
                Output.ShowLog("LoginWithEncryptedData() => " + macAddress + " Credentials from cookie? " + isDataSourceFromCookie);

                if (isDataSourceFromCookie && Time.TimeDistanceInMinute(new Time(credentials["last_login_time"].ToString()), Time.CurrentTime) > 4320) return DataProcessor.ProcessLoggedInUserData(null);

                User loggedInUser = ClientManager.Instance.RegisterLoggedInUser(macAddress, password);
                return DataProcessor.ProcessLoggedInUserData(loggedInUser);
            }
            catch (Exception ex)
            {
                Output.ShowLog(ex.Message);
                return null;
            }
        }

        public long SignupUser(List<double> encryptedData)          //receives encrypted json string
        {
            //type:consumer";
            //username:example"
            //email:ex@mple.com"
            //name:example"
            //mac_address:E1234567890AB"
            try
            {
                JObject signupDataJson = JObject.Parse(MatrixCryptography.Decrypt(encryptedData));
                Output.ShowLog("SignupUser() => " + signupDataJson.ToString());
                string accountType = signupDataJson["type"].ToString();
                long? insertedId = null;
                User newUser = null;
                if (accountType == "consumer")
                {
                    newUser = new Consumer(signupDataJson["username"].ToString(), signupDataJson["email"].ToString(), signupDataJson["name"].ToString());
                    insertedId = UserRepository.Instance.Insert(newUser);
                    if (insertedId == null) return -1;
                    newUser.Id = (long)insertedId;
                    ConsumerRepository.Instance.Insert((Consumer)newUser);
                }
                if (insertedId != null)
                {
                    UserRepository.Instance.BindMacAddressById((long)insertedId, signupDataJson["mac_address"].ToString());
                }
                return insertedId ?? -1;                //if the insertedId is null, we will return -1
            }
            catch (Exception ex)
            {
                Output.ShowLog(ex.InnerException.Message);
                return -1;
            }
        }

        public int VerifyVerificationCode(JObject verificationDataJson)
        {
            Output.ShowLog("VerifyUserEmail() => " + verificationDataJson);
            string macAddress = verificationDataJson["mac_address"].ToString();
            string verificationCode = verificationDataJson["verification_code"].ToString();
            string purpose = verificationDataJson["purpose"].ToString();
            int status = UserRepository.Instance.VerifyVerificationCode(macAddress, verificationCode, purpose);
            if(status == 3 || status == 4)
            {
                BackgroundWorker bworker = new BackgroundWorker();
                bworker.DoWork += (s, e) =>
                {
                    Consumer consumer = ConsumerRepository.Instance.GetConsumerByMacAddress(macAddress);
                    if (consumer == null) return;
                    string assignedCode = UserRepository.Instance.AssignEmailVerificationCode(consumer.Id, purpose);
                    if (assignedCode != null) MailServices.SendVerificationCodeToEmail(consumer, assignedCode, purpose);
                };
                bworker.RunWorkerAsync();
                bworker.RunWorkerCompleted += (s, e) => { bworker.Dispose(); };
            }
            return status;
        }

        public bool SendVerificationCodeToResetPassword(string deviceMac)
        {
            BackgroundWorker bworker = new BackgroundWorker();
            bworker.DoWork += (s, e) =>
            {
                Consumer consumer = ConsumerRepository.Instance.GetConsumerByMacAddress(deviceMac);
                if (consumer == null) return;
                string assignedCode = UserRepository.Instance.AssignEmailVerificationCode(consumer.Id, "password_reset");
                if (assignedCode != null) MailServices.SendVerificationCodeToEmail(consumer, assignedCode, "password_reset");
            };
            bworker.RunWorkerAsync();
            bworker.RunWorkerCompleted += (s, e) => { bworker.Dispose(); };
            return true;
        }

        public bool UpdateUserInfo(JObject consumerJson)
        {
            Output.ShowLog("UpdateUserInfo() => " + consumerJson);
            bool? success = ConsumerRepository.Instance.Update(new Consumer(consumerJson));
            if (success == null || success == false) return false;
            return true;
        }


        public JObject GetConsumer(long userId)
		{
            Output.ShowLog("GetConsumer() => " + userId);
			Consumer fetchedConsumer = ConsumerRepository.Instance.Get(userId);
            if (fetchedConsumer == null) return null;
            return fetchedConsumer.ToJson();
		}

        public bool BindDeviceWithExistingAccount(List<double> encryptedData)
        {
            JObject deviceBindDataJson = JObject.Parse(MatrixCryptography.Decrypt(encryptedData));
            string macAddress = deviceBindDataJson["mac_address"].ToString();
            string username = deviceBindDataJson["username"].ToString();
            string password = deviceBindDataJson["password"].ToString();
            Output.ShowLog("BindDeviceWithExistingAccount() => mac: " + macAddress + " Username: " + username);
            bool? success = UserRepository.Instance.BindMacAddressByUsername(username, macAddress);
            if (success == null || success == false) return false;
            return true;
        }

        public bool UnbindDeviceFromUserAccount(long userId, string macAddress)
        {
            Output.ShowLog("UnbindDeviceFromUserAccount() => " + userId + " " + macAddress);
            bool? success = UserRepository.Instance.UnbindMacAddressFromUserAccount(userId, macAddress);
            if (success == null || success == false) return false;
            this.OnDisconnected(true);
            return true;
        }
        public int UserVerifiedOrPasswordIsSet(string deviceMac)
        {
            try
            {
                Output.ShowLog("UserVerifiedOrPasswordIsSet() => " + deviceMac);
                Consumer consumer = ConsumerRepository.Instance.GetConsumerByMacAddress(deviceMac);
                if (consumer == null) return -1;
                int? status = UserRepository.Instance.UserVerifiedOrPasswordIsSet(consumer.Id);
                if(status == 0)                     //0 means user's email is not verified
                {
                    BackgroundWorker bworker = new BackgroundWorker();
                    bworker.DoWork += (s, e) =>
                    {
                        string assignedCode = UserRepository.Instance.AssignEmailVerificationCode(consumer.Id, "email_verify");
                        if(assignedCode != null) MailServices.SendVerificationCodeToEmail(consumer, assignedCode, "email_verify");
                    };
                    bworker.RunWorkerAsync();
                    bworker.RunWorkerCompleted += (s, e) => { bworker.Dispose(); };
                }
                return status ?? -1;
            }
            catch(Exception ex)
            {
                Output.ShowLog(ex.Message);
                return -1;
            }
        }
        public bool MacAddressExists(string deviceMac)
        {
            Output.ShowLog("MacAddressExists() => " + deviceMac);
            if (UserRepository.Instance.MacAddressExists(deviceMac) != true) return false;
            return true;
        }

        public bool UsernameAlreadyExists(string username)
        {
            Output.ShowLog("UsernameAlreadyExists() => " + username);
            if (UserRepository.Instance.UserNameAlreadyExists(username) != true) return false;
            return true;
        }

        public bool EmailAlreadyExists(string email)
        {
            Output.ShowLog("EmailAlreadyExists() => " + email);
            if (ConsumerRepository.Instance.EmailAlreadyExists(email) != true) return false;
            return true;
        }

        public bool PasswordMatches(long userId, string password)
        {
            Output.ShowLog("PasswordMatches() => " + userId);
            if (UserRepository.Instance.GetPassword(userId) != password) return false;
            return true;
        }

        public bool SetPassword(string deviceMac, string newPassword)
        {
            Output.ShowLog("SetPassword() => " + deviceMac);
            string error = UserRepository.Instance.SetPassword(deviceMac, newPassword);
            if (error == null) return true;
            Output.ShowLog("Error = " + error);
            return false;
        }

        public string UserLastActiveTime(long userId)
        {
            return UserRepository.Instance.UserLastActiveTime(userId);
        }

        public bool DeleteNuntias(JObject deleteRequestJson)
        {
            Output.ShowLog("DeleteNuntias() => " + deleteRequestJson);
            long userId = (long)deleteRequestJson["owner_id"];
            long nuntiasId = (long)deleteRequestJson["nuntias_id"];
            bool forBoth = (bool)deleteRequestJson["for_both"];
            string requestingMacAddress = Context.Headers["mac_address"].ToString();

            bool success = NuntiasRepository.Instance.DeleteNuntias(userId, nuntiasId, forBoth);
            Output.ShowLog(success + " " + forBoth);
            if (success && forBoth)
            {
                ClientManager.Instance.SendNuntiasToConsumers(NuntiasRepository.Instance.Get(nuntiasId), requestingMacAddress);
            }
            return success;
        }

        public List<JObject> SearchTop20PersonsByKeyword(long userId, string keyword)
        {
            Output.ShowLog("SearchTop20PersonsByKeyword() => " + userId + " " + keyword);
            List<Consumer> metchedList = ConsumerRepository.Instance.SearchNonfriendPersonsByKeyword(userId, keyword);
            List<JObject> personsJsonDataList = new List<JObject>();
            foreach (Consumer consumer in metchedList)
            {
                JObject personJson = consumer.ToJson();
                Output.ShowLog(personJson);
                personsJsonDataList.Add(personJson);
            }
            ConsumerRepository.Instance.SetFriendRequestStatusOfPersonList(userId, personsJsonDataList);
            return personsJsonDataList;
        }

        public List<JObject> GetConversations(List<long> conversationIdList)
        {
            List<JObject> conversationJsonList = ConversationRepository.Instance.GetConversations(conversationIdList);
            if (conversationJsonList == null) conversationJsonList = new List<JObject>();
            return conversationJsonList;
        }

        public List<JObject> GetTop20FriendListOf(long userId, string keyword)
        {
            Output.ShowLog("GetTop20FriendListOf() => " + userId);
            List<Consumer> metchedList = ConsumerRepository.Instance.GetFriendListOf(userId, keyword);
            List<JObject> personsJsonDataList = new List<JObject>();
            foreach (Consumer consumer in metchedList)
            {
                JObject personJson = consumer.ToJson();
                Output.ShowLog(personJson);
                personsJsonDataList.Add(personJson);
            }
            return personsJsonDataList;
        }

        public List<JObject> GetFriendRequestListOf(long userId, string keyword)
        {
            Output.ShowLog("GetFriendRequestListOf() => " + userId);
            List<Consumer> metchedList = ConsumerRepository.Instance.GetFriendRequestListOf(userId, keyword);
            List<JObject> personsJsonDataList = new List<JObject>();
            foreach (Consumer consumer in metchedList)
            {
                JObject personJson = consumer.ToJson();
                Output.ShowLog(personJson);
                personsJsonDataList.Add(personJson);
            }
            ConsumerRepository.Instance.SetFriendRequestStatusOfPersonList(userId, personsJsonDataList);
            return personsJsonDataList;
        }

        public bool SendFriendRequest(long senderId, long receiverId)
        {
            Output.ShowLog("SendFriendRequest() => " + senderId + " => " + receiverId);
            return ConsumerRepository.Instance.SetFriendRequest(senderId, receiverId);
        }

        public bool DeleteFriendRequest(long userId1, long userId2)
        {
            Output.ShowLog("DeleteFriendRequest() => " + userId1 + " => " + userId2);
            return ConsumerRepository.Instance.DeleteFriendRequest(userId1, userId2);
        }

        public bool AcceptFriendRequest(long userId1, long userId2)
        {
            Output.ShowLog("AcceptFriendRequest() => " + userId1 + " => " + userId2);
            return ConsumerRepository.Instance.AcceptFriendRequest(userId1, userId2);
        }

        public long GetDuetConversationId(long memberId1, long memberId2)
        {
            Output.ShowLog("GetDuetConversationId() => " + memberId1 + " => " + memberId2);
            long? fetchedConversationId = DuetConversationRepository.Instance.GetDuetConversationId(memberId1, memberId2);
            if (fetchedConversationId != null) return (long)fetchedConversationId;
            else
            {
                Conversation newConversation = new DuetConversation(new Consumer(memberId1), new Consumer(memberId2));
                long? newConversationId = DuetConversationRepository.Instance.Insert(newConversation);
                return newConversationId ?? 0;
            }
        }

        public List<JObject> GetAllNuntiasOf(JObject requestData)
        {
            long userId = long.Parse(requestData["user_id"] + "");
            long lastLocalNuntiasId = long.Parse(requestData["last_local_nuntiasId"] + "");
            string senderMacAddress = requestData["sender_mac_address"] + "";
            Output.ShowLog("GetAllNuntiasOf() => " + userId + " => " + lastLocalNuntiasId);
            List<Nuntias> fetchedNuntiasList = NuntiasRepository.Instance.GetNuntiasListOfUser(userId, lastLocalNuntiasId);
            if (fetchedNuntiasList == null) return null;
            List<JObject> fetchedNuntiasJsonList = new List<JObject>();
            foreach (Nuntias nuntias in fetchedNuntiasList)
            {
                fetchedNuntiasJsonList.Add(nuntias.ToJson());
                Output.ShowLog(nuntias.ToJson());
            }
            return fetchedNuntiasJsonList;
        }

        public long SendNewNuntias(JObject nuntiasJsonData)
        {
            Output.ShowLog("SendNewNuntias() => " + nuntiasJsonData);
            Nuntias newNuntias = new Nuntias(nuntiasJsonData);
            long? nuntiasId = NuntiasRepository.Instance.Insert(newNuntias);
            newNuntias.Id = nuntiasId ?? 0;
            if (newNuntias.Id > 0)
            {
                Output.ShowLog("Called " + newNuntias.Id);
                ClientManager.Instance.SendNuntiasToConsumers(newNuntias, Context.Headers["mac_address"]);
            }
            return newNuntias.Id;
        }

        public byte[] GetNuntiasContentFile(long nuntiasId)
        {
            string contentPath = ContentFileAccess.GetNuntiasContentFilePath(nuntiasId);
            return (contentPath == null) ? null : File.ReadAllBytes(contentPath);
        }

        public bool UpdateNuntiasStatusToOwners(JObject nuntiasJsonData)
        {
            Output.ShowLog("UpdateNuntiasStatusToOwners() => " + nuntiasJsonData);
            Nuntias newNuntias = new Nuntias(nuntiasJsonData);
            bool? success = NuntiasRepository.Instance.UpdateNuntiasStatusTimes(newNuntias, (long)nuntiasJsonData["user_id"]);
            ClientManager.Instance.SendNuntiasToConsumers(newNuntias, Context.Headers["mac_address"]);
            if (success == true) return true;
            return false;
        }

        public long SendContentedNuntias(KeyValuePair<JObject, byte[]> nuntiasData)     //key contains the JsonObject, Value contains the file bytes
        {
            Output.ShowLog("SendContentedNuntias() => " + nuntiasData.Key + nuntiasData.Value.Length);
            Nuntias newNuntias = new Nuntias(nuntiasData.Key);
            long? nuntiasId = NuntiasRepository.Instance.Insert(newNuntias);
            newNuntias.Id = nuntiasId ?? 0;
            if (newNuntias.Id > 0)
            {
                Output.ShowLog("Called " + newNuntias.Id);
                ContentFileAccess.StoreNuntiasContentFile(nuntiasData.Value, newNuntias.Id);
                ClientManager.Instance.SendNuntiasToConsumers(newNuntias, Context.Headers["mac_address"]);
            }
            return newNuntias.Id;
        }

        public JObject ChangeProfileImage(long userId, byte[] imageArray)
        {
            Output.ShowLog("ChangeProfileImage() => " + userId);
            Image img = new Bitmap(GraphicsStudio.ClipToCircle(Universal.ByteArrayToImage(imageArray)), new Size(200, 200));
            string newProfileImgId = "DP_" + userId + "_" + Time.CurrentTime.TimeStampString;
            string oldProfileImgId = ConsumerRepository.Instance.ReassignConsumerProfileImgId(userId, newProfileImgId);
            if (oldProfileImgId != null && oldProfileImgId.Length >= 5) ImageFileAccess.EraseProfileImage(oldProfileImgId);
            JObject profileImgIdJson = new JObject();
            profileImgIdJson["old_image_id"] = null;
            profileImgIdJson["new_image_id"] = null;
            if(ImageFileAccess.SaveProfileImage(img, newProfileImgId))
            {
                profileImgIdJson["old_image_id"] = oldProfileImgId;
                profileImgIdJson["new_image_id"] = newProfileImgId;
                Output.ShowLog(profileImgIdJson);
            }
            return profileImgIdJson;
        }

        public byte[] GetProfileImageByProfileImgId(string profileImgId)
        {
            return File.ReadAllBytes(ImageFileAccess.ProfileImagePath(profileImgId));
        }

        public string DeleteConsumerAccount(string macAddress, string password)
        {
            Output.ShowLog("DeleteConsumerAccount() => " + macAddress);
            return ConsumerRepository.Instance.DeleteConsumerAccount(macAddress, password);
        }

        public bool SomethingTypingOnConversationFor(long conversationId, long typingUserId, string text)
        {
            Output.ShowLog("SomethingTypingOnConversationFor() => " + conversationId + " " + typingUserId + " " + text);
            try
            {
                ClientManager.Instance.CastTypingTextToConsumers(conversationId, text, typingUserId);
                return true;
            }
            catch(Exception ex)
            {
                Output.ShowLog("Error in SomethingTypingOnConversationFor() => " + ex.Message + " " + ex.InnerException.Message);
                return false;
            }
        }

        //testing APIs
        public bool ReceiveFile(byte[] imageArray)
		{
			Image img = Universal.ByteArrayToImage(imageArray);
			Output.ShowImage(img);
			return true;
		}
    }
}