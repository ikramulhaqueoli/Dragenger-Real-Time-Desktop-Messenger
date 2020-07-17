using Display;
using EntityLibrary;
using Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerConnections
{
    public class ClientManager
    {
        internal static Dictionary<long, List<string>> UserIdMacList
        {
            set;
            get;
        }
        internal static Dictionary<string, string> MacConnectionIdMap
        {
            set;
            get;
        }
        internal static Dictionary<string, bool> IsMacLoggedIn
        {
            set;
            get;
        }
        public static void InitializeStaticDictionaries()
        {
            UserIdMacList = new Dictionary<long, List<string>>();
            MacConnectionIdMap = new Dictionary<string, string>();
            IsMacLoggedIn = new Dictionary<string, bool>();
        }
        internal void RegisterConnectionWithConsumerDevice(long userId, string macAddress, string connectionId)
        {
            Output.ShowLog("Registering connection with: " + userId + " " + macAddress);
            if (!UserIdMacList.ContainsKey(userId)) UserIdMacList[userId] = new List<string>();
            if (!IsMacLoggedIn.ContainsKey(macAddress)) IsMacLoggedIn[macAddress] = false;
            if (!UserIdMacList[userId].Contains(macAddress)) UserIdMacList[userId].Add(macAddress);
            MacConnectionIdMap[macAddress] = connectionId;
        }
        internal void DeregisterConnectionWithConsumerDevice(long userId, string macAddress, string connectionId)
        {
            Output.ShowLog("Deregistering connection with: " + userId + " " + macAddress);
            try { IsMacLoggedIn.Remove(macAddress); }
            catch { }
            try { UserIdMacList[userId].Remove(macAddress); }
            catch { }
            try { if (UserIdMacList[userId].Count == 0) UserIdMacList.Remove(userId); }
            catch { }
            try { MacConnectionIdMap.Remove(macAddress); }
            catch { }
        }
        internal User RegisterLoggedInUser(string macAddress, string password)
        {
            User user = UserRepository.Instance.GetByCredentials(macAddress, password);
            if (user != null)
            {
                IsMacLoggedIn[macAddress] = true;
                UserRepository.Instance.SetUserActive(user.Id);
                this.UpdateUsersActivityToFriends(user.Id, "active");
            }
            return user;
        }

        internal List<string> LoggedInConnectionIdList(long userId)
        {
            List<string> connectionIdList = new List<string>();
            if (UserIdMacList.ContainsKey(userId))
            {
                foreach (string macAddress in UserIdMacList[userId])
                {
                    if (IsMacLoggedIn.ContainsKey(macAddress) && IsMacLoggedIn[macAddress])
                    {
                        connectionIdList.Add(MacConnectionIdMap[macAddress]);
                    }
                }
            }
            return connectionIdList;
        }

        //conversation and nuntias requests methods
        internal void SendNuntiasToConsumers(Nuntias newNuntias, string requestingMacAddress)
        {
            Output.ShowLog("SendNuntiasToConsumers() => " + newNuntias.ToJson());
            BackgroundWorker bworker = new BackgroundWorker();
            bworker.DoWork += (s, e) =>
            {
                try
                {
                    List<long> nuntiasOwnerIdList = NuntiasRepository.Instance.NuntiasOwnerIdList(newNuntias.Id);
                    foreach (long userId in nuntiasOwnerIdList)
                    {
                        Output.ShowLog(userId);
                        List<string> connectionIdList = LoggedInConnectionIdList(userId);
                        foreach (string connectionId in connectionIdList)
                        {
                            if (MacConnectionIdMap[requestingMacAddress] == connectionId) continue;
                            Output.ShowLog("cm: " + userId + " " + connectionId);
                            ServerHub.WorkingHubInstance.Clients.Client(connectionId).SendNuntias(newNuntias.ToJson());
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Exception in SendNuntiasToConsumers() => " + ex.Message);
                }
            };
            bworker.RunWorkerAsync();
            bworker.RunWorkerCompleted += (s, e) => { bworker.Dispose(); };
        }

        internal void CastTypingTextToConsumers(long conversationId, string text, long typingUserId)
        {
            Output.ShowLog("CastTypingTextToConsumers() => " + conversationId + " text: " + text);
            BackgroundWorker bworker = new BackgroundWorker();
            bworker.DoWork += (s, e) =>
            {
                
                List<long> nuntiasOwnerIdList = ConsumerRepository.Instance.ConversationMemberIdList(conversationId);
                foreach (long userId in nuntiasOwnerIdList)
                {
                    if (userId == typingUserId) continue;
                    Output.ShowLog(userId);
                    List<string> connectionIdList = LoggedInConnectionIdList(userId);
                    foreach (string connectionId in connectionIdList)
                    {
                        try
                        {
                            Output.ShowLog("cm: " + userId + " " + connectionId);
                            ServerHub.WorkingHubInstance.Clients.Client(connectionId).SomethingBeingTypedForYou(conversationId, text);
                        }
                        catch(Exception ex)
                        {
                            Output.ShowLog("Exception in CastTypingTextToConsumers() => for id:" + userId + " " + ex.Message);
                        }
                    }
                }
            };
            bworker.RunWorkerAsync();
            bworker.RunWorkerCompleted += (s, e) => { bworker.Dispose(); };
        }

        internal void UpdateUsersActivityToFriends(long userId, string activity)
        {
            Output.ShowLog("UpdateUsersActivityToFriends() => " + userId + " activity: " + activity);
            BackgroundWorker bworker = new BackgroundWorker();
            bworker.DoWork += (s, e) =>
            {

                List<Consumer> friendListIdList = ConsumerRepository.Instance.GetFriendListOf(userId, "");
                foreach (Consumer friend in friendListIdList)
                {
                    long friendId = friend.Id;
                    List<string> connectionIdList = LoggedInConnectionIdList(friendId);
                    foreach (string connectionId in connectionIdList)
                    {
                        try
                        {
                            Output.ShowLog("cm: " + friendId + " " + connectionId);
                            ServerHub.WorkingHubInstance.Clients.Client(connectionId).UpdateUsersActivity(userId, activity);
                        }
                        catch (Exception ex)
                        {
                            Output.ShowLog("Exception in UpdateUsersActivityToFriends() => for id:" + friendId + " " + ex.Message);
                        }
                    }
                }
            };
            bworker.RunWorkerAsync();
            bworker.RunWorkerCompleted += (s, e) => { bworker.Dispose(); };
        }

        public static ClientManager Instance { get { return new ClientManager(); } }
    }
}
