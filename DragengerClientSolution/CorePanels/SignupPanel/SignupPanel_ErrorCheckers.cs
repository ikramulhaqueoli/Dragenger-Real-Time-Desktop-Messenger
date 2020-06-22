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

namespace CorePanels
{
    public partial class SignupPanel : Panel
    {
        private bool NameValid { set; get; }
        private bool UserNameValid { set; get; }
        private bool EmailValid { set; get; }
        private bool DeviceIDValid { set; get; }

        private Label nameBoxSign, nameBoxErrorMessage;
        private Label usernameBoxSign, usernameBoxErrorMessage;
        private Label emailBoxSign, emailBoxErrorMessage;
        private Label deviceIDSign, deviceIDErrorMessage;
        private Label signUpButtonMessage;
        private Timer userNameValidateTimer, emailValidateTimer;
        private System.Threading.Thread childThreadForErrorCheck;
        private Time lastUsernameTextChangeTime, lastEmailTextChangeTime;

        private void InitializeErrorShowers()
        {
            this.nameBoxSign = new Label();
            this.nameBoxSign.Size = new Size(this.nameBox.Height / 2, this.nameBox.Height / 2);
            this.nameBoxSign.Top = this.nameBox.Top + (this.nameBox.Height - nameBoxSign.Height) / 2;
            this.nameBoxSign.Left = this.nameBox.Right + 3;
            this.Controls.Add(this.nameBoxSign);
            this.nameBoxErrorMessage = new Label();
            this.nameBoxErrorMessage.Font = CustomFonts.Smallest;
            this.nameBoxErrorMessage.ForeColor = Colors.ErrorTextColor;
            this.nameBoxErrorMessage.Top = this.nameBox.Top - this.nameBoxErrorMessage.Height - 1;
            this.Controls.Add(this.nameBoxErrorMessage);

            this.usernameBoxSign = new Label();
            this.usernameBoxSign.Size = new Size(this.usernameBox.Height / 2, this.usernameBox.Height / 2);
            this.usernameBoxSign.Top = this.usernameBox.Top + (this.usernameBox.Height - usernameBoxSign.Height) / 2;
            this.usernameBoxSign.Left = this.usernameBox.Right + 3;
            this.Controls.Add(this.usernameBoxSign);
            this.usernameBoxErrorMessage = new Label();
            this.usernameBoxErrorMessage.Font = CustomFonts.Smallest;
            this.usernameBoxErrorMessage.ForeColor = Colors.ErrorTextColor;
            this.usernameBoxErrorMessage.Top = this.usernameBox.Top - this.usernameBoxErrorMessage.Height - 1;
            this.Controls.Add(this.usernameBoxErrorMessage);

            this.emailBoxSign = new Label();
            this.emailBoxSign.Size = new Size(this.emailBox.Height / 2, this.emailBox.Height / 2);
            this.emailBoxSign.Top = this.emailBox.Top + (this.emailBox.Height - emailBoxSign.Height) / 2;
            this.emailBoxSign.Left = this.emailBox.Right + 3;
            this.Controls.Add(this.emailBoxSign);
            this.emailBoxErrorMessage = new Label();
            this.emailBoxErrorMessage.Font = CustomFonts.Smallest;
            this.emailBoxErrorMessage.ForeColor = Colors.ErrorTextColor;
            this.emailBoxErrorMessage.Top = this.emailBox.Top - this.emailBoxErrorMessage.Height - 1;
            this.Controls.Add(this.emailBoxErrorMessage);

            this.deviceIDSign = new Label();
            this.deviceIDSign.Size = new Size(this.macAddressBox.Height / 2, this.macAddressBox.Height / 2);
            this.deviceIDSign.Top = this.macAddressBox.Top + (this.macAddressBox.Height - deviceIDSign.Height) / 2;
            this.deviceIDSign.Left = this.macAddressBox.Right + 3;
            this.Controls.Add(this.deviceIDSign);
            this.deviceIDErrorMessage = new Label();
            this.deviceIDErrorMessage.Font = CustomFonts.Smallest;
            this.deviceIDErrorMessage.ForeColor = Colors.ErrorTextColor;
            this.deviceIDErrorMessage.Top = this.macAddressBox.Top - this.deviceIDErrorMessage.Height - 1;
            this.Controls.Add(this.deviceIDErrorMessage);

            this.signUpButtonMessage = new Label();
            this.signUpButtonMessage.Font = CustomFonts.Smallest;
            this.signUpButtonMessage.TextAlign = ContentAlignment.TopRight;
            this.signUpButtonMessage.ForeColor = Colors.ErrorTextColor;
            this.signUpButtonMessage.Top = this.signUpButton.Bottom + 3;
            this.signUpButtonMessage.Visible = false;
            this.Controls.Add(this.signUpButtonMessage);

            this.SetDeviceIDValidity();

            childThreadForErrorCheck = new System.Threading.Thread(delegate() 
                {
                    this.userNameValidateTimer = new Timer();
                    this.userNameValidateTimer.Interval = 1000;
                    this.userNameValidateTimer.Tick += new EventHandler(this.SetUserNameValidity);

                    this.emailValidateTimer = new Timer();
                    this.emailValidateTimer.Interval = 1000;
                    this.emailValidateTimer.Tick += new EventHandler(this.SetEmailValidity);
                });
            childThreadForErrorCheck.Start();
        }

