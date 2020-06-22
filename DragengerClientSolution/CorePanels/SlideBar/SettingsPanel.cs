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
using System.Collections;
using ServerConnections;
using Newtonsoft.Json.Linq;

namespace CorePanels
{
    public class SettingsPanel : Panel
    {
        private Panel parent;
        private Panel userViewPanel, othersInfoEditPanel;
        private List<Label> userViewIcons;
        private Label userProfilePictureLabel;
        private TextBox fullnameTextBox, dragengerUsernameTextBox, emailTextBox, phoneTextBox, usernameTextbox, macAddressTextbox, currentPasswordTextBox;
        private DateTimePicker birthdatePicker;
        private ComboBox genderChooser;
        private Button saveButton, deleteAccountButton;
        public SettingsPanel(Panel parent)
        {
            this.parent = parent;
            this.Width = this.parent.Width;
            this.Height = this.parent.Height;
            this.BackColor = Color.FromArgb(200, 200, 200);

            this.UserViewPanelInitialize();
            this.othersInfoEditBoxInitialize();
        }

        public int CurrentOpenOptionIndex
        {
            set;
            get;
        }

        public Size ListPanelsSize
        {
            set;
            get;
        }

        private void UserViewPanelInitialize()
        {
            this.userViewPanel = new Panel();
            this.userViewPanel.BackColor = Color.FromArgb(0, 0, 80);
            this.userViewPanel.Width = this.Width - 4;
            this.userViewPanel.Location = new Point(2, 2);

            this.AddIconsToUserView();

            userProfilePictureLabel = new Label();
            userProfilePictureLabel.Image = this.ResizedProfileImage;
            userProfilePictureLabel.Location = new Point((this.userViewPanel.Width - userProfilePictureLabel.Image.Width) / 2, userViewIcons[0].Bottom + 20);
            userProfilePictureLabel.Size = userProfilePictureLabel.Image.Size;
            userProfilePictureLabel.MouseHover += delegate(Object sender, EventArgs e) { userProfilePictureLabel.Image = GraphicsStudio.Overlap(userProfilePictureLabel.Image, new Bitmap(FileResources.Picture("pictureChangeHint.png"), userProfilePictureLabel.Image.Size)); };
            userProfilePictureLabel.MouseLeave += delegate(Object sender, EventArgs e) { userProfilePictureLabel.Image = this.ResizedProfileImage; };
            userProfilePictureLabel.MouseDoubleClick += delegate(Object sender, MouseEventArgs e) { ((SlidebarPanel)this.parent).ChangeProfilePicture(); };
            this.userViewPanel.Controls.Add(userProfilePictureLabel);

            fullnameTextBox = new TextBox();
            fullnameTextBox.Text = this.UserProfileName;
            fullnameTextBox.Font = CustomFonts.SmallBold;
            fullnameTextBox.Height = fullnameTextBox.PreferredHeight;
            fullnameTextBox.TextAlign = HorizontalAlignment.Center;
            fullnameTextBox.Width = this.Width * 3/4;
            fullnameTextBox.BackColor = Color.FromArgb(userViewPanel.BackColor.R + 50, userViewPanel.BackColor.G + 50, userViewPanel.BackColor.B + 50);
            fullnameTextBox.Location = new Point((this.userViewPanel.Width - fullnameTextBox.Width) / 2, userProfilePictureLabel.Bottom + 20);
            fullnameTextBox.ForeColor = Color.FromArgb(240, 240, 240);
            this.userViewPanel.Controls.Add(fullnameTextBox);

            dragengerUsernameTextBox = new TextBox();
            dragengerUsernameTextBox.Text = "@"+User.LoggedIn.Username;
            dragengerUsernameTextBox.ReadOnly = true;
            dragengerUsernameTextBox.Font = CustomFonts.New(13.0f, 'i');
            dragengerUsernameTextBox.Width = fullnameTextBox.Width;
            dragengerUsernameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dragengerUsernameTextBox.TextAlign = HorizontalAlignment.Center;
            dragengerUsernameTextBox.BackColor = Color.FromArgb(userViewPanel.BackColor.R + 20, userViewPanel.BackColor.G + 20, userViewPanel.BackColor.B + 20);
            dragengerUsernameTextBox.Location = new Point((this.userViewPanel.Width - dragengerUsernameTextBox.Width) / 2, fullnameTextBox.Bottom + 5);
            dragengerUsernameTextBox.ForeColor = Color.FromArgb(240, 240, 240);
            this.userViewPanel.Controls.Add(dragengerUsernameTextBox);

            this.userViewPanel.Height = this.userViewPanel.PreferredSize.Height + 20;
            this.Controls.Add(this.userViewPanel);
        }


