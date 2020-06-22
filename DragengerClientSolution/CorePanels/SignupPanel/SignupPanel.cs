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
using ServerConnections;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace CorePanels
{
    public partial class SignupPanel : Panel
    {
        private Form parent;
        private Label signUpLabel, awesomeLabel, nameLabel, usernameLabel, emailLabel, macAddressLabel;
        private LinkLabel policiesLinkLabel, bindDeviceLinkLabel;
        private TextBox nameBox, usernameBox, emailBox, macAddressBox;
        private Button signUpButton;
        private CheckBox policiesCheckBox;
        private System.Threading.Thread childThreadDB;

        public SignupPanel(Form parent)
        {
            this.parent = parent;
            this.Size = parent.ClientSize;
            this.BackColor = Color.FromArgb(221, 221, 221);
            this.LabelInitialize();
            this.TextBoxInitialize();
            this.ButtonInitialize();
            this.SetAllLocations();
            this.InitializeErrorShowers();
        }
        private void LabelInitialize()
        {
            this.signUpLabel = new Label();
            this.signUpLabel.Text = "LET'S SIGN UP";
            this.signUpLabel.Font = CustomFonts.BigBold;
            this.signUpLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.signUpLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(signUpLabel);

            this.awesomeLabel = new Label();
            this.awesomeLabel.Text = "it's awesome";
            this.awesomeLabel.Font = CustomFonts.New(CustomFonts.StylishFontName, 16.0f, 'b');
            this.awesomeLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.awesomeLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(awesomeLabel);

            this.nameLabel = new Label();
            this.nameLabel.Text = "Full Name";
            this.nameLabel.Font = CustomFonts.SmallBold;
            this.nameLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.nameLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(nameLabel);

            this.usernameLabel = new Label();
            this.usernameLabel.Text = "Username";
            this.usernameLabel.Font = CustomFonts.SmallBold;
            this.usernameLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.usernameLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(usernameLabel);

            this.emailLabel = new Label();
            this.emailLabel.Text = "Email";
            this.emailLabel.Font = CustomFonts.SmallBold;
            this.emailLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.emailLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(emailLabel);

            this.macAddressLabel = new Label();
            this.macAddressLabel.Text = "Device MAC";
            this.macAddressLabel.Font = CustomFonts.SmallBold;
            this.macAddressLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.macAddressLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(macAddressLabel);

            this.policiesCheckBox = new CheckBox();
            this.policiesCheckBox.Font = CustomFonts.Smallest;
            this.policiesCheckBox.ForeColor = Color.FromArgb(65, 65, 65);
            this.policiesCheckBox.CheckedChanged += delegate(Object sender, EventArgs e) { if (this.signUpButtonMessage.Visible) this.AllSignupConstraintsOk(); };
            this.Controls.Add(policiesCheckBox);

            this.policiesLinkLabel = new LinkLabel();
            this.policiesLinkLabel.Text = "Accept Terms && Policies";
            this.policiesLinkLabel.Font = CustomFonts.New(CustomFonts.Smallest.Size, 'u');
            this.policiesLinkLabel.LinkArea = new LinkArea(7,17);
            this.policiesLinkLabel.LinkClicked += delegate(Object sender, LinkLabelLinkClickedEventArgs e) { this.policiesCheckBox.Checked = false; ((LinkLabel)sender).LinkVisited = !((LinkLabel)sender).LinkVisited; this.ShowRulesRegulations(); };
            this.policiesLinkLabel.MouseClick += delegate(Object sender, MouseEventArgs e) { this.policiesCheckBox.Checked = !this.policiesCheckBox.Checked; };
            this.Controls.Add(policiesLinkLabel);

            this.bindDeviceLinkLabel = new LinkLabel();
            this.bindDeviceLinkLabel.Text = "Bind this device with\r\nan existing account";
            this.bindDeviceLinkLabel.TextAlign = ContentAlignment.MiddleRight;
            this.bindDeviceLinkLabel.Font = CustomFonts.Smallest;
            this.bindDeviceLinkLabel.LinkArea = new LinkArea(0, 16);
            this.bindDeviceLinkLabel.LinkClicked += delegate(Object sender, LinkLabelLinkClickedEventArgs e) { this.Visible = false; this.parent.Controls.Add(new DeviceBindPanel(this)); };
            this.Controls.Add(bindDeviceLinkLabel);
        }
        private void TextBoxInitialize()
        {
            this.nameBox = new TextBox();
            this.nameBox.Font = CustomFonts.BigBold;
            this.nameBox.BackColor = Color.FromArgb(190, 190, 190);
            this.nameBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.nameBox.BorderStyle = BorderStyle.None;
            this.nameBox.Size = new Size(this.Size.Width - (this.Size.Width / 3), nameBox.PreferredHeight);
            this.nameBox.TextChanged += new EventHandler(this.SetNameValidity);
            this.Controls.Add(nameBox);

            this.usernameBox = new TextBox();
            this.usernameBox.Font = CustomFonts.BigBold;
            this.usernameBox.BackColor = Color.FromArgb(190, 190, 190);
            this.usernameBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.usernameBox.BorderStyle = BorderStyle.None;
            this.usernameBox.Size = new Size(this.Size.Width - (this.Size.Width / 3), usernameBox.PreferredHeight);
            this.usernameBox.TextChanged += new EventHandler(this.StartUsernameChecking);
            this.Controls.Add(usernameBox);

            this.emailBox = new TextBox();
            this.emailBox.Font = CustomFonts.BigBold;
            this.emailBox.BackColor = Color.FromArgb(190, 190, 190);
            this.emailBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.emailBox.BorderStyle = BorderStyle.None;
            this.emailBox.Size = new Size(this.Size.Width - (this.Size.Width / 3), emailBox.PreferredHeight);
            this.emailBox.TextChanged += new EventHandler(this.StartEmailChecking);
            this.Controls.Add(emailBox);

            this.macAddressBox = new TextBox();
            this.macAddressBox.Text = Universal.SystemMACAddress;
            this.macAddressBox.Font = CustomFonts.BigBold;
            this.macAddressBox.BackColor = Color.FromArgb(190, 190, 190);
            this.macAddressBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.macAddressBox.BorderStyle = BorderStyle.None;
            this.macAddressBox.Size = new Size(this.Size.Width - (this.Size.Width / 3), emailBox.PreferredHeight);
            this.macAddressBox.ReadOnly = true;
            this.Controls.Add(macAddressBox);
        }

        private void ButtonInitialize()
        {
            this.signUpButton = new Button();
            this.signUpButton.Font = CustomFonts.SmallerBold;
            this.signUpButton.BackColor = Color.FromArgb(0, 0, 135);
            this.signUpButton.ForeColor = Color.FromArgb(210, 210, 210);
            this.signUpButton.FlatStyle = FlatStyle.Flat;
            this.signUpButton.FlatAppearance.BorderSize = 0;
            this.signUpButton.Text = "&Sign up";
            this.signUpButton.Click += new EventHandler(EventListener);
            this.Controls.Add(signUpButton);
        }

        private void SetAllLocations()
        {
            this.signUpLabel.SetBounds((this.Size.Width - this.signUpLabel.PreferredWidth) / 2, this.Size.Height / 12, signUpLabel.PreferredWidth, signUpLabel.PreferredHeight);
            this.awesomeLabel.SetBounds(this.signUpLabel.Left + (this.signUpLabel.PreferredWidth - this.awesomeLabel.PreferredWidth) / 2, this.signUpLabel.Bottom + 10, awesomeLabel.PreferredWidth, awesomeLabel.PreferredHeight);
            this.nameLabel.SetBounds((this.Size.Width - this.nameBox.Width) / 2 - 5, this.awesomeLabel.Bottom + 80, nameLabel.PreferredWidth, nameLabel.PreferredHeight);
            this.nameBox.Location = new Point(this.nameLabel.Left + 5, this.nameLabel.Bottom + 5);
            this.usernameLabel.SetBounds((this.Size.Width - this.usernameBox.Width) / 2 - 5, this.nameBox.Bottom + 20, usernameLabel.PreferredWidth, usernameLabel.PreferredHeight);
            this.usernameBox.Location = new Point(this.usernameLabel.Left + 5, this.usernameLabel.Bottom + 5);
            this.emailLabel.SetBounds((this.Size.Width - this.emailBox.Width) / 2 - 5, this.usernameBox.Bottom + 20, emailLabel.PreferredWidth, emailLabel.PreferredHeight);
            this.emailBox.Location = new Point(this.emailLabel.Left + 5, this.emailLabel.Bottom + 5);
            this.macAddressLabel.SetBounds((this.Size.Width - this.macAddressBox.Width) / 2 - 5, this.emailBox.Bottom + 20, macAddressLabel.PreferredWidth, macAddressLabel.PreferredHeight);
            this.macAddressBox.Location = new Point(this.macAddressLabel.Left + 5, this.macAddressLabel.Bottom + 5);
            this.signUpButton.SetBounds(this.macAddressBox.Right - signUpButton.PreferredSize.Width, this.macAddressBox.Bottom + 40, signUpButton.PreferredSize.Width, signUpButton.PreferredSize.Height);
            this.policiesCheckBox.SetBounds(this.macAddressBox.Left, this.signUpButton.Top + 3, this.policiesCheckBox.PreferredSize.Width, this.policiesCheckBox.PreferredSize.Height);
            this.policiesLinkLabel.SetBounds(this.policiesCheckBox.Right + 2, this.policiesCheckBox.Top, policiesLinkLabel.PreferredSize.Width, policiesLinkLabel.PreferredSize.Height);
            this.bindDeviceLinkLabel.SetBounds(this.signUpButton.Left - bindDeviceLinkLabel.PreferredSize.Width - 10, this.signUpButton.Top, bindDeviceLinkLabel.PreferredWidth, bindDeviceLinkLabel.PreferredHeight);
        }

        private void ShowRulesRegulations()
        {
            Universal.ShowInfoMessage(ConfigurationManager.AppSettings["rulesAndRegulations"]);
        }

        private void EventListener(Object sender, EventArgs e)
        {
            if (sender == this.signUpButton)
            {
                if (this.AllSignupConstraintsOk())
                {
                    VisualizingTools.ShowWaitingAnimation(new Point(this.signUpButton.Left, this.signUpButton.Bottom + 20), new Size(this.signUpButton.Width, this.signUpButton.Height / 2), this);
                    childThreadDB = new System.Threading.Thread(delegate ()
                        {
                            JObject signupData = new JObject();
                            signupData["type"]="consumer";
                            signupData["username"]=this.usernameBox.Text;
                            signupData["email"] = this.emailBox.Text;
                            signupData["name"] = Universal.NameValidator(this.nameBox.Text);
                            signupData["mac_address"] = Universal.SystemMACAddress;

                            long? userId = ServerRequest.SignupUser(signupData);

                            if (this.InvokeRequired) this.Invoke(new Action(() =>
                            {
                                if (userId != null)
                                {
                                    Universal.ParentForm.Controls.Remove(this);
                                    this.Dispose();
                                    VisualizingTools.HideWaitingAnimation();
                                }
                                else
                                {
                                    this.signUpButtonMessage.Text = "Error in connection!";
                                    VisualizingTools.HideWaitingAnimation();
                                }
                            }));
                            if (userId != null) BackendManager.LoginProcessRun();
                        });
                    childThreadDB.Start();
                }
            }
            else if (sender == this.policiesLinkLabel)
            {
                this.policiesLinkLabel.ForeColor = Color.FromArgb(106, 0, 154);
            }
        }
    }
}
