using CorePanels;
using EntityLibrary;
using LocalRepository;
using Newtonsoft.Json.Linq;
using ServerConnections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using FileIOAccess;
using System.ComponentModel;
using ResourceLibrary;

namespace ProcessManagement
{
    public class ConnectionManager
    {
        public static bool EstablishConnection()
        {
            ServerHub.WorkingInstance = new ServerHub();
            ConnectionManager.DefineServerToClientMethods();
            //test
            {
                
            }
            return true;
        }

        public static void DefineServerToClientMethods()
        {
            ServerHub.WorkingInstance.ServerHubProxy.On<JObject>("SendNuntias", (nuntiasJson) =>
            {
                //Console.WriteLine(nuntiasJson);

                Nuntias newNuntias = new Nuntias(nuntiasJson);
                if (newNuntias != null)
                {
                    Console.WriteLine("SendNuntias() Called by server. " + newNuntias.Id + " " + newNuntias.ContentFileId);
                    bool updatedNuntias = false;
                    if (newNuntias.SenderId != Consumer.LoggedIn.Id && newNuntias.DeliveryTime == null)
                    {
                        newNuntias.DeliveryTime = Time.CurrentTime;
                        updatedNuntias = true;
                    }
                    if (newNuntias.ContentFileId != null && newNuntias.ContentFileId.Length > 0)
                    {
                        if (newNuntias.ContentFileId == "deleted")
                        {
                            try
                            {
                                LocalDataFileAccess.EraseContentFile(SyncAssets.NuntiasSortedList[newNuntias.Id].ContentFileId);
                            }
                            catch { }
                        }
                        else ServerFileRequest.DownloadAndStoreContentFile(newNuntias);
                    }
                    bool? updateResult = NuntiasRepository.Instance.Update(newNuntias);
                    if (updateResult == false)
                    {
                        long? result = NuntiasRepository.Instance.Insert(newNuntias);
                        if (result == null) return;
                        try { if (ConversationPanel.CurrentDisplayedConversationPanel.TheConversation.ConversationID == newNuntias.NativeConversationID) ConversationPanel.CurrentDisplayedConversationPanel.ShowNuntias(newNuntias, true); }
                        catch { }
                    }
                    else if (updateResult == true)
                    {
                        SyncAssets.UpdateNuntias(newNuntias);
                    }
                    if (updatedNuntias) ServerConnections.ServerRequest.UpdateNuntiasStatus(newNuntias);
                }
            }
                );

            ServerHub.WorkingInstance.ServerHubProxy.On("SendPendingNuntias", () =>
                    {
                        Console.WriteLine("SendPendingNuntias() Called by server. ");
                        BackendManager.SendPendingNuntii();
                    }
                );

            ServerHub.WorkingInstance.ServerHubProxy.On<long,string>("SomethingBeingTypedForYou", (conversationId, messageText) =>
                    {
                        try
                        {
                            if (ConversationPanel.CurrentDisplayedConversationPanel.TheConversation.ConversationID == conversationId)
                            {
                                if (Universal.ParentForm.InvokeRequired) Universal.ParentForm.Invoke(new Action(() => 
                                {
                                    ConversationPanel.CurrentDisplayedConversationPanel.SomethingBeingTypedLabel.Text = messageText;
                                    ConversationPanel.CurrentDisplayedConversationPanel.AdjustTypingBarSize();
                                }));

                                else
                                {
                                    ConversationPanel.CurrentDisplayedConversationPanel.SomethingBeingTypedLabel.Text = messageText;
                                    ConversationPanel.CurrentDisplayedConversationPanel.AdjustTypingBarSize();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception: " + ex.Message);
                        }
                    }
                );
        }
    }
}