        private void othersInfoEditBoxInitialize()
        {
            this.othersInfoEditPanel = new Panel();
            this.othersInfoEditPanel.Width = this.Width;
            this.othersInfoEditPanel.Height = this.Height;

            Label settingsLabel = new Label();
            settingsLabel.Text = "Account Settings";
            settingsLabel.Font = CustomFonts.RegularBold;
            settingsLabel.Size = settingsLabel.PreferredSize;
            settingsLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.othersInfoEditPanel.Controls.Add(settingsLabel);

            Label macAddressLabel = new Label();
            macAddressLabel.Text = "Device MAC :";
            macAddressLabel.Font = CustomFonts.Smaller;
            macAddressLabel.Size = macAddressLabel.PreferredSize;
            macAddressLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.othersInfoEditPanel.Controls.Add(macAddressLabel);

            this.macAddressTextbox = new TextBox();
            this.macAddressTextbox.Text = Universal.SystemMACAddress;
            this.macAddressTextbox.ReadOnly = true;
            this.macAddressTextbox.BorderStyle = BorderStyle.None;
            this.macAddressTextbox.Font = CustomFonts.Small;
            this.macAddressTextbox.Width = this.Width * 4 / 5;
            this.macAddressTextbox.ForeColor = Color.FromArgb(77, 77, 77);
            this.macAddressTextbox.BackColor = this.BackColor;
            this.othersInfoEditPanel.Controls.Add(macAddressTextbox);

            Label usernameLabel = new Label();
            usernameLabel.Text = "Username :";
            usernameLabel.Font = CustomFonts.Smaller;
            usernameLabel.Size = usernameLabel.PreferredSize;
            usernameLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.othersInfoEditPanel.Controls.Add(usernameLabel);

            this.usernameTextbox = new TextBox();
            this.usernameTextbox.Text = Consumer.LoggedIn.Username;
            this.usernameTextbox.Font = CustomFonts.Small;
            this.usernameTextbox.BorderStyle = BorderStyle.None;
            this.usernameTextbox.Width = (this.Width * 5 / 6) - usernameLabel.Width;
            this.usernameTextbox.ForeColor = Color.FromArgb(77, 77, 77);
            this.usernameTextbox.TextChanged += (s, e) => { this.dragengerUsernameTextBox.Text = "@" + this.usernameTextbox.Text; };
            this.othersInfoEditPanel.Controls.Add(usernameTextbox);

            Label phoneLabel = new Label();
            phoneLabel.Text = "Phone :";
            phoneLabel.Font = CustomFonts.Smaller;
            phoneLabel.Size = phoneLabel.PreferredSize;
            phoneLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.othersInfoEditPanel.Controls.Add(phoneLabel);

            this.phoneTextBox = new TextBox();
            this.phoneTextBox.Text = Consumer.LoggedIn.Phone;
            this.phoneTextBox.Font = CustomFonts.Small;
            this.phoneTextBox.BorderStyle = BorderStyle.None;
            this.phoneTextBox.Width = (this.Width * 5 / 6) - phoneLabel.Width;
            this.phoneTextBox.ForeColor = Color.FromArgb(77, 77, 77);
            //this.phoneTextBox.KeyPress += (s, e) => { if(e.KeyChar >= '0' && e.KeyChar <= '9') };
            this.othersInfoEditPanel.Controls.Add(phoneTextBox);

            Label emailLabel = new Label();
            emailLabel.Text = "Email";
            emailLabel.Font = CustomFonts.Smaller;
            emailLabel.Size = emailLabel.PreferredSize;
            emailLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.othersInfoEditPanel.Controls.Add(emailLabel);

            this.emailTextBox = new TextBox();
            this.emailTextBox.Text = Consumer.LoggedIn.Email;
            this.emailTextBox.ReadOnly = true;                  //changing email address is temporarily disabled
            this.emailTextBox.Font = CustomFonts.Small;
            this.emailTextBox.BorderStyle = BorderStyle.None;
            this.emailTextBox.Width = this.Width * 5 / 6;
            this.emailTextBox.ForeColor = Color.FromArgb(77, 77, 77);
            this.othersInfoEditPanel.Controls.Add(emailTextBox);

            Label genderLabel = new Label();
            genderLabel.Text = "Gender";
            genderLabel.Font = CustomFonts.Smaller;
            genderLabel.ForeColor = Color.FromArgb(77, 77, 77);
            genderLabel.Size = genderLabel.PreferredSize;
            this.othersInfoEditPanel.Controls.Add(genderLabel);

            genderChooser = new ComboBox();
            genderChooser.Font = CustomFonts.Smallest;
            string[] itemList = new string[] {"", "Male", "Female", "Others"};
            foreach (string str in itemList) genderChooser.Items.Add(str);
            if (Consumer.LoggedIn.GenderIndex > 0) genderChooser.SelectedIndex = (int)Consumer.LoggedIn.GenderIndex;
            genderChooser.DropDownStyle = ComboBoxStyle.DropDownList;
            this.othersInfoEditPanel.Controls.Add(genderChooser);

            Label birthdateLabel = new Label();
            birthdateLabel.Text = "Birth Date";
            birthdateLabel.Font = CustomFonts.Smaller;
            birthdateLabel.Size = birthdateLabel.PreferredSize;
            birthdateLabel.ForeColor = Color.FromArgb(77, 77, 77);
            this.othersInfoEditPanel.Controls.Add(birthdateLabel);

            birthdatePicker = new DateTimePicker();
            if (Consumer.LoggedIn.Birthdate != null)
            {
                birthdatePicker.Value = Convert.ToDateTime(Consumer.LoggedIn.Birthdate.DbFormat);
                birthdateLabel.Text = "Birth Date";
            }
            else birthdateLabel.Text = "Birth Date (not set)";
            birthdateLabel.Size = birthdateLabel.PreferredSize;
            birthdatePicker.Font = CustomFonts.Smaller;
            this.othersInfoEditPanel.Controls.Add(birthdatePicker);

            Label oldPasswordLabel = new Label();
            oldPasswordLabel.Text = "Current Password";
            oldPasswordLabel.Font = CustomFonts.Smaller;
            oldPasswordLabel.ForeColor = Color.FromArgb(77, 77, 77);
            oldPasswordLabel.Size = oldPasswordLabel.PreferredSize;
            this.othersInfoEditPanel.Controls.Add(oldPasswordLabel);

            currentPasswordTextBox = new TextBox();
            currentPasswordTextBox.PasswordChar = '•';
            currentPasswordTextBox.ForeColor = Color.FromArgb(77, 77, 77);
            currentPasswordTextBox.Font = CustomFonts.Smallest;
            currentPasswordTextBox.Size = currentPasswordTextBox.PreferredSize;
            this.othersInfoEditPanel.Controls.Add(currentPasswordTextBox);

            this.Controls.Add(othersInfoEditPanel);
            this.othersInfoEditPanel.Location = new Point(0, userViewPanel.Bottom + 4);

            //initializing sizes
            settingsLabel.Location = new Point((this.Width - settingsLabel.Width) / 2, settingsLabel.Height / 2);

            emailTextBox.Left = (this.Width - emailTextBox.Width) / 2;
            usernameLabel.Left = emailTextBox.Left - 3;
            usernameTextbox.Left = usernameLabel.Right + 10;
            macAddressLabel.Left = emailTextBox.Left - 3;
            macAddressTextbox.Left = macAddressLabel.Right + 10;
            phoneLabel.Left = emailTextBox.Left - 3;
            phoneTextBox.Left = phoneLabel.Right + 10;
            emailLabel.Left = emailTextBox.Left - 3;
            genderLabel.Left = emailTextBox.Left - 3;
            genderChooser.Left = genderLabel.Right + 10;
            birthdateLabel.Left = emailTextBox.Left - 3;
            birthdatePicker.Left = birthdateLabel.Right + 5;
            oldPasswordLabel.Left = emailTextBox.Left - 3;
            currentPasswordTextBox.Left = oldPasswordLabel.Right + 5;

            macAddressLabel.Top = settingsLabel.Bottom + 20; 
            macAddressTextbox.Top = macAddressLabel.Top; 

            usernameLabel.Top = macAddressTextbox.Bottom + 15;
            usernameTextbox.Top = usernameLabel.Top;

            phoneLabel.Top = usernameTextbox.Bottom + 15;
            phoneTextBox.Top = phoneLabel.Top;

            emailLabel.Top = phoneTextBox.Bottom + 15; 
            emailTextBox.Top = emailLabel.Bottom + 5; 

            genderLabel.Top = emailTextBox.Bottom + 15;
            genderChooser.Top = genderLabel.Top;

            birthdateLabel.Top = genderChooser.Bottom + 15;
            birthdatePicker.Top = birthdateLabel.Top;

            oldPasswordLabel.Top = birthdatePicker.Bottom + 15;
            currentPasswordTextBox.Top = oldPasswordLabel.Top;
            currentPasswordTextBox.Width = emailTextBox.Right - oldPasswordLabel.Right;

            //buttons initialize

            saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.BackColor = Color.FromArgb(0,0,102);
            saveButton.ForeColor = Color.FromArgb(234,234,234);
            saveButton.Font = CustomFonts.SmallerBold;
            saveButton.Size = saveButton.PreferredSize;
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.Left = this.currentPasswordTextBox.Right - saveButton.Width;
            saveButton.MouseEnter += (s, e) => { saveButton.BackColor = Color.FromArgb(saveButton.BackColor.R + 50, saveButton.BackColor.G + 50, saveButton.BackColor.B + 50); };
            saveButton.MouseLeave += (s, e) => { saveButton.BackColor = Color.FromArgb(saveButton.BackColor.R - 50, saveButton.BackColor.G - 50, saveButton.BackColor.B - 50); };
            saveButton.Click += (sender, eargs) => 
            {
                try
                {
                    string name = this.fullnameTextBox.Text, phone = phoneTextBox.Text, username = usernameTextbox.Text, email = emailTextBox.Text, inputPassword = currentPasswordTextBox.Text;
                    Time birthdate = new Time(birthdatePicker.Value);
                    string nameError = null, usernameError = null, emailError = null, genderError = null, birthdateError = null, passwordError = null;
                    nameError = StandardAssuranceLibrary.Checker.CheckNameValidity(name);
                    if (username != Consumer.LoggedIn.Username) usernameError = StandardAssuranceLibrary.Checker.CheckUsernameValidity(ref username);
                    if (email != Consumer.LoggedIn.Email) emailError = StandardAssuranceLibrary.Checker.CheckEmailValidity(ref email);
                    if (genderChooser.SelectedIndex == -1 || genderChooser.SelectedIndex == 0) genderError = "No gender is choosen.";
                    if (Time.TimeDistanceInMinute(birthdate, Time.CurrentTime) < 6837340) birthdateError = "Less than 13 years old are not allowed.";

                    JObject consumerJson = new JObject();
                    consumerJson["id"] = Consumer.LoggedIn.Id;
                    consumerJson["name"] = name;
                    consumerJson["username"] = username;
                    consumerJson["email"] = email;
                    consumerJson["phone"] = phone;
                    consumerJson["birthdate"] = birthdate.TimeStampString;
                    consumerJson["gender_index"] = genderChooser.SelectedIndex;
                    if (Consumer.LoggedIn.ProfileImageId.Length > 5) consumerJson["profile_img_id"] = Consumer.LoggedIn.ProfileImageId;

                    BackgroundWorker loaderWorker = new BackgroundWorker();
                    loaderWorker.DoWork += (s, e) =>
                    {
                        passwordError = StandardAssuranceLibrary.Checker.CheckOldPasswordMatch(inputPassword);
                        string errorMessage = passwordError + "\r\n" + nameError + "\r\n" + usernameError + "\r\n" + emailError + "\r\n" + genderError + "\r\n" + birthdateError + "\r\n";
                        string errorSum = passwordError + usernameError + nameError + emailError + genderError + birthdateError;
                        if (errorSum.Length < 4)
                        {
                            Consumer consumer = new Consumer(consumerJson);
                            bool? success = ServerRequest.UpdateUserInfo(consumer);
                            if (success == true)
                            {
                                Consumer.LoggedIn = consumer;
                                Universal.ShowInfoMessage("The informations have been updated successful!", "Updated successfully!");
                                Universal.ParentForm.Invoke(new Action(() =>
                                {
                                    currentPasswordTextBox.Text = "";
                                }));
                            }
                            else
                            {
                                Universal.ShowErrorMessage("Error encountered while updating the informations.", "Update unsuccessful!");
                                Universal.ParentForm.Invoke(new Action(() =>
                                {
                                    currentPasswordTextBox.Text = "";
                                }));
                            }
                        }
                        else
                        {
                            Universal.ShowErrorMessage(errorMessage);
                        }
                    };
                    loaderWorker.RunWorkerAsync();
                    loaderWorker.RunWorkerCompleted += (s, e) => { loaderWorker.Dispose(); };
                }
                catch (Exception ex) { Console.WriteLine("Exception in UpdateUser() => " + ex.Message); }
            };
            this.othersInfoEditPanel.Controls.Add(saveButton);

            deleteAccountButton = new Button();
            deleteAccountButton.Text = "Delete Account";
            deleteAccountButton.BackColor = Color.FromArgb(153, 0, 0);
            deleteAccountButton.ForeColor = Color.FromArgb(234, 234, 234);
            deleteAccountButton.Font = CustomFonts.SmallerBold;
            deleteAccountButton.Size = deleteAccountButton.PreferredSize;
            deleteAccountButton.FlatStyle = FlatStyle.Flat;
            deleteAccountButton.Left = oldPasswordLabel.Left;
            deleteAccountButton.MouseEnter += (s, e) => { deleteAccountButton.BackColor = Color.FromArgb(deleteAccountButton.BackColor.R + 50, deleteAccountButton.BackColor.G + 50, deleteAccountButton.BackColor.B + 50); };
            deleteAccountButton.MouseLeave += (s, e) => { deleteAccountButton.BackColor = Color.FromArgb(deleteAccountButton.BackColor.R - 50, deleteAccountButton.BackColor.G - 50, deleteAccountButton.BackColor.B - 50); };
            deleteAccountButton.Click += (sender, eargs) => 
            {
                DialogResult result = MessageBox.Show("Are you sure to delete your account?\r\nAll your messages, conversations, friendlist and everything belongs to you will be deleted.\r\nAnd this can't be undone!", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly);
                if(result == DialogResult.Yes)
                {
                    if(this.currentPasswordTextBox.Text == null || this.currentPasswordTextBox.Text.Length <= 6)
                    {
                        Universal.ShowErrorMessage("Please enter your valid current password.", "Deletion unsuccessful!");
                        return;
                    }
                    BackgroundWorker loaderWorker = new BackgroundWorker();
                    loaderWorker.DoWork += (s, e) =>
                    {
                        try
                        {
                            string operationMessage = ServerRequest.DeleteConsumerAccount(Universal.SystemMACAddress, this.currentPasswordTextBox.Text);
                            if (operationMessage == null || operationMessage.Length == 0)
                            {
                                Universal.ShowErrorMessage("Connection to the server failed!", "Deletion unsuccessful!");
                            }
                            if (operationMessage == "success")
                            {
                                Universal.ShowInfoMessage("Your account has been deleted!", "Deletion successful!");
                                BackendManager.Logout();
                            }
                            else Universal.ShowErrorMessage(operationMessage, "Deletion unsuccessful!");
                        }
                        catch { }
                    };
                    loaderWorker.RunWorkerAsync();
                    loaderWorker.RunWorkerCompleted += (s, e) => { loaderWorker.Dispose(); };
                }

            };
            this.othersInfoEditPanel.Controls.Add(deleteAccountButton);

            Label cancelButtonLabel = new Label();
            cancelButtonLabel.Text = "Cancel && Back";
            cancelButtonLabel.BackColor = Color.FromArgb(this.BackColor.R - 20, this.BackColor.G - 20, this.BackColor.B - 20);
            cancelButtonLabel.TextAlign = ContentAlignment.MiddleCenter;
            cancelButtonLabel.Font = CustomFonts.SmallerBold;
            cancelButtonLabel.Width = this.Width;
            cancelButtonLabel.Height = cancelButtonLabel.PreferredHeight * 5/2;
            cancelButtonLabel.Top = this.Bottom - this.userViewPanel.Height - cancelButtonLabel.Height;
            cancelButtonLabel.MouseEnter += (s, e) => { cancelButtonLabel.BackColor = Color.FromArgb(cancelButtonLabel.BackColor.R - 30, cancelButtonLabel.BackColor.G - 30, cancelButtonLabel.BackColor.B - 30); };
            cancelButtonLabel.MouseLeave += (s, e) => { cancelButtonLabel.BackColor = Color.FromArgb(cancelButtonLabel.BackColor.R + 30, cancelButtonLabel.BackColor.G + 30, cancelButtonLabel.BackColor.B + 30); };
            cancelButtonLabel.Click += (s, e) =>
            {
                SlidebarPanel.MySidebarPanel.Controls.Remove(this);
                this.Dispose();
                SlidebarPanel.MySidebarPanel.Controls.Add(SlidebarPanel.MySidebarPanel.MainPanelInSidebar);
            };

            deleteAccountButton.Top = cancelButtonLabel.Top - deleteAccountButton.Height - 10;
            saveButton.Top = deleteAccountButton.Top;

            LinkLabel changePasswordLink = new LinkLabel();
            changePasswordLink.Text = "or, Change Password";
            changePasswordLink.Font = CustomFonts.Smaller;
            changePasswordLink.LinkArea = new LinkArea(4, changePasswordLink.Text.Length - 4);
            changePasswordLink.Size = changePasswordLink.PreferredSize;
            changePasswordLink.Left = oldPasswordLabel.Left;
            changePasswordLink.Top = deleteAccountButton.Top - changePasswordLink.Height - 10;
            changePasswordLink.LinkClicked += (s, e) =>
            {
                if (ConversationPanel.CurrentDisplayedConversationPanel != null)
                {
                    ConversationPanel.CurrentDisplayedConversationPanel.Visible = false; SlidebarPanel.MySidebarPanel.Visible = false;
                }
                (new LoginPanel(Universal.ParentForm, true, true)).ShowPasswordSetupPanel();
            };
            this.othersInfoEditPanel.Controls.Add(changePasswordLink);

            LinkLabel UnbindDeviceLink = new LinkLabel();
            UnbindDeviceLink.Text = "&Unbind Device";
            UnbindDeviceLink.Font = CustomFonts.Smaller;
            UnbindDeviceLink.LinkArea = new LinkArea(0, UnbindDeviceLink.Text.Length);
            UnbindDeviceLink.Size = UnbindDeviceLink.PreferredSize;
            UnbindDeviceLink.Left = this.othersInfoEditPanel.Right - UnbindDeviceLink.Width - (changePasswordLink.Left - this.othersInfoEditPanel.Left);
            UnbindDeviceLink.Top = changePasswordLink.Top;
            UnbindDeviceLink.LinkClicked += (sender, eargs) =>
            {
                string passwordError = StandardAssuranceLibrary.Checker.CheckOldPasswordMatch(this.currentPasswordTextBox.Text);
                if(passwordError != null && passwordError.Length > 0)
                {
                    Universal.ShowErrorMessage(passwordError, "Error!");
                    return;
                }

                DialogResult result = MessageBox.Show("Are you sure to unbind this device from this account?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly);
                if (result == DialogResult.Yes)
                {
                    BackgroundWorker loaderWorker = new BackgroundWorker();
                    loaderWorker.DoWork += (s, e) =>
                    {
                        bool? success = ServerRequest.UnbindDeviceFromAccount();
                        if(success == true)
                        {
                            Universal.ShowInfoMessage("This device has been unbound\r\nsuccessfullly from user account '" + User.LoggedIn.Username + "'!", "Unbound successfull!");
                            BackendManager.Logout();
                        }
                        else
                        {
                            string operationMessage = "Sorry! The device unbound process has been failed!";
                            if (success == null) operationMessage = "Server Connection Failed!";
                            Universal.ShowErrorMessage(operationMessage, "Unbound unsuccessful!");
                        }
                    };
                    loaderWorker.RunWorkerAsync();
                    loaderWorker.RunWorkerCompleted += (s, e) => { loaderWorker.Dispose(); };
                }
            };
            this.othersInfoEditPanel.Controls.Add(UnbindDeviceLink);

            this.othersInfoEditPanel.Controls.Add(cancelButtonLabel);
        }

        private void AddIconsToUserView()
        {
            this.userViewIcons = new List<Label>(3);

            this.userViewIcons.Add(new Label());

            Label lastLabel = null;
            for (int i = 1; i < this.userViewIcons.Capacity; i++)
            {
                Label item = new Label();
                item.Name = String.Format("userViewIcon{0}", i);
                item.Image = new Bitmap(FileResources.Icon(String.Format("userViewIcon{0}.png", i)), new Size(this.userViewPanel.Width / 20, userViewPanel.Width / 20));
                item.Size = item.Image.Size;
                if (lastLabel != null) item.Left = lastLabel.Left - item.Width - item.Width / 2;
                else item.Left = this.userViewPanel.Width - item.Width * 2;
                item.Top = item.Width;
                lastLabel = item;
                item.Click += new EventHandler(OnClick);
                item.MouseEnter += new EventHandler(OnMouseEnter);
                item.MouseLeave += new EventHandler(OnMouseLeave);
                this.userViewPanel.Controls.Add(item);
                this.userViewIcons.Add(item);
            }

            //for sidebar closing button
            int index = 0;
            this.userViewIcons[index].Name = String.Format("userViewIcon{0}", index);
            this.userViewIcons[index].Image = new Bitmap(FileResources.Icon(String.Format("userViewIcon{0}.png", index)), new Size(this.userViewPanel.Width / 20, this.userViewPanel.Height / 3));
            this.userViewIcons[index].Size = new Size(this.userViewIcons[index].Image.Width * 5 / 2, this.userViewIcons[index].Image.Height + this.userViewIcons[index].Image.Height / 2);
            this.userViewIcons[index].ImageAlign = ContentAlignment.MiddleCenter;
            this.userViewIcons[index].BackColor = this.userViewPanel.BackColor;
            this.userViewIcons[index].Left = this.userViewPanel.Right - this.userViewIcons[1].Right;
            this.userViewIcons[index].Top = this.userViewIcons[1].Top;
            this.userViewIcons[index].Click += new EventHandler(OnClick);
            this.userViewIcons[index].MouseEnter += new EventHandler(OnMouseEnter);
            this.userViewIcons[index].MouseLeave += new EventHandler(OnMouseLeave);
            this.userViewPanel.Controls.Add(this.userViewIcons[index]);
            this.userViewIcons.Add(this.userViewIcons[index]);
        }

        private void OnClick(object sender, EventArgs e)
        {
            if(sender == this.userViewIcons[0])
            {
                SlidebarPanel.MySidebarPanel.ChangeState();
            }
            else if (sender == this.userViewIcons[1])
            {
                BackendManager.Logout();
            }
            else if (sender == this.userViewIcons[2])
            {
                SlidebarPanel.MySidebarPanel.Controls.Remove(this);
                this.Dispose();
                SlidebarPanel.MySidebarPanel.Controls.Add(SlidebarPanel.MySidebarPanel.MainPanelInSidebar);
            }
        }

        private string UserProfileName
        {
            get { return Consumer.LoggedIn.Name; }
        }

        private Image ResizedProfileImage
        {
            get
            {
                return new Bitmap(Consumer.LoggedIn.ProfileImage, new Size(this.userViewPanel.Width / 3, userViewPanel.Width / 3));
            }
        }

        private string FetchDragengerEmail
        {
            get { return Consumer.LoggedIn.DragengerEmail; }
        }

        private void OnMouseEnter(Object sender, EventArgs me)
        {
            if (sender == userViewIcons[0])
            {
                this.userViewIcons[0].BackColor = Color.FromArgb(this.userViewIcons[0].BackColor.R + 50, this.userViewIcons[0].BackColor.G + 50, this.userViewIcons[0].BackColor.B + 50);
            }
            for (int i = 0; i < userViewIcons.Count; i++)
            {
                if (sender == userViewIcons[i])
                {
                    this.userViewIcons[i].Image = new Bitmap(FileResources.Icon(String.Format("userViewIcon{0}rev.png", i)), this.userViewIcons[i].Image.Size);
                    return;
                }
            }
        }

        private void OnMouseLeave(Object sender, EventArgs me)
        {
            if (sender == userViewIcons[0])
            {
                this.userViewIcons[0].BackColor = Color.FromArgb(this.userViewIcons[0].BackColor.R - 50, this.userViewIcons[0].BackColor.G - 50, this.userViewIcons[0].BackColor.B - 50);
            }
            for (int i = 0; i < userViewIcons.Count; i++)
            {
                if (sender == userViewIcons[i])
                {
                    this.userViewIcons[i].Image = new Bitmap(FileResources.Icon(String.Format("userViewIcon{0}.png", i)), this.userViewIcons[i].Image.Size);
                    return;
                }
            }
        }
    }

}