        private void SetNameValidity(Object sender, EventArgs e)
        {
            this.NameValid = false;
            if (((TextBox)sender).TextLength > 0)
            {
                string errorMessage = StandardAssuranceLibrary.Checker.CheckNameValidity(((TextBox)sender).Text);
                if (errorMessage == null) NameValid = true;

                if (NameValid)
                {
                    this.nameBoxSign.Image = new Bitmap(FileResources.Icon("ok.png"), nameBoxSign.Size);
                    this.nameBoxErrorMessage.Visible = false;
                    this.nameBoxSign.Visible = true;
                }
                else
                {
                    this.nameBoxErrorMessage.Text = errorMessage;
                    this.nameBoxErrorMessage.Size = this.nameBoxErrorMessage.PreferredSize;
                    this.nameBoxErrorMessage.Left = this.nameBox.Right - this.nameBoxErrorMessage.Width + 5;
                    this.nameBoxSign.Image = new Bitmap(FileResources.Icon("redwarning.png"), nameBoxSign.Size);
                    this.nameBoxErrorMessage.Visible = true;
                    this.nameBoxSign.Visible = true;
                }
            }
            else
            {
                nameBoxSign.Visible = false;
                nameBoxErrorMessage.Visible = false;
            }
            if (this.signUpButtonMessage.Visible) this.AllSignupConstraintsOk();
        }

        public void StartUsernameChecking(Object sender, EventArgs e)
        {
            this.usernameBoxSign.Visible = false;
            this.emailBoxErrorMessage.Visible = false;
            this.lastUsernameTextChangeTime = Time.CurrentTime;
            this.userNameValidateTimer.Start();
        }

        private bool WaitingUsernameAnimationShowing
        {
            set;
            get;
        }

        private void SetUserNameValidity(Object sender, EventArgs e)
        {
            if (this.usernameBox.TextLength > 0 && usernameBoxSign.Visible == false && WaitingUsernameAnimationShowing == false)
            {
                VisualizingTools.ShowWaitingAnimation(usernameBoxSign.Location, usernameBoxSign.Size, this);
                WaitingUsernameAnimationShowing = true;
            }
            this.UserNameValid = false;
            if (Time.TimeDistanceInSecond(this.lastUsernameTextChangeTime, Time.CurrentTime) < 3) return;
            this.userNameValidateTimer.Stop();

            if (this.usernameBox.TextLength > 0)
            {
                int selectionStartPrev = this.usernameBox.SelectionStart;
                string input = this.usernameBox.Text;
                string errorMessage = Checker.CheckUsernameValidity(ref input);
                this.usernameBox.Text = input;
                this.usernameBox.SelectionStart = selectionStartPrev;
                if (errorMessage == null) UserNameValid = true;

                if (UserNameValid)
                {
                    this.usernameBoxSign.Image = new Bitmap(FileResources.Icon("ok.png"), usernameBoxSign.Size);
                    this.usernameBoxErrorMessage.Visible = false;
                }
                else
                {
                    this.usernameBoxErrorMessage.Text = errorMessage;
                    this.usernameBoxErrorMessage.Size = this.usernameBoxErrorMessage.PreferredSize;
                    this.usernameBoxErrorMessage.Left = this.usernameBox.Right - this.usernameBoxErrorMessage.Width + 5;
                    this.usernameBoxSign.Image = new Bitmap(FileResources.Icon("redwarning.png"), usernameBoxSign.Size);
                    this.usernameBoxErrorMessage.Visible = true;
                }
                VisualizingTools.HideWaitingAnimation();
                this.WaitingUsernameAnimationShowing = false;
                this.usernameBoxSign.Visible = true;
            }
            else
            {
                usernameBoxSign.Visible = false;
                usernameBoxErrorMessage.Visible = false;
            }
            if (this.signUpButtonMessage.Visible) this.AllSignupConstraintsOk();
        }

