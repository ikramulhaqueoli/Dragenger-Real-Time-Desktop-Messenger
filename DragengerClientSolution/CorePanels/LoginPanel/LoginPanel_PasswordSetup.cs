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
using System.Threading;
using ServerConnections;

namespace CorePanels
{
    public partial class LoginPanel : Panel
    {
        private Label setupWelcomeLabel, setupWelcomeSpeechLabel, oldPasswordLabel, newPasswordLabel, confirmPasswordLabel;
        private TextBox oldPasswordBox, newPasswordBox, confirmPasswordBox;
        private Button passwordSetupButton, backButton;
        private string oldPasswordString;

        public void ShowPasswordSetupPanel()
        {
            this.LoginKeySetupBoxesInitialize();
            this.LoginKeySetupLabelsInitialize();
            this.LoginKeyButtonsInitialize();
            this.LoginKeySetupAtParentControl();
            this.InitializeKeySetupErrorShowers();
            this.oldPasswordString = "";
            if (this.passwordIsSet)
            {
                backButton = new Button();
                backButton.Text = "Back";
                backButton.Font = passwordSetupButton.Font;
                backButton.Height = passwordSetupButton.Height;
                backButton.Width = backButton.PreferredSize.Width;
                backButton.FlatStyle = FlatStyle.Flat;
                backButton.ForeColor = Color.FromArgb(221,221,221);
                backButton.MouseEnter += (s, e) => { backButton.BackColor = Color.FromArgb(backButton.BackColor.R - 50, backButton.BackColor.G - 50, backButton.BackColor.B - 50); };
                backButton.MouseLeave += (s, e) => { backButton.BackColor = Color.FromArgb(backButton.BackColor.R + 50, backButton.BackColor.G + 50, backButton.BackColor.B + 50); };
                backButton.Click += (s, e) => { Universal.ParentForm.Controls.Remove(this); ConversationPanel.CurrentDisplayedConversationPanel.Visible = true; SlidebarPanel.MySidebarPanel.Visible = true; };
                backButton.BackColor = Color.FromArgb(51, 51, 51);
                backButton.Location = new Point(oldPasswordBox.Left, passwordSetupButton.Top);
                this.Controls.Add(backButton);
            }
            else
            {
                this.oldPasswordBox.Visible = false;
                this.oldPasswordLabel.Visible = false;
            }
        }

        private void LoginKeySetupAtParentControl()
        {
            this.setupWelcomeLabel.Location = new Point((this.Width - this.setupWelcomeLabel.PreferredWidth) / 2, this.Height / 8);
            this.setupWelcomeSpeechLabel.Location = new Point((this.Width - this.setupWelcomeSpeechLabel.PreferredWidth) / 2, setupWelcomeLabel.Bottom + this.setupWelcomeSpeechLabel.PreferredHeight);
            this.oldPasswordBox.Left = (this.Width - this.oldPasswordBox.Width) / 2;
            this.oldPasswordLabel.Left = this.oldPasswordBox.Left - 5;
            this.newPasswordLabel.Left = this.oldPasswordBox.Left - 5;
            this.newPasswordBox.Left = this.oldPasswordBox.Left;
            this.confirmPasswordLabel.Left = this.oldPasswordBox.Left - 5;
            this.confirmPasswordBox.Left = this.oldPasswordBox.Left;
            this.passwordSetupButton.Left = confirmPasswordBox.Right - this.passwordSetupButton.Width;

            this.oldPasswordLabel.Top = this.setupWelcomeSpeechLabel.Bottom + (this.oldPasswordBox.Height * 2);
            this.oldPasswordBox.Top = this.oldPasswordLabel.Bottom + this.oldPasswordLabel.Height / 3;
            this.newPasswordLabel.Top = this.oldPasswordBox.Bottom + this.oldPasswordLabel.Height * 2;
            this.newPasswordBox.Top = this.newPasswordLabel.Bottom + this.oldPasswordLabel.Height / 3;
            this.confirmPasswordLabel.Top = this.newPasswordBox.Bottom + this.newPasswordBox.Height * 2;
            this.confirmPasswordBox.Top = this.confirmPasswordLabel.Bottom + this.oldPasswordLabel.Height / 3;
            this.passwordSetupButton.Top = this.confirmPasswordBox.Bottom + this.oldPasswordLabel.Height * 2;

            this.parent.Controls.Add(this);
        }

