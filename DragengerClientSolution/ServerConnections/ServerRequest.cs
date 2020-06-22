using EntityLibrary;
using LocalRepository;
using MathLaboratory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.SqlServerCe;
using ResourceLibrary;
using FileIOAccess;
using System.ComponentModel;

namespace ServerConnections
{
    public class ServerRequest
    {
        public static bool? LoginWithCredentials(string macAddress, string password)
        {
            JObject credentialsJson = new JObject();
            credentialsJson["mac_address"] = macAddress;
            credentialsJson["password"] = password;
            credentialsJson["from_cookie"] = false;
            return SetLoggedInUser(credentialsJson);
        }

        public static bool? LoginWithCookie(JObject cookieJson)
        {
            cookieJson["from_cookie"] = true;
            return SetLoggedInUser(cookieJson);
        }

        public static bool? SetLoggedInUser(JObject loginDataJson)
        {
            List<double> encryptedCookieData = MatrixCryptography.Encrypt(loginDataJson.ToString());
            List<double> encryptedFetchedUserData = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<List<double>>("LoginWithEncryptedData", encryptedCookieData).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    encryptedFetchedUserData = task.Result;
                }
                else Console.WriteLine(task.Exception.InnerException.Message);
            }).Wait();
            if (encryptedFetchedUserData == null) return null;
            JObject fetchedUserDataJson = JObject.Parse(MatrixCryptography.Decrypt(encryptedFetchedUserData));
            User loggedInUser = null;
            if (bool.Parse(fetchedUserDataJson["found"].ToString()) == true)
            {
                string type = fetchedUserDataJson["type"].ToString();
                if (type == "consumer")
                {
                    Consumer.LoggedIn = new Consumer(fetchedUserDataJson);
                    loggedInUser = Consumer.LoggedIn;
                    if (fetchedUserDataJson.ContainsKey("profile_img_id") && fetchedUserDataJson["profile_img_id"].ToString().Length >= 5)
                    {
                        ServerFileRequest.RefetchProfileImage(fetchedUserDataJson["profile_img_id"].ToString());
                        Consumer.LoggedIn.SetProfileImage(fetchedUserDataJson["profile_img_id"].ToString());
                    }
                }
            }
            else return false;
            User.LoggedIn = loggedInUser;
            return true;
        }

        public static bool? MacAddressExists(string macAddress)             //requests server to check if the mac address exists or not
        {
            bool? exists = false;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("MacAddressExists", macAddress).ContinueWith(task =>
            {
                if (!task.IsFaulted) exists = task.Result;
                else exists = null;
            }).Wait();
            return exists;
        }

        public static bool? UsernameAlreadyExists(string username)             //requests server to check if the username exists or not while signup
        {
            bool? exists = false;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("UsernameAlreadyExists", username).ContinueWith(task =>
            {
                if (!task.IsFaulted) exists = task.Result;
                else
                {
                    Console.WriteLine(task.Exception.Message);
                    exists = null;
                }
            }).Wait();
            return exists;
        }

        public static bool? EmailAlreadyExists(string email)             //requests server to check if the email exists or not while signup
        {
            bool? exists = false;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("EmailAlreadyExists", email).ContinueWith(task =>
            {
                if (!task.IsFaulted) exists = task.Result;
                else exists = null;
            }).Wait();
            return exists;
        }

        public static bool? PasswordMatches(long userId, string inputPassword)             //requests server to check if the userId matches with password or not while signup
        {
            bool? exists = false;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("PasswordMatches", userId, inputPassword).ContinueWith(task =>
            {
                if (!task.IsFaulted) exists = task.Result;
                else exists = null;
            }).Wait();
            return exists;
        }

        public static bool DeleteNuntias(long ownerId, long nuntiasId, bool forBoth)
        {
            if(nuntiasId <= 0)
            {
                NuntiasRepository.Instance.DeleteTmpNuntias(nuntiasId);
                return true;
            }
            JObject nuntiasDeleteRequestJson = new JObject();
            nuntiasDeleteRequestJson["owner_id"] = ownerId;
            nuntiasDeleteRequestJson["nuntias_id"] = nuntiasId;
            nuntiasDeleteRequestJson["for_both"] = forBoth;

            bool? success = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("DeleteNuntias", nuntiasDeleteRequestJson).ContinueWith(task =>
            {
                if (!task.IsFaulted) success = task.Result;
            }).Wait();

            if (success == null)
            {
                Universal.ShowErrorMessage("Server connection failed", "Message deletion failed");
                return false;
            }

            if(!forBoth)
            {
                NuntiasRepository.Instance.Delete(nuntiasId);
            }
            return true;
        }

        public static long? SignupUser(JObject signupDataJson)
        {
            List<double> encryptedData = MatrixCryptography.Encrypt(signupDataJson.ToString());
            long? userId = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<long>("SignupUser", encryptedData).ContinueWith(task =>
            {
                if (!task.IsFaulted) userId = task.Result;
                else userId = null;
            }).Wait();
            return userId;
        }

        public static Time ReceiverLastActiveTime(long userId)
        {
            string lastActiveStamp = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<string>("UserLastActiveTime", userId).ContinueWith(task =>
            {
                try
                {
                    lastActiveStamp = task.Result;
                }
                catch (Exception ex) { Console.WriteLine("Exception in ReceiverLastActiveTime() => " + ex.Message); }
            }).Wait();
            if (lastActiveStamp == null || lastActiveStamp.Length == 0) return null;
            //Console.WriteLine(lastActiveStamp);
            if (lastActiveStamp == "active") return Time.CurrentTime;
            return new Time(lastActiveStamp);
        }

        public static bool? BindDeviceAndLogin(string macAddress, string username, string password)
        {
            JObject deviceBindDataJson = new JObject();
            deviceBindDataJson["mac_address"] = macAddress;
            deviceBindDataJson["username"] = username;
            deviceBindDataJson["password"] = password;
            List<double> encryptedData = MatrixCryptography.Encrypt(deviceBindDataJson.ToString());
            bool? success = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("BindDeviceWithExistingAccount", encryptedData).ContinueWith(task =>
            {
                if (!task.IsFaulted) success = task.Result;
                else success = null;
            }).Wait();
            if (success == true)
            {
                deviceBindDataJson["from_cookie"] = false;
                return SetLoggedInUser(deviceBindDataJson);
            }
            return success;
        }

        public static int? UserVerifiedOrPasswordIsSet(string systemMACAddress)
        {
            int? status = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<int>("UserVerifiedOrPasswordIsSet", systemMACAddress).ContinueWith(task =>
            {
                if (!task.IsFaulted) status = task.Result;
            }).Wait();
            return status;
        }

        public static string SetPassword(string systemMACAddress, string newPassword)
        {
            bool? status = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("SetPassword", systemMACAddress, newPassword).ContinueWith(task =>
            {
                if (!task.IsFaulted) status = task.Result;
            }).Wait();
            if (status == null) return "Server connection failed!";
            else if (status == false) return "Failed to change password";
            return null;
        }
        public static List<JObject> SearchTop20PersonsByKeyword(long userId, string keyword)
        {
            List<JObject> fetchedUserListJson = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<List<JObject>>("SearchTop20PersonsByKeyword", userId, keyword).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    fetchedUserListJson = task.Result;
                }
            }).Wait();
            if (fetchedUserListJson == null) return new List<JObject>();
            foreach (JObject consumerJson in fetchedUserListJson)
            {
                try { ServerFileRequest.RefetchProfileImage(consumerJson["profile_img_id"].ToString()); } catch { }
            }
            return fetchedUserListJson;
        }

        public static List<Consumer> GetTop20FriendListOf(long userId, string keyword)
        {
            List<JObject> fetchedFriendListJson = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<List<JObject>>("GetTop20FriendListOf", userId, keyword).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    fetchedFriendListJson = task.Result;
                }
            }).Wait();
            List<Consumer> fetchedFriendList = new List<Consumer>();
            if (fetchedFriendListJson == null) return fetchedFriendList;
            foreach (JObject consumerJson in fetchedFriendListJson)
            {
                Consumer consumer = new Consumer(consumerJson);
                ServerFileRequest.RefetchProfileImage(consumer.ProfileImageId);
                fetchedFriendList.Add(consumer);
            }
            return fetchedFriendList;
        }

        public static List<JObject> GetFriendRequestsByKeyword(long userId, string keyword)
        {
            List<JObject> fetchedFriendListJson = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<List<JObject>>("GetFriendRequestListOf", userId, keyword).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    fetchedFriendListJson = task.Result;
                }
            }).Wait();
            foreach (JObject consumerJson in fetchedFriendListJson)
            {
                try { ServerFileRequest.RefetchProfileImage(consumerJson["profile_img_id"].ToString()); } catch { }
            }
            return fetchedFriendListJson;
        }

        public static int? VerifyVerificationCode(string inputCode, string purpose)
        {
            int? status = null;
            JObject verificationDataJson = new JObject();
            verificationDataJson["mac_address"] = Universal.SystemMACAddress;
            verificationDataJson["verification_code"] = inputCode;
            verificationDataJson["purpose"] = purpose;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<int>("VerifyVerificationCode", verificationDataJson).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    status = task.Result;
                }
            }).Wait();
            Console.WriteLine(status);
            return status;
        }

        public static bool? SendVerificationCodeToResetPassword()
        {
            bool? success = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("SendVerificationCodeToResetPassword", Universal.SystemMACAddress).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    success = task.Result;
                }
            }).Wait();
            return success;
        }

        public static bool? SendFriendRequest(long senderId, long receiverId)
        {
            bool? success = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("SendFriendRequest", senderId, receiverId).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    success = task.Result;
                }
            }).Wait();
            return success;
        }

        public static bool? DeleteFriendRequest(long userId1, long userId2)
        {
            bool? success = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("DeleteFriendRequest", userId1, userId2).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    success = task.Result;
                }
            }).Wait();
            return success;
        }

        public static bool? AcceptFriendRequest(long senderId, long receiverId)
        {
            bool? success = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("AcceptFriendRequest", senderId, receiverId).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    success = task.Result;
                }
            }).Wait();
            return success;
        }

        public static List<Consumer> SearchFriendsByKeyword(long userId, string keyword)
        {
            return new List<Consumer>();
        }

        public static long? GetDuetConversationId(User user, Consumer consumer)
        {
            long? conversationId = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<long>("GetDuetConversationId", user.Id, consumer.Id).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    conversationId = task.Result;
                }
            }).Wait();
            return conversationId;
        }

        public static bool SyncUncachedConversations(List<Nuntias> nuntiasList)
        {
            //Console.WriteLine("SyncUncachedConversations()");
            HashSet<long> conversationIdSet = new HashSet<long>();
            foreach (Nuntias nuntias in nuntiasList)
            {
                conversationIdSet.Add(nuntias.NativeConversationID);
            }
            List<long> unlistedConversationIdList = new List<long>();
            foreach (long conversationId in conversationIdSet)
            {
                bool? exists = ConversationRepository.Instance.ExistsConversation(conversationId);
                if (exists == false)
                {
                    unlistedConversationIdList.Add(conversationId);
                }
            }
            if (unlistedConversationIdList.Count == 0) return true;
            List<JObject> conversationJsonList = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<List<JObject>>("GetConversations", unlistedConversationIdList).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    conversationJsonList = task.Result;
                }
            }).Wait();
            if (conversationJsonList == null) return false;
            foreach (JObject conversationJson in conversationJsonList)
            {
                long id = (long)conversationJson["id"];
                string type = (string)conversationJson["type"];
                Conversation conversation = null;
                if (type == "duet")
                {
                    Consumer member1 = ServerRequest.GetConsumer((long)conversationJson["member_id_1"]);
                    Consumer member2 = ServerRequest.GetConsumer((long)conversationJson["member_id_2"]);
                    if (member1 == null || member2 == null) continue;
                    conversation = new DuetConversation(id, member1, member2);
                    DuetConversationRepository.Instance.Insert(conversation);
                }
                else if (type == "group")
                {
                    List<Consumer> memberList = new List<Consumer>();
                    int member_count = (int)conversationJson["member_counter"];
                    for (int i = 1; i <= member_count; i++)
                    {
                        memberList.Add(ServerRequest.GetConsumer((long)conversationJson["member_id_" + i]));
                    }
                    conversation = new GroupConversation(id, conversationJson["name"].ToString(), memberList);
                    GroupConversationRepository.Instance.Insert(conversation);
                }
            }
            return true;
        }

        public static Consumer GetConsumer(long id)
        {
            ServerRequest.SyncConsumer(id);
            return ConsumerRepository.Instance.Get(id);
        }

        public static bool SyncConsumer(long id)
        {
            JObject consumerJson = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<JObject>("GetConsumer", id).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    consumerJson = task.Result;
                }
            }).Wait();
            Consumer consumer = new Consumer(id, consumerJson["username"] + "", consumerJson["email"] + "", consumerJson["name"] + "");
            try { consumer.ProfileImageId = consumerJson["profile_img_id"].ToString(); }
            catch { }
            ServerFileRequest.RefetchProfileImage(consumer.ProfileImageId);
            bool? success = ConsumerRepository.Instance.Update(consumer);
            if (success == null || success == false) return false;
            return true;
        }

        public static bool SyncUncachedData()
        {
            long? lastLocalNuntiasId = NuntiasRepository.Instance.LastLocalNuntiasId;
            JObject requestData = new JObject();
            requestData["user_id"] = Consumer.LoggedIn.Id;
            requestData["last_local_nuntiasId"] = lastLocalNuntiasId;
            requestData["sender_mac_address"] = Universal.SystemMACAddress;
            List<JObject> nuntiasJsonList = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<List<JObject>>("GetAllNuntiasOf", requestData).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    nuntiasJsonList = task.Result;
                }
            }).Wait();
            if (nuntiasJsonList == null) return false;
            List<Nuntias> nuntiasList = new List<Nuntias>();
            foreach (JObject nuntiasJson in nuntiasJsonList)
            {
                nuntiasList.Add(new Nuntias(nuntiasJson) { DeliveryTime = Time.CurrentTime });
            }
            if (!ServerRequest.SyncUncachedConversations(nuntiasList)) return false;
            foreach (Nuntias nuntias in nuntiasList)
            {
                NuntiasRepository.Instance.Insert(nuntias);
                ServerRequest.UpdateNuntiasStatus(nuntias);
            }
            return true;
        }

        public static bool? UpdateUserInfo(Consumer consumer)
        {
            bool? operationSuccess = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("UpdateUserInfo", consumer.ToJson()).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    operationSuccess = task.Result;
                }
            }).Wait();
            return operationSuccess;
        }

        public static string DeleteConsumerAccount(string macAdress, string password)
        {
            string operationResult = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<string>("DeleteConsumerAccount", macAdress, password).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    operationResult = task.Result;
                }
            }).Wait();
            return operationResult;
        }

        public static long? SendNewNuntias(Nuntias newNuntias)
        {
            JObject nuntiasJsonData = newNuntias.ToJson();
            nuntiasJsonData["sender_mac_address"] = Universal.SystemMACAddress;
            long? nuntiasId = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<long>("SendNewNuntias", nuntiasJsonData).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    nuntiasId = task.Result;
                }
            }).Wait();
            return nuntiasId;
        }

        public static void UpdateNuntiasStatus(Nuntias newNuntias)
        {
            JObject nuntiasJsonData = newNuntias.ToJson();
            nuntiasJsonData["user_id"] = Consumer.LoggedIn.Id;
            bool? success = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("UpdateNuntiasStatusToOwners", nuntiasJsonData).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    success = task.Result;
                }
            }).Wait();
        }

        private static bool SomethingTypingOnConversationForBusy = false;
        public static void SomethingTypingOnConversationFor(long conversationId, string text)
        {
            if (SomethingTypingOnConversationForBusy) return;
            SomethingTypingOnConversationForBusy = true;
            Console.WriteLine("hello");
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("SomethingTypingOnConversationFor", conversationId, text).ContinueWith(task =>
            {
                
            }).Wait();
            SomethingTypingOnConversationForBusy = false;
        }

        public static bool? UnbindDeviceFromAccount()
        {
            bool? success = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("UnbindDeviceFromUserAccount", User.LoggedIn.Id, Universal.SystemMACAddress).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    success = task.Result;
                }
            }).Wait();
            return success;
        }
    }
}