        public void StartEmailChecking(Object sender, EventArgs e)
        {
            this.emailBoxSign.Visible = false;
            this.emailBoxErrorMessage.Visible = false;
            this.lastEmailTextChangeTime = Time.CurrentTime;
            this.emailValidateTimer.Start();
        }

        
        private bool WaitingEmailAnimationShowing
        {
            set;
            get;
        }
        private void SetEmailValidity(Object sender, EventArgs e)
        {
            if (this.emailBox.TextLength > 0 && emailBoxSign.Visible == false && WaitingEmailAnimationShowing == false)
            {
                VisualizingTools.ShowWaitingAnimation(emailBoxSign.Location, emailBoxSign.Size, this);
                WaitingEmailAnimationShowing = true;
            }
            this.EmailValid = false;
            if (Time.TimeDistanceInSecond(this.lastEmailTextChangeTime, Time.CurrentTime) < 3) return;
            this.emailValidateTimer.Stop();

            if (this.emailBox.TextLength > 0)
            {
                int selectionStartPrev = this.emailBox.SelectionStart;
                string input = this.emailBox.Text;
                string errorMessage = Checker.CheckEmailValidity(ref input);
                this.emailBox.Text = input;
                this.emailBox.SelectionStart = selectionStartPrev;
                if (errorMessage == null) EmailValid = true;

                if (EmailValid)
                {
                    this.emailBoxSign.Image = new Bitmap(FileResources.Icon("ok.png"), emailBoxSign.Size);
                    this.emailBoxErrorMessage.Visible = false;
                }
                else
                {
                    this.emailBoxErrorMessage.Text = errorMessage;
                    this.emailBoxErrorMessage.Size = this.emailBoxErrorMessage.PreferredSize;
                    this.emailBoxErrorMessage.Left = this.emailBox.Right - this.emailBoxErrorMessage.Width + 5;
                    this.emailBoxSign.Image = new Bitmap(FileResources.Icon("redwarning.png"), emailBoxSign.Size);
                    this.emailBoxErrorMessage.Visible = true;
                }
                this.emailBoxSign.Visible = true;
                this.WaitingEmailAnimationShowing = false;
            }
            else
            {
                emailBoxSign.Visible = false;
                emailBoxErrorMessage.Visible = false;
            }
            if (this.signUpButtonMessage.Visible) this.AllSignupConstraintsOk();
        }

        private void SetDeviceIDValidity()
        {
            string errorMessage = Checker.CheckDeviceIDValidity(this.macAddressBox.Text);
            if (errorMessage == null) this.DeviceIDValid = true;
            else this.DeviceIDValid = false;
            if (DeviceIDValid)
            {
                this.deviceIDSign.Image = new Bitmap(FileResources.Icon("ok.png"), deviceIDSign.Size);
                this.deviceIDErrorMessage.Visible = false;
                this.deviceIDSign.Visible = true;
            }
            else
            {
                this.deviceIDErrorMessage.Text = errorMessage;
                this.deviceIDErrorMessage.Size = this.deviceIDErrorMessage.PreferredSize;
                this.deviceIDErrorMessage.Left = this.macAddressBox.Right - this.deviceIDErrorMessage.Width + 5;
                this.deviceIDSign.Image = new Bitmap(FileResources.Icon("redwarning.png"), deviceIDSign.Size);
                this.deviceIDErrorMessage.Visible = true;
                this.deviceIDSign.Visible = true;
            }
            if (this.signUpButtonMessage.Visible) this.AllSignupConstraintsOk();
        }

        public bool AllSignupConstraintsOk()
        {
            this.signUpButtonMessage.Visible = false;
            string errorMessage = null;
            if (!this.NameValid) errorMessage = "Name not valid";
            if (!this.UserNameValid)
            {
                if (errorMessage != null) errorMessage += "\r\n";
                errorMessage += "Username not valid";
            }
            if (!this.EmailValid)
            {
                if (errorMessage != null) errorMessage += "\r\n";
                errorMessage += "Email address not valid";
            }
            if (!this.DeviceIDValid)
            {
                if (errorMessage != null) errorMessage += "\r\n";
                errorMessage += "Device ID not valid";
            }
            if (!this.policiesCheckBox.Checked)
            {
                if (errorMessage != null) errorMessage += "\r\n";
                errorMessage += "Please accept our 'Terms && Policies'";
            }

            if (errorMessage == null) return true;

            this.signUpButtonMessage.Text = errorMessage;
            this.signUpButtonMessage.Size = this.signUpButtonMessage.PreferredSize;
            this.signUpButtonMessage.Left = this.signUpButton.Right - this.signUpButtonMessage.Width + 5;
            this.signUpButtonMessage.Visible = true;

            return false;
        }
    }
}