        private void LoginKeySetupLabelsInitialize()
        {
            this.setupWelcomeLabel = new Label();
            if(!this.passwordIsSet) this.setupWelcomeLabel.Text = "Setup Your Password";
            else this.setupWelcomeLabel.Text = "Change Your Password";
            this.setupWelcomeLabel.Font = CustomFonts.BiggerBold;
            this.setupWelcomeLabel.Size = this.setupWelcomeLabel.PreferredSize;
            this.setupWelcomeLabel.ForeColor = Color.FromArgb(120, 120, 120);
            this.Controls.Add(setupWelcomeLabel);

            this.setupWelcomeSpeechLabel = new Label();
            if (!this.passwordIsSet) this.setupWelcomeSpeechLabel.Text = "No Worries! You can change your password anytime later";
            else this.setupWelcomeSpeechLabel.Visible = false;
            this.setupWelcomeSpeechLabel.Font = CustomFonts.Smallest;
            this.setupWelcomeSpeechLabel.Size = this.setupWelcomeSpeechLabel.PreferredSize;
            this.setupWelcomeSpeechLabel.ForeColor = Color.FromArgb(120, 120, 120);
            this.Controls.Add(setupWelcomeSpeechLabel);

            this.oldPasswordLabel = new Label();
            this.oldPasswordLabel.Text = "Enter Current Password";
            this.oldPasswordLabel.Font = CustomFonts.SmallBold;
            this.oldPasswordLabel.Size = this.oldPasswordLabel.PreferredSize;
            this.oldPasswordLabel.ForeColor = Color.FromArgb(120, 120, 120);
            this.Controls.Add(oldPasswordLabel);

            this.newPasswordLabel = new Label();
            this.newPasswordLabel.Text = "Enter New Password";
            this.newPasswordLabel.Font = CustomFonts.SmallBold;
            this.newPasswordLabel.Size = this.newPasswordLabel.PreferredSize;
            this.newPasswordLabel.ForeColor = Color.FromArgb(120, 120, 120);
            this.Controls.Add(newPasswordLabel);

            this.confirmPasswordLabel = new Label();
            this.confirmPasswordLabel.Text = "Confirm Password";
            this.confirmPasswordLabel.Font = CustomFonts.SmallBold;
            this.confirmPasswordLabel.Size = this.confirmPasswordLabel.PreferredSize;
            this.confirmPasswordLabel.ForeColor = Color.FromArgb(120, 120, 120);
            this.Controls.Add(confirmPasswordLabel);
        }

        private void LoginKeySetupBoxesInitialize()
        {
            this.oldPasswordBox = new TextBox();
            this.oldPasswordBox.Font = CustomFonts.BigBold;
            this.oldPasswordBox.PasswordChar = '•';
            this.oldPasswordBox.Size = new Size(this.Size.Width - (this.Size.Width / 3), oldPasswordBox.PreferredHeight);
            this.oldPasswordBox.BackColor = Color.FromArgb(190, 190, 190);
            this.oldPasswordBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.oldPasswordBox.BorderStyle = BorderStyle.None;
            this.oldPasswordBox.TextChanged += (s, e) => { this.oldPasswordString = ((TextBox)s).Text; };
            this.Controls.Add(oldPasswordBox);

            this.newPasswordBox = new TextBox();
            this.newPasswordBox.Font = CustomFonts.BigBold;
            this.newPasswordBox.PasswordChar = '•';
            this.newPasswordBox.Size = new Size(this.Size.Width - (this.Size.Width / 3), newPasswordBox.PreferredHeight);
            this.newPasswordBox.BackColor = Color.FromArgb(190, 190, 190);
            this.newPasswordBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.newPasswordBox.BorderStyle = BorderStyle.None;
            this.newPasswordBox.TextChanged += new EventHandler(this.SetNewPasswordValidity);
            this.Controls.Add(newPasswordBox);

            this.confirmPasswordBox = new TextBox();
            this.confirmPasswordBox.Font = CustomFonts.BigBold;
            this.confirmPasswordBox.PasswordChar = '•';
            this.confirmPasswordBox.Size = new Size(this.Size.Width - (this.Size.Width / 3), confirmPasswordBox.PreferredHeight);
            this.confirmPasswordBox.BackColor = Color.FromArgb(190, 190, 190);
            this.confirmPasswordBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.confirmPasswordBox.BorderStyle = BorderStyle.None;
            this.confirmPasswordBox.TextChanged += new EventHandler(this.SetConfirmPasswordValidity);
            this.Controls.Add(confirmPasswordBox);
        }

