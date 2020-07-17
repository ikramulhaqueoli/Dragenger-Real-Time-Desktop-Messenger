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
    internal class DeviceBindPanel : Panel
    {
        private Panel parent;
        private TextBox usernameTextBox, passwordTextBox, macAddressBox;
        private Label bindDeviceTitleLabel, macAddressLabel, usernameLabel, passwordLabel, loginLabel, errorLabel;
        private Button bindButton, backButton;
        private CheckBox loginCheckbox;
        public DeviceBindPanel(Panel parent)
        {
            this.parent = parent;
            this.Size = this.parent.Size;

            this.LabelInitialize();
            this.TextBoxInitialize();
            this.ButtonInitialize();
            this.SetAllLocations();
        }

        private void LabelInitialize()
        {
            this.bindDeviceTitleLabel = new Label();
            this.bindDeviceTitleLabel.Text = "BIND THIS DEVICE WITH\r\nAN EXISTING ACCOUNT";
            this.bindDeviceTitleLabel.Font = CustomFonts.SmallBold;
            this.bindDeviceTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.bindDeviceTitleLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.bindDeviceTitleLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(bindDeviceTitleLabel);

            this.macAddressLabel = new Label();
            this.macAddressLabel.Text = "Device MAC";
            this.macAddressLabel.Font = CustomFonts.SmallBold;
            this.macAddressLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.macAddressLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(macAddressLabel);

            this.usernameLabel = new Label();
            this.usernameLabel.Text = "Username";
            this.usernameLabel.Font = CustomFonts.SmallBold;
            this.usernameLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.usernameLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(usernameLabel);

            this.passwordLabel = new Label();
            this.passwordLabel.Text = "Password";
            this.passwordLabel.Font = CustomFonts.SmallBold;
            this.passwordLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.passwordLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(passwordLabel);

            this.loginLabel = new Label();
            this.loginLabel.Text = "Keep me logged in";
            this.loginLabel.Font = CustomFonts.Smallest;
            this.Controls.Add(loginLabel);
        }

        private void TextBoxInitialize()
        {
            this.macAddressBox = new TextBox();
            this.macAddressBox.Text = Universal.SystemMACAddress;
            this.macAddressBox.Font = CustomFonts.BigBold;
            this.macAddressBox.BackColor = Color.FromArgb(190, 190, 190);
            this.macAddressBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.macAddressBox.BorderStyle = BorderStyle.None;
            this.macAddressBox.Size = new Size(this.Size.Width - (this.Size.Width / 3), macAddressBox.PreferredHeight);
            this.macAddressBox.ReadOnly = true;
            this.Controls.Add(macAddressBox);

            this.usernameTextBox = new TextBox();
            this.usernameTextBox.Font = CustomFonts.BigBold;
            this.usernameTextBox.BackColor = Color.FromArgb(190, 190, 190);
            this.usernameTextBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.usernameTextBox.BorderStyle = BorderStyle.None;
            this.usernameTextBox.Size = new Size(this.Size.Width - (this.Size.Width / 3), macAddressBox.PreferredHeight);
            this.Controls.Add(usernameTextBox);

            this.passwordTextBox = new TextBox();
            this.passwordTextBox.Font = CustomFonts.BigBold;
            this.passwordTextBox.BackColor = Color.FromArgb(190, 190, 190);
            this.passwordTextBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.passwordTextBox.BorderStyle = BorderStyle.None;
            this.passwordTextBox.PasswordChar = '•';
            this.passwordTextBox.Size = new Size(this.Size.Width - (this.Size.Width / 3), macAddressBox.PreferredHeight);
            this.Controls.Add(passwordTextBox);

            this.loginCheckbox = new CheckBox();
            this.loginCheckbox.Font = CustomFonts.Smallest;
            this.loginCheckbox.ForeColor = Color.FromArgb(65, 65, 65);
            this.loginLabel.Click += delegate(Object sender, EventArgs e) { this.loginCheckbox.Checked = !this.loginCheckbox.Checked; };
            this.Controls.Add(loginCheckbox);
        }

        private void ButtonInitialize()
        {
            this.bindButton = new Button();
            this.bindButton.Font = CustomFonts.SmallerBold;
            this.bindButton.BackColor = Color.FromArgb(0, 0, 135);
            this.bindButton.ForeColor = Color.FromArgb(210, 210, 210);
            this.bindButton.FlatStyle = FlatStyle.Flat;
            this.bindButton.FlatAppearance.BorderSize = 0;
            this.bindButton.Text = "&Bind Device";
            this.bindButton.Click += new EventHandler(EventListener);
            this.Controls.Add(bindButton);

            this.backButton = new Button();
            this.backButton.Font = CustomFonts.SmallerBold;
            this.backButton.BackColor = Color.FromArgb(0, 0, 135);
            this.backButton.ForeColor = Color.FromArgb(210, 210, 210);
            this.backButton.FlatStyle = FlatStyle.Flat;
            this.backButton.FlatAppearance.BorderSize = 0;
            this.backButton.Text = "◄ &Back";
            this.backButton.Click += new EventHandler(EventListener);
            this.Controls.Add(backButton);
        }

        private void SetAllLocations()
        {
            this.bindDeviceTitleLabel.SetBounds((this.Size.Width - this.bindDeviceTitleLabel.PreferredWidth) / 2, this.Size.Height / 12, bindDeviceTitleLabel.PreferredWidth, bindDeviceTitleLabel.PreferredHeight);
            this.macAddressLabel.SetBounds((this.Size.Width - this.usernameTextBox.Width) / 2 - 5, this.bindDeviceTitleLabel.Bottom + 80, macAddressLabel.PreferredWidth, macAddressLabel.PreferredHeight);
            this.macAddressBox.Location = new Point((this.Size.Width - this.usernameTextBox.Width) / 2, this.macAddressLabel.Bottom + 5);
            this.usernameLabel.SetBounds((this.Size.Width - this.usernameTextBox.Width) / 2 - 5, this.macAddressBox.Bottom + 20, usernameLabel.PreferredWidth, usernameLabel.PreferredHeight);
            this.usernameTextBox.Location = new Point((this.Size.Width - this.usernameTextBox.Width) / 2, this.usernameLabel.Bottom + 5);
            this.passwordLabel.SetBounds((this.Size.Width - this.usernameTextBox.Width) / 2 - 5, this.usernameTextBox.Bottom + 20, passwordLabel.PreferredWidth, passwordLabel.PreferredHeight);
            this.passwordTextBox.Location = new Point((this.Size.Width - this.usernameTextBox.Width) / 2, this.passwordLabel.Bottom + 5);
            this.loginCheckbox.SetBounds((this.Size.Width - this.usernameTextBox.Width) / 2, this.passwordTextBox.Bottom + 10, this.loginCheckbox.PreferredSize.Width, this.loginCheckbox.PreferredSize.Height);
            this.loginLabel.SetBounds(this.loginCheckbox.Right + 2, this.loginCheckbox.Top - 2, loginLabel.PreferredSize.Width, loginLabel.PreferredSize.Height);
            this.bindButton.SetBounds(this.passwordTextBox.Right - this.bindButton.PreferredSize.Width, this.passwordTextBox.Bottom + 60, bindButton.PreferredSize.Width, bindButton.PreferredSize.Height);
            this.backButton.SetBounds((this.Size.Width - this.usernameTextBox.Width) / 2, this.bindButton.Top, backButton.PreferredSize.Width, backButton.PreferredSize.Height);
        }

        private void EventListener(object sender, EventArgs ee)
        {
            if(sender == this.bindButton)
            {
                string errorMessage = "";
                bool allOk = true;
                if(macAddressBox.Text == null || macAddressBox.Text.Length == 0)
                {
                    errorMessage += "Invalid MAC Address Input!";
                    allOk = false;
                }
                if (usernameTextBox.Text == null || usernameTextBox.Text.Length == 0)
                {
                    if (errorMessage.Length > 0) errorMessage += "\r\n";
                    errorMessage += "Username is empty!";
                    allOk = false;
                }
                if (passwordTextBox.Text == null || passwordTextBox.Text.Length == 0)
                {
                    if (errorMessage.Length > 0) errorMessage += "\r\n";
                    errorMessage += "Password is empty!";
                    allOk = false;
                }
                if (allOk)
                {
                    VisualizingTools.ShowWaitingAnimation(new Point(this.bindButton.Left, this.bindButton.Bottom + 20), new Size(this.bindButton.Width, this.bindButton.Height / 2), this);
                    BackgroundWorker loaderWorker = new BackgroundWorker();
                    loaderWorker.DoWork += (s, e) =>
                    {
                        try
                        {
                            bool? success = ServerRequest.BindDeviceAndLogin(macAddressBox.Text, usernameTextBox.Text, passwordTextBox.Text);
                            if (success == true)
                            {
                                if (this.loginCheckbox.Checked)
                                {
                                    BackendManager.SaveLoginCookie();
                                }
                                BackendManager.LoginNow(User.LoggedIn);
                                this.Invoke(new Action(() =>
                                {
                                    VisualizingTools.HideWaitingAnimation();
                                    this.Visible = false;
                                    this.parent.Visible = false;
                                    this.parent.Dispose();
                                    this.Dispose();
                                }
                                ));
                            }
                            else if (success == false)
                            {
                                errorMessage = "Invalid username or password!";
                                this.Invoke(new Action(() =>
                                {
                                    ShowErrorMessage(errorMessage);
                                }
                                ));
                            }
                            else
                            {
                                errorMessage = "Server connection failed!";
                                this.Invoke(new Action(() =>
                                {
                                    ShowErrorMessage(errorMessage);
                                }
                                ));
                            }
                        }
                        catch { }
                    };
                    loaderWorker.RunWorkerAsync();
                    loaderWorker.RunWorkerCompleted += (s, e) => { loaderWorker.Dispose(); };
                }
                else
                {
                    ShowErrorMessage(errorMessage);
                }
            }
            else if(sender == this.backButton)
            {
                this.parent.Visible = true;
                this.Dispose();
            }
        }
        private void ShowErrorMessage(string errorMessage)
        {
            if (errorMessage.Length > 0)
            {
                VisualizingTools.HideWaitingAnimation();
                this.errorLabel = new Label();
                this.errorLabel.Text = errorMessage;
                this.errorLabel.Font = CustomFonts.New(CustomFonts.SmallerSize, 'i');
                this.errorLabel.ForeColor = Color.Red;
                this.errorLabel.TextAlign = ContentAlignment.MiddleCenter;
                this.errorLabel.Size = this.errorLabel.PreferredSize;
                this.errorLabel.Location = new Point(this.backButton.Right - this.errorLabel.PreferredWidth, this.backButton.Bottom + 15);
                this.Controls.Add(this.errorLabel);
            }
        }
    }
}
