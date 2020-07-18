using LocalRepository;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;
using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerConnections
{
    public class ServerHub
    {
        private HubConnection hubConnection;
        private IHubProxy serverHubProxy;
        public ServerHub()
        {
            ConnectToServer();

            hubConnection.DeadlockErrorTimeout = TimeSpan.FromSeconds(120);
            hubConnection.Closed += new Action(ConnectToServer);
            hubConnection.Error += new Action<Exception>(ReconnectToServer);
        }

        private void ReconnectToServer(Exception exception)
        {
            Console.WriteLine("Connection Error in ServerHub => " + exception.Message + "\n" + exception.StackTrace);
            try { this.StopConnection(); } catch { }
            this.ConnectToServer();
            BackgroundWorker bworker = new BackgroundWorker();
            bworker.DoWork += (s, e) =>
            {
                while (true)
                {
                    try
                    {
                        string serverConnectionAddress = ConfigurationManager.AppSettings["connectionType"] + "://" + ConfigurationManager.AppSettings["serverIp"] + ":" + ConfigurationManager.AppSettings["serverPort"] + "/";
                        hubConnection = new HubConnection(serverConnectionAddress);
                        serverHubProxy = hubConnection.CreateHubProxy("ServerHub");
                        hubConnection.Headers.Add("mac_address", Universal.SystemMACAddress);
                        hubConnection.Start().Wait();
                        return;
                    }
                    catch
                    {
                        Thread.Sleep(50000);
                    }
                }
            };
            bworker.RunWorkerAsync();
            bworker.RunWorkerCompleted += (s,e) => { bworker.Dispose(); }; 
        }

        private void ConnectToServer()
        {
            try
            {
                string serverConnectionAddress = ConfigurationManager.AppSettings["connectionType"] + "://" + ConfigurationManager.AppSettings["serverIp"] + ":" + ConfigurationManager.AppSettings["serverPort"] + "/";
                hubConnection = new HubConnection(serverConnectionAddress);
                serverHubProxy = hubConnection.CreateHubProxy("ServerHub");
                hubConnection.Headers.Add("mac_address", Universal.SystemMACAddress);
                hubConnection.Start().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in connecting: " + ex.Message);
                System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("Connection to server failed", "Error!", System.Windows.Forms.MessageBoxButtons.RetryCancel, System.Windows.Forms.MessageBoxIcon.Error);
                if (result == System.Windows.Forms.DialogResult.Retry)
                {
                    ConnectToServer();
                }
                else
                {
                    System.Windows.Forms.Application.Exit();
                }
            }
        }

        public void StopConnection()
        {
            this.hubConnection.Stop();
        }

        public IHubProxy ServerHubProxy
        {
            get { return serverHubProxy; }
        }

        public static ServerHub WorkingInstance
        {
            set;
            get;
        }
    }
}
