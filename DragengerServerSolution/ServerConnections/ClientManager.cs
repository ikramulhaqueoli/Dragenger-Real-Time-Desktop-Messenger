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
        internal void SendNuntiasToConsumers(Nuntias newNuntias)
        {
            Output.ShowLog("SendNuntiasToConsumers() => " + newNuntias.ToJson());
            BackgroundWorker bworker = new BackgroundWorker();
            bworker.DoWork += (s, e) =>
            {
                try
                {
            //        List<long> nuntiasOwnerIdList = NuntiasRepository.Instance.NuntiasOwnerIdList(newNuntias.Id);
            //        foreach (long userId in nuntiasOwnerIdList)
            //        {
            //            Output.ShowLog(userId);
            //            List<string> connectionIdList = LoggedInConnectionIdList(userId);
            //            foreach (string connectionId in connectionIdList)
            //            {
            //                Output.ShowLog("cm: " + userId + " " + connectionId + " " + connectionIdList.Count);
                            ServerHub.WorkingHubInstance.Clients.All.SendNuntias(newNuntias.ToJson());
            //            }
            //        }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Exception in SendNuntiasToConsumers() => " + ex.Message);
                }
            };
            bworker.RunWorkerAsync();
            bworker.RunWorkerCompleted += (s, e) => { bworker.Dispose(); };
        }

        internal void CastTypingTextToConsumers(long conversationId, string text, string macAddress)
        {
            //List<long> conversationMemberIdList = ConsumerRepository.Instance.ConversationMemberIdList(conversationId);
            //foreach (long userId in conversationMemberIdList)
            //{
            //    List<string> connectionIdList = LoggedInConnectionIdList(userId);
            //    foreach (string connectionId in connectionIdList)
            //    {
            //        if (MacConnectionIdMap[macAddress] == connectionId) continue;
            //        BackgroundWorker bworker = new BackgroundWorker();
            //        bworker.DoWork += (s, e) =>
            //        {
                        ServerHub.WorkingHubInstance.Clients.All.SomethingBeingTypedForYou(conversationId, text);
            //        };
            //        bworker.RunWorkerAsync();
            //        bworker.RunWorkerCompleted += (s, e) => { bworker.Dispose(); };
            //    }
            //}
        }

        public static ClientManager Instance { get { return new ClientManager(); } }
    }
}