        private void LoginKeyButtonsInitialize()
        {
            this.passwordSetupButton = new Button();
            this.passwordSetupButton.Name = "passwordSetupButton";
            if (!this.passwordIsSet) this.passwordSetupButton.Text = "Setup Password";
            else this.passwordSetupButton.Text = "Change Password";
            this.passwordSetupButton.Font = CustomFonts.SmallerBold;
            this.passwordSetupButton.Size = this.passwordSetupButton.PreferredSize;
            this.passwordSetupButton.BackColor = Color.FromArgb(0, 0, 135);
            this.passwordSetupButton.ForeColor = Color.FromArgb(210, 210, 210);
            this.passwordSetupButton.FlatStyle = FlatStyle.Flat;
            this.passwordSetupButton.FlatAppearance.BorderSize = 0;
            this.passwordSetupButton.Click += new EventHandler(this.OnSetupButtonClick);
            this.Controls.Add(passwordSetupButton);
        }

        private void OnSetupButtonClick(Object sender, EventArgs e)
        {
            VisualizingTools.ShowWaitingAnimation(new Point(this.passwordSetupButton.Left, this.passwordSetupButton.Bottom + 30), new Size(this.passwordSetupButton.Width, this.passwordSetupButton.Height / 2), this);
            Thread childThread = new Thread(delegate() 
                {
                    if (this.AllKeySetupConstraintsOk())
                    {
                        TryToSetupLoginKey();
                    }
                    else
                    {
                        if (this.InvokeRequired) this.Invoke(new MethodInvoker(delegate
                        {
                            VisualizingTools.HideWaitingAnimation();
                        }));
                        else VisualizingTools.HideWaitingAnimation();
                    }
                });
            childThread.Start();
        }

        private void TryToSetupLoginKey()
        {
            string errorMessage = ServerRequest.SetPassword(Universal.SystemMACAddress, this.newPassword);
            if (errorMessage != null)
            {
                VisualizingTools.HideWaitingAnimation();
                MessageBox.Show(errorMessage);
            }
            else
            {
                if (this.InvokeRequired) this.Invoke(new MethodInvoker(delegate
                    {
                        this.parent.Controls.Remove(this);
                        VisualizingTools.HideWaitingAnimation();
                    }));
                else
                {
                    this.parent.Controls.Remove(this);
                    VisualizingTools.HideWaitingAnimation();
                }
                if (!this.passwordIsSet) BackendManager.LoginProcessRun();
                else
                {
                    BackendManager.SetChangedPassword(this.newPassword);
                    if (Universal.ParentForm.InvokeRequired)
                    {
                        Universal.ParentForm.Invoke(new Action(() =>
                        {
                            Universal.ParentForm.Controls.Remove(this);
                            ConversationPanel.CurrentDisplayedConversationPanel.Visible = true;
                            SlidebarPanel.MySidebarPanel.Visible = true;
                            MessageBox.Show(Universal.ParentForm, "Password has been sucessfully changed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                    }
                    else
                    {
                        Universal.ParentForm.Controls.Remove(this);
                        ConversationPanel.CurrentDisplayedConversationPanel.Visible = true;
                        SlidebarPanel.MySidebarPanel.Visible = true;
                        MessageBox.Show(Universal.ParentForm, "Password has been sucessfully changed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}
