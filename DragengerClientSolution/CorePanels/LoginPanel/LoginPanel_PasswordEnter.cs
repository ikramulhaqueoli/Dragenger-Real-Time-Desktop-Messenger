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
using FileIOAccess;
using System.Threading;
using MathLaboratory;
using Newtonsoft.Json.Linq;
using ServerConnections;

namespace CorePanels
{
    public partial class LoginPanel : Panel
    {
        private Form parent;
        private TextBox passwordBox;
        private Button loginButton;
        private CheckBox rememberCheck;
        private Label enterLoginLabel, logoLabel;
        private LinkLabel forgotPasswordLinkLabel;
        private bool passwordIsSet;

        public LoginPanel(Form parent, bool passwordIsSet, bool toSetup)
        {
            this.parent = parent;
            this.passwordIsSet = passwordIsSet;
            this.Size = this.parent.ClientSize;
            this.BackColor = Color.FromArgb(221, 221, 221);
            if (passwordIsSet && !toSetup) this.ShowLoginKeyEnterPanel();
            else this.ShowPasswordSetupPanel();
        }

        private void ShowLoginKeyEnterPanel()
        {
            this.LogoAndLoginKeyBoxInitialize();
            this.LoginButtonInitialize();
            this.RememberCheckInitialize();
            this.LabelInitialize();
        }

        private void LogoAndLoginKeyBoxInitialize()
        {
            this.logoLabel = new Label();
            Image logoImg = FileResources.Icon("logo_with_name.png");
            double logoHeight = this.Height / 4.0;
            double logoWidth = logoImg.Width * (logoHeight / logoImg.Height);
            this.logoLabel.Image = new Bitmap(logoImg, new Size((int)logoWidth, (int)logoHeight));
            this.logoLabel.Size = this.logoLabel.Image.Size;
            this.logoLabel.Location = new Point((this.Width - this.logoLabel.Width) / 2, 30);
            this.Controls.Add(this.logoLabel);

            this.passwordBox = new TextBox();
            this.passwordBox.Font = CustomFonts.BigBold;
            this.passwordBox.PasswordChar = '•';
            this.passwordBox.Size = new Size(this.Size.Width - (this.Size.Width / 3), passwordBox.PreferredHeight);
            this.passwordBox.Location = new Point((this.Size.Width - this.passwordBox.Width) / 2, this.Height / 2);
            this.passwordBox.BackColor = Color.FromArgb(190, 190, 190);
            this.passwordBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.passwordBox.BorderStyle = BorderStyle.None;
            this.Controls.Add(passwordBox);
        }

        /// <summary>
        /// 
        /// </summary>
        public void LabelInitialize()
        {
            this.enterLoginLabel = new Label();
            this.enterLoginLabel.Text = "Enter Password";
            Rectangle lbound = this.passwordBox.Bounds;
            this.enterLoginLabel.Font = CustomFonts.SmallBold;
            this.enterLoginLabel.ForeColor = Color.FromArgb(120, 120, 120);
            this.enterLoginLabel.SetBounds(lbound.Left - 5, lbound.Top + 5 - enterLoginLabel.PreferredHeight * 2, enterLoginLabel.PreferredWidth, enterLoginLabel.PreferredHeight);
            this.Controls.Add(enterLoginLabel);

            this.forgotPasswordLinkLabel = new LinkLabel();
            this.forgotPasswordLinkLabel.Text = "&Forgot Password";
            this.forgotPasswordLinkLabel.TextAlign = ContentAlignment.MiddleRight;
            this.forgotPasswordLinkLabel.Font = CustomFonts.Smaller;
            this.forgotPasswordLinkLabel.LinkArea = new LinkArea(0, 16);
            this.forgotPasswordLinkLabel.LinkBehavior = LinkBehavior.NeverUnderline;
            this.forgotPasswordLinkLabel.Size = this.forgotPasswordLinkLabel.PreferredSize;
            this.forgotPasswordLinkLabel.Location = new Point(this.loginButton.Left - this.forgotPasswordLinkLabel.Width - 10, this.loginButton.Top);
            this.forgotPasswordLinkLabel.LinkClicked += delegate (Object sender, LinkLabelLinkClickedEventArgs e) 
            {
                BackgroundWorker loaderWorker = new BackgroundWorker();
                loaderWorker.DoWork += (ss, ee) =>
                {
                    ServerRequest.SendVerificationCodeToResetPassword();
                };
                loaderWorker.RunWorkerAsync();
                loaderWorker.RunWorkerCompleted += (ss, ee) => { loaderWorker.Dispose(); };
                this.Visible = false;
                this.parent.Controls.Add(new UserVerificationPanel(this, "password_reset"));
            };
            this.Controls.Add(forgotPasswordLinkLabel);
        }

