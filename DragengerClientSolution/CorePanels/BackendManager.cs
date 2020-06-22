using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ResourceLibrary;
using EntityLibrary;
using StandardAssuranceLibrary;
using FileIOAccess;
using System.Threading;
using ServerConnections;
using Newtonsoft.Json.Linq;
using LocalRepository;
using Microsoft.VisualBasic;

namespace CorePanels
{
    public static class BackendManager
    {
        internal static string Password
        {
            set;
            get;
        }
        public static bool KeepRefreshingRunning { get; set; }

        public static void LoginProcessRun()                                                    //works as expected
        {
            bool? macExists = ServerRequest.MacAddressExists(Universal.SystemMACAddress);
            if (macExists == false)
            {
                if (Universal.ParentForm.InvokeRequired) Universal.ParentForm.Invoke(new MethodInvoker(delegate
                {
                    Universal.ParentForm.Controls.Add(new SignupPanel(Universal.ParentForm));
                    SplashScreen.Instance.Hide();
                }));
                else
                {
                    Universal.ParentForm.Controls.Add(new SignupPanel(Universal.ParentForm));
                    SplashScreen.Instance.Hide();
                }
                return;
            }
            else if (macExists == null)
            {
                if (Universal.ParentForm.InvokeRequired) Universal.ParentForm.Invoke(new MethodInvoker(delegate
                {
                    MessageBox.Show(Universal.ParentForm, "Server connection failed", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
                else MessageBox.Show(Universal.ParentForm, "Server connection failed", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            JObject localCookieJson = Checker.ValidLoginCookieData(Universal.SystemMACAddress);
            if (localCookieJson != null && (Time.TimeDistanceInMinute(new Time(localCookieJson["last_login_time"].ToString()), Time.CurrentTime) > 4320 || localCookieJson["mac_address"].ToString() != Universal.SystemMACAddress))
            {
                LocalDataFileAccess.EraseCurrentLoginCookie();
                localCookieJson = null;
            }

            if (localCookieJson == null)
            {
                int? userVerifiedOrPasswordIsSet = ServerRequest.UserVerifiedOrPasswordIsSet(Universal.SystemMACAddress);
                if(userVerifiedOrPasswordIsSet == 0)            //0 means user is not verified
                {
                    if (Universal.ParentForm.InvokeRequired) Universal.ParentForm.Invoke(new MethodInvoker(ShowEmailVerificationInputPanel));
                    else ShowEmailVerificationInputPanel();
                }
                else if (userVerifiedOrPasswordIsSet == 1)          //1 means user verified but password is not set
                {
                    if (Universal.ParentForm.InvokeRequired) Universal.ParentForm.Invoke(new MethodInvoker(ShowPasswordSetupPanel));
                    else ShowPasswordSetupPanel();
                }
                else if (userVerifiedOrPasswordIsSet == 2)           //2 means user is verified and password is set
                {
                    if (Universal.ParentForm.InvokeRequired) Universal.ParentForm.Invoke(new MethodInvoker(ShowPasswordEnterPanel));
                    else ShowPasswordEnterPanel();
                }
                else if(userVerifiedOrPasswordIsSet == null) Universal.ShowErrorMessage("Server connection failed!");
            }
            else
            {
                bool? loginSuccess = ServerRequest.LoginWithCookie(localCookieJson);
                if (loginSuccess == true)
                {
                    BackendManager.Password = localCookieJson["password"].ToString();
                    BackendManager.SaveLoginCookie();
                    BackendManager.LoginNow(User.LoggedIn);
                    SplashScreen.Instance.Hide();
                }
                else if(loginSuccess == false)
                {
                    LocalDataFileAccess.EraseCurrentLoginCookie();
                    LoginProcessRun();
                }
                else
                {
                    MessageBox.Show("Server connection failed!");
                }
            }
        }

        public static void SaveLoginCookie()
        {
            JObject credentialsJson = new JObject();
            credentialsJson["mac_address"] = Universal.SystemMACAddress;
            credentialsJson["password"] = BackendManager.Password;
            credentialsJson["last_login_time"] = Time.CurrentTime.TimeStampString;
            LocalDataFileAccess.CreateNewLoginCoockie(MathLaboratory.MatrixCryptography.Encrypt(credentialsJson.ToString()));
        }

        internal static void LoginNow(User user)                                            //works as expected
        {
            DatabaseAccess.LocalDbPassword = BackendManager.GenerateLocalDbPassword();
            DatabaseAccess.Instance.VerifyUserDataValidity();
            if (Universal.ParentForm.InvokeRequired) Universal.ParentForm.Invoke(new MethodInvoker(delegate { SplashScreen.Instance.Show(); }));
            else SplashScreen.Instance.Show();
            if (user.AccountType == "consumer")
            {
                if (Universal.ParentForm.InvokeRequired) Universal.ParentForm.Invoke(new MethodInvoker(LoadEssentialPanelsAfterLoginSuccess));
                else LoadEssentialPanelsAfterLoginSuccess();
                BackendManager.SendPendingNuntii();
                BackendManager.SyncWithTheServer();
                if (Universal.ParentForm.InvokeRequired) Universal.ParentForm.Invoke(new MethodInvoker(RefreshUsingLongPoolingMethod));
                else RefreshUsingLongPoolingMethod();
            }
        }

        private static string GenerateLocalDbPassword()
        {
            return Universal.SystemMACAddress + "" + User.LoggedIn.Id + "" + BackendManager.Password;
        }

        internal static void SetChangedPassword(string newPassword)
        {
            BackendManager.Password = newPassword;
            DatabaseAccess.Instance.ChangeLocalDataBasePassword(BackendManager.GenerateLocalDbPassword());
        }

        public static void RefreshUsingLongPoolingMethod()
        {
            KeepRefreshingRunning = true;
            BackgroundWorker bworker = new BackgroundWorker();
            bworker.DoWork += (s, e) =>
            {
                while (BackendManager.KeepRefreshingRunning)
                {
                    try
                    {
                        Thread.Sleep(30000);
                        ConversationPanel.CurrentDisplayedConversationPanel.RefreshCurrentConversationReceiver();
                    }
                    catch { }
                }
            };
            bworker.RunWorkerAsync();
            bworker.RunWorkerCompleted += (s, e) => { bworker.Dispose(); };
        }

        private static bool SendPendingNuntiiTaskGoing = false;
        public static void SendPendingNuntii()
        {
            while (SendPendingNuntiiTaskGoing)
            {
                Thread.Sleep(500);
                continue;
            }
            SendPendingNuntiiTaskGoing = true;
            List<Nuntias> pendingNuntiiList = NuntiasRepository.Instance.GetAllPendingPendingNuntiiList();
            if(pendingNuntiiList == null)
            {
                Universal.ShowErrorMessage("Error in accessing user data!");
                return;
            }
            Dictionary<long, long?> universalIdMapAgainstTmpId = new Dictionary<long, long?>();
            List<long> toBeDeleted = new List<long>();
            foreach (Nuntias tmpNuntias in pendingNuntiiList)
            {
                if (tmpNuntias.Id == 0) continue;
                long localNuntiasId = (long)tmpNuntias.Id;
                long? universalNuntiasId = null;
                if (tmpNuntias.ContentFileId == null || tmpNuntias.ContentFileId.Length == 0) universalNuntiasId = ServerRequest.SendNewNuntias(tmpNuntias);
                else universalNuntiasId = ServerFileRequest.SendContentedNuntias(tmpNuntias);
                if (universalNuntiasId == null || universalNuntiasId == 0) continue;
                tmpNuntias.Id = (long)universalNuntiasId;
                NuntiasRepository.Instance.Insert(tmpNuntias);
                SyncAssets.ConvertLocalNuntiasToGlobal(tmpNuntias, localNuntiasId);
                toBeDeleted.Add(localNuntiasId);
            }
            NuntiasRepository.Instance.DeleteTmpNuntii(toBeDeleted);
            SendPendingNuntiiTaskGoing = false;
        }

        private static void SyncWithTheServer()
        {
            ServerRequest.SyncUncachedData();
        }

        internal static void Logout()
        {
            SplashScreen sp = new SplashScreen();
            sp.Show();

            ServerHub.WorkingInstance.StopConnection();
            if(Universal.ParentForm.InvokeRequired)
            {
                Universal.ParentForm.Invoke(new MethodInvoker(delegate
                {
                    Logout();
                    return;
                }));
            }
            else
            {
                try
                {
                    SlidebarPanel.MySidebarPanel.Dispose();
                    ConversationPanel.CurrentDisplayedConversationPanel.Dispose();

                    LocalDataFileAccess.EraseCurrentLoginCookie();
                }
                catch { }
                Application.Restart();
            }
        }

        private static void ShowEmailVerificationInputPanel()
        {
            Universal.ParentForm.Controls.Add(new UserVerificationPanel(null, "email_verify"));
            SplashScreen.Instance.Hide();
        }

        public static void ShowPasswordEnterPanel()
        {
            Universal.ParentForm.Controls.Add((new LoginPanel(Universal.ParentForm, true, false)));
            SplashScreen.Instance.Hide();
        }

        public static void ShowPasswordSetupPanel()
        {
            (new LoginPanel(Universal.ParentForm, false, true)).ShowPasswordSetupPanel();
            SplashScreen.Instance.Hide();
        }

        public static void LoadEssentialPanelsAfterLoginSuccess()
        {
            SlidebarPanel.MySidebarPanel = new SlidebarPanel(Universal.ParentForm);
            Universal.ParentForm.Controls.Add(SlidebarPanel.MySidebarPanel);
            new ConversationPanel(Universal.ParentForm, Conversation.TopConversation);
            SplashScreen.Instance.Hide();
        }
    }
}
