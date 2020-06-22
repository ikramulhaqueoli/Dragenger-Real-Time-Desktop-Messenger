using EntityLibrary;
using ResourceLibrary;
using ServerConnections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CorePanels
{
    internal class UserVerificationPanel : Panel
    {
        private Panel parent;
        private TextBox verificationCodeTextBox;
        private Label verificationCodeHeaderLabel, verificationCodeHintLabel, verificationCodeLabel, errorLabel;
        private Button verifyButton, backButton;
        private string headingString;
        private string purpose;
        public UserVerificationPanel(Panel parent, string purpose)
        {
            this.parent = parent;
            this.Size = Universal.ParentForm.Size;
            this.purpose = purpose;
            if(this.purpose == "email_verify") this.headingString = "WELCOME\r\n\r\nVERIFY EMAIL ADDRESS";
            else if(this.purpose == "password_reset") this.headingString = "RESET PASSWORD\r\n\r\nENTER VERIFICATION CODE";

            this.LabelInitialize();
            this.TextBoxInitialize();
            this.ButtonInitialize();
            this.SetAllLocations();
        }

        private void LabelInitialize()
        {
            this.verificationCodeHeaderLabel = new Label();
            this.verificationCodeHeaderLabel.Text = this.headingString;
            this.verificationCodeHeaderLabel.Font = CustomFonts.SmallBold;
            this.verificationCodeHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.verificationCodeHeaderLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.verificationCodeHeaderLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(verificationCodeHeaderLabel);

            this.verificationCodeHintLabel = new Label();
            this.verificationCodeHintLabel.Text = "A verification code is sent to your email.\r\nIt will expire after 60 minutes.";
            this.verificationCodeHintLabel.Font = CustomFonts.New(CustomFonts.SmallerSize, 'i');
            this.verificationCodeHintLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.verificationCodeHintLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.verificationCodeHintLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(verificationCodeHintLabel);

            this.verificationCodeLabel = new Label();
            this.verificationCodeLabel.Text = "Verification Code";
            this.verificationCodeLabel.Font = CustomFonts.SmallBold;
            this.verificationCodeLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.verificationCodeLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(verificationCodeLabel);

            this.errorLabel = new Label();
        }

        private void TextBoxInitialize()
        {
            this.verificationCodeTextBox = new TextBox();
            this.verificationCodeTextBox.Font = CustomFonts.BiggerBold;
            this.verificationCodeTextBox.BackColor = Color.FromArgb(190, 190, 190);
            this.verificationCodeTextBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.verificationCodeTextBox.BorderStyle = BorderStyle.None;
            this.verificationCodeTextBox.TextAlign = HorizontalAlignment.Center;
            this.verificationCodeTextBox.Size = new Size(150, verificationCodeTextBox.PreferredHeight);
            this.Controls.Add(verificationCodeTextBox);
        }

        private void ButtonInitialize()
        {
            this.verifyButton = new Button();
            this.verifyButton.Font = CustomFonts.SmallerBold;
            this.verifyButton.BackColor = Color.FromArgb(0, 0, 135);
            this.verifyButton.ForeColor = Color.FromArgb(210, 210, 210);
            this.verifyButton.FlatStyle = FlatStyle.Flat;
            this.verifyButton.FlatAppearance.BorderSize = 0;
            this.verifyButton.Text = "&Verify";
            this.verifyButton.Click += new EventHandler(EventListener);
            this.Controls.Add(verifyButton);

            this.backButton = new Button();
            this.backButton.Font = CustomFonts.SmallerBold;
            this.backButton.BackColor = Color.FromArgb(0, 0, 135);
            this.backButton.ForeColor = Color.FromArgb(210, 210, 210);
            this.backButton.FlatStyle = FlatStyle.Flat;
            this.backButton.FlatAppearance.BorderSize = 0;
            this.backButton.Text = "◄ &Back";
            if (this.parent == null) this.backButton.Text = "&Cancel";
            this.backButton.Click += new EventHandler(EventListener);
            this.Controls.Add(backButton);
        }

        private void SetAllLocations()
        {
            this.verificationCodeHeaderLabel.SetBounds((this.Size.Width - this.verificationCodeHeaderLabel.PreferredWidth) / 2, this.Size.Height / 12, verificationCodeHeaderLabel.PreferredWidth, verificationCodeHeaderLabel.PreferredHeight);
            this.verificationCodeLabel.SetBounds((this.Size.Width - this.verificationCodeLabel.PreferredWidth) / 2 - 5, this.verificationCodeHeaderLabel.Bottom + 100, verificationCodeLabel.PreferredWidth, verificationCodeLabel.PreferredHeight);
            this.verificationCodeTextBox.Location = new Point((this.Size.Width - this.verificationCodeTextBox.Width) / 2 - 5, this.verificationCodeLabel.Bottom + 5);
            this.verificationCodeHintLabel.SetBounds((this.Width - this.verificationCodeHintLabel.PreferredWidth) / 2 - 5, this.verificationCodeTextBox.Bottom + 10, verificationCodeHintLabel.PreferredWidth, verificationCodeHintLabel.PreferredHeight);
            this.verifyButton.Size = this.backButton.Size = this.backButton.PreferredSize;
            this.verifyButton.Location = new Point((this.Width - this.verifyButton.PreferredSize.Width) / 2, this.verificationCodeHintLabel.Bottom + 50);
            this.backButton.Location = new Point(this.verifyButton.Left, this.verifyButton.Bottom + 10);
        }

        private void EventListener(object sender, EventArgs ee)
        {
            if (sender == this.verifyButton)
            {
                string errorMessage = "";
                if (verificationCodeTextBox.Text == null || verificationCodeTextBox.Text.Length == 0)
                {
                    errorMessage += "Verification Code Field Empty!";
                    ShowErrorMessage(errorMessage);
                    return;
                }
                VisualizingTools.ShowWaitingAnimation(new Point(this.verifyButton.Left, this.verifyButton.Bottom + 20), new Size(this.verifyButton.Width, this.verifyButton.Height / 2), this);
                BackgroundWorker loaderWorker = new BackgroundWorker();
                loaderWorker.DoWork += (s, e) =>
                {
                    try
                    {
                        int? status = null;
                        status = ServerRequest.VerifyVerificationCode(verificationCodeTextBox.Text, this.purpose);
                        if (status == 1)        //1 means verification code is verified successfully
                        {
                            this.Invoke(new Action(() =>
                            {
                                VisualizingTools.HideWaitingAnimation();
                                this.Visible = false;
                                this.Dispose();
                                if(this.parent != null)
                                {
                                    this.parent.Hide();
                                    this.parent.Dispose();
                                }
                            }
                            ));
                            if (this.purpose == "email_verify") BackendManager.LoginProcessRun();
                            else if(this.purpose == "password_reset") Universal.ParentForm.Invoke(new MethodInvoker(BackendManager.ShowPasswordSetupPanel));
                        }
                        else
                        {
                            if (status == 2)       //2 means verification code is not valid
                            {
                                errorMessage = "Verification Code is invalid!"; 
                            }
                            else if(status == 3)   //3 means verification code is expried
                            {
                                errorMessage = "Verification Code is expired!\r\nA new verification code is sent.";
                            }
                            else if(status == 4)   //4 means too many wrong (more than 5) attempts
                            {
                                errorMessage = "Too many unsuccessful attempts!\r\nA new verification code is sent.";
                            }
                            else
                            {
                                errorMessage = "Server connection failed!";
                            }
                            this.Invoke(new Action(() =>
                            {
                                ShowErrorMessage(errorMessage);
                            }
                            ));
                        }
                    }
                    catch (Exception ex) { Console.WriteLine("Exception in UserVerificationPanel.cs = > " + ex.Message); }
                };
                loaderWorker.RunWorkerAsync();
                loaderWorker.RunWorkerCompleted += (s, e) => { loaderWorker.Dispose(); };
            }
            else if (sender == this.backButton)
            {
                if(this.parent != null) this.parent.Visible = true;
                else
                {
                    ServerRequest.DeleteConsumerAccount(Universal.SystemMACAddress, "");
                    BackendManager.Logout();
                }
                this.Dispose();
            }
        }
        private void ShowErrorMessage(string errorMessage)
        {
            if (errorMessage.Length > 0)
            {
                VisualizingTools.HideWaitingAnimation();
                this.errorLabel.Text = errorMessage;
                this.errorLabel.Font = CustomFonts.New(CustomFonts.SmallerSize, 'i');
                this.errorLabel.TextAlign = ContentAlignment.MiddleCenter;
                this.errorLabel.ForeColor = Color.Red;
                this.errorLabel.Size = this.errorLabel.PreferredSize;
                this.errorLabel.Location = new Point((this.Width - this.errorLabel.PreferredWidth) / 2, this.backButton.Bottom + 15);
                this.Controls.Add(this.errorLabel);
            }
        }
    }
}