        private void LoginButtonInitialize()
        {
            this.loginButton = new Button();
            this.loginButton.Name = "LoginButton";
            Rectangle lbound = this.passwordBox.Bounds;
            this.loginButton.Font = CustomFonts.SmallerBold;
            this.loginButton.BackColor = Color.FromArgb(0, 0, 135);
            this.loginButton.ForeColor = Color.FromArgb(210, 210, 210);
            this.loginButton.FlatStyle = FlatStyle.Flat;
            this.loginButton.FlatAppearance.BorderSize = 0;
            this.loginButton.Text = "Sign in";
            this.loginButton.SetBounds(lbound.Right - loginButton.PreferredSize.Width, lbound.Bottom + 15, loginButton.PreferredSize.Width, loginButton.PreferredSize.Height);
            this.loginButton.Click += new EventHandler(OnLoginButtonClick);
            this.Controls.Add(loginButton);
        }

        private void RememberCheckInitialize()
        {
            this.rememberCheck = new CheckBox();
            Rectangle sbound = this.loginButton.Bounds;
            Rectangle lbound = this.passwordBox.Bounds;
            this.rememberCheck.Font = CustomFonts.Smallest;
            this.rememberCheck.Text = "Remember Login";
            this.rememberCheck.ForeColor = Color.FromArgb(65, 65, 65);
            this.rememberCheck.SetBounds(lbound.Left, sbound.Top, rememberCheck.PreferredSize.Width, rememberCheck.PreferredSize.Height);
            this.Controls.Add(rememberCheck);
        }

        private bool RememberLogin
        {
            set;
            get;
        }

        private void OnLoginButtonClick(Object sender, EventArgs e)
        {
            if (((Button)sender).Name == this.loginButton.Name)
            {
                VisualizingTools.ShowWaitingAnimation(new Point(loginButton.Right - loginButton.Width, loginButton.Bottom + 30), new Size(loginButton.Width, loginButton.Height / 2), this);
                Thread childThread = new Thread(TryToLogin);
                childThread.Start();
            }
        }

        private void TryToLogin()
        {
            BackendManager.Password = passwordBox.Text;
            this.RememberLogin = this.rememberCheck.Checked;
            bool? loginSuccess = ServerConnections.ServerRequest.LoginWithCredentials(Universal.SystemMACAddress, BackendManager.Password);
            if (loginSuccess == true)
            {
                if (this.RememberLogin)
                {
                    BackendManager.SaveLoginCookie();
                }
                this.Invoke(new MethodInvoker(delegate
                {
                    this.parent.Controls.Remove(this);
                }));
                BackendManager.LoginNow(User.LoggedIn);
            }
            else
            {
                if (this.InvokeRequired)
                {

                    this.Invoke(new MethodInvoker(delegate
                    {
                        VisualizingTools.HideWaitingAnimation();
                        Label errorLabel = new Label();
                        if (loginSuccess == false) errorLabel.Text = "Incorrect password!";
                        else errorLabel.Text = "Communication with database failed!";
                        errorLabel.ForeColor = Colors.ErrorTextColor;
                        errorLabel.Font = CustomFonts.Smallest;
                        errorLabel.Size = errorLabel.PreferredSize;
                        errorLabel.Location = new Point(this.loginButton.Right - errorLabel.Width + 5, this.loginButton.Bottom + 3);
                        this.Controls.Add(errorLabel);
                    }));
                }
            }
        }
    }
}
