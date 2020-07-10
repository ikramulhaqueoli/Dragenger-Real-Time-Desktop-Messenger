using EntityLibrary;
using FileIOAccess;
using Newtonsoft.Json.Linq;
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

namespace CorePanels.SlideBar
{
    public abstract class ConsumerListPanel : Panel
    {
        protected Panel foundUserListPanel;
        protected Label backButtonLabel, searchIcon;
        protected int previousUserBottom;
        protected Panel parent;
        protected TextBox searchBox;

        protected void InitilizeFoundUserListPanel()
        {
            this.foundUserListPanel = new Panel();
            this.foundUserListPanel.Width = this.parent.Width - 10;
            this.foundUserListPanel.Top = this.searchBox.Bottom + 3;
            this.foundUserListPanel.Left = 3;
            this.Controls.Add(foundUserListPanel);
        }

        protected void ShowBackButtonLabel()
        {
            this.backButtonLabel = new Label();
            this.backButtonLabel.Name = "Back";
            this.backButtonLabel.Size = new Size(this.Width / 6, this.Height / 8);
            this.backButtonLabel.Location = new Point(3, 8);
            this.backButtonLabel.Image = new Bitmap(FileResources.Icon("back.png"), new Size(backButtonLabel.Height - 20, backButtonLabel.Height - 20));
            this.backButtonLabel.ImageAlign = ContentAlignment.MiddleCenter;
            this.backButtonLabel.MouseEnter += delegate (Object sender, EventArgs e) { ((Label)sender).BackColor = Color.FromArgb(((Label)sender).BackColor.R - 25, ((Label)sender).BackColor.G - 25, ((Label)sender).BackColor.B - 25); };
            this.backButtonLabel.MouseLeave += delegate (Object sender, EventArgs e) { ((Label)sender).BackColor = Color.FromArgb(((Label)sender).BackColor.R + 25, ((Label)sender).BackColor.G + 25, ((Label)sender).BackColor.B + 25); };
            this.backButtonLabel.Click += delegate (Object sender, EventArgs e) { this.parent.Controls.Remove(this); ((FriendListPanel)parent).RefreshFriendlist(); this.parent.Controls.Add(((FriendListPanel)parent).FriendListMainPanel); };
            this.Controls.Add(this.backButtonLabel);
        }

        protected void ShowHeadlineLabel(string heading)
        {
            Label headline = new Label();
            headline.Text = heading;
            headline.Font = CustomFonts.BigBold;
            headline.ForeColor = Color.FromArgb(95, 95, 95);
            headline.Size = headline.PreferredSize;
            headline.Location = new Point(this.backButtonLabel.Right + 5, 3 + (this.backButtonLabel.Bottom - headline.Height) / 2);
            this.Controls.Add(headline);
        }

        protected void ShowMatchedList(List<JObject> matchList)
        {
            previousUserBottom = 0;

            foreach (Control item in this.foundUserListPanel.Controls)
            {
                this.foundUserListPanel.Controls.Remove(item);
                item.Dispose();
            }

            if (matchList == null || matchList.Count == 0)
            {
                Label nothingLabel = new Label();
                nothingLabel.Font = CustomFonts.New(12, 'i');
                nothingLabel.Text = "No User Found!";
                nothingLabel.TextAlign = ContentAlignment.MiddleCenter;
                nothingLabel.Size = nothingLabel.PreferredSize;
                nothingLabel.Location = new Point((this.Width - nothingLabel.Width) / 2, 20);
                this.foundUserListPanel.Controls.Add(nothingLabel);
                return;
            }

            foreach (JObject consumerJson in matchList)
            {
                Consumer consumer = new Consumer(consumerJson);
                Panel singleUserPanel = new Panel();
                singleUserPanel.Width = this.foundUserListPanel.Width;
                singleUserPanel.BackColor = Colors.DragengerTileColor;

                Label userIconLabel = new Label();
                userIconLabel.Image = new Bitmap(consumer.ProfileImage, new Size(50, 50));
                userIconLabel.Size = userIconLabel.Image.Size;
                singleUserPanel.Height = userIconLabel.Height + 8;
                userIconLabel.Location = new Point(5, (singleUserPanel.Height - userIconLabel.Height) / 2);
                singleUserPanel.Controls.Add(userIconLabel);

                Label nameLabel = new Label();
                nameLabel.Text = consumer.Name;
                nameLabel.Font = CustomFonts.Smaller;
                nameLabel.Size = nameLabel.PreferredSize;
                nameLabel.Location = new Point(userIconLabel.Right + 5, 5);
                singleUserPanel.Controls.Add(nameLabel);

                Label usernameLabel = new Label();
                usernameLabel.Text = consumer.Username;
                usernameLabel.Font = CustomFonts.Smallest;
                usernameLabel.Size = usernameLabel.PreferredSize;
                usernameLabel.Location = new Point(userIconLabel.Right + 5, nameLabel.Bottom + 5);
                singleUserPanel.Controls.Add(usernameLabel);

                string friendRequestStatus = "";
                try { friendRequestStatus = consumerJson["frequest_status"].ToString(); }
                catch { }
                this.SetAddFriendButton(consumer, singleUserPanel, friendRequestStatus);

                singleUserPanel.Height = Math.Max(singleUserPanel.PreferredSize.Height, userIconLabel.Height) + 10;
                singleUserPanel.Top = previousUserBottom + 2;

                previousUserBottom = singleUserPanel.Bottom;

                userIconLabel.Click += (s, e) => { };
                userIconLabel.MouseEnter += (s, e) => { singleUserPanel.BackColor = Color.FromArgb(singleUserPanel.BackColor.R - 25, singleUserPanel.BackColor.G - 25, singleUserPanel.BackColor.B - 25); };
                userIconLabel.MouseLeave += (s, e) => { singleUserPanel.BackColor = Color.FromArgb(singleUserPanel.BackColor.R + 25, singleUserPanel.BackColor.G + 25, singleUserPanel.BackColor.B + 25); };

                nameLabel.Click += (s, e) => { };
                nameLabel.MouseEnter += (s, e) => { singleUserPanel.BackColor = Color.FromArgb(singleUserPanel.BackColor.R - 25, singleUserPanel.BackColor.G - 25, singleUserPanel.BackColor.B - 25); };
                nameLabel.MouseLeave += (s, e) => { singleUserPanel.BackColor = Color.FromArgb(singleUserPanel.BackColor.R + 25, singleUserPanel.BackColor.G + 25, singleUserPanel.BackColor.B + 25); };

                usernameLabel.Click += (s, e) => { };
                usernameLabel.MouseEnter += (s, e) => { singleUserPanel.BackColor = Color.FromArgb(singleUserPanel.BackColor.R - 25, singleUserPanel.BackColor.G - 25, singleUserPanel.BackColor.B - 25); };
                usernameLabel.MouseLeave += (s, e) => { singleUserPanel.BackColor = Color.FromArgb(singleUserPanel.BackColor.R + 25, singleUserPanel.BackColor.G + 25, singleUserPanel.BackColor.B + 25); };

                singleUserPanel.Click += (s, e) => { };
                singleUserPanel.MouseEnter += (s, e) => { ((Panel)s).BackColor = Color.FromArgb(((Panel)s).BackColor.R - 25, ((Panel)s).BackColor.G - 25, ((Panel)s).BackColor.B - 25); };
                singleUserPanel.MouseLeave += (s, e) => { ((Panel)s).BackColor = Color.FromArgb(((Panel)s).BackColor.R + 25, ((Panel)s).BackColor.G + 25, ((Panel)s).BackColor.B + 25); };

                this.foundUserListPanel.Controls.Add(singleUserPanel);
            }
            this.foundUserListPanel.Size = this.foundUserListPanel.PreferredSize;
        }
        public void SetAddFriendButton(Consumer consumer, Panel singleUserPanel, string friendRequestStatus)
        {
            if (singleUserPanel.Controls.ContainsKey("addFriendLabel"))
            {
                if (singleUserPanel.InvokeRequired) singleUserPanel.Invoke(new Action(() => { singleUserPanel.Controls.RemoveByKey("addFriendLabel"); }));
                else singleUserPanel.Controls.RemoveByKey("addFriendLabel");
            }
            if (singleUserPanel.Controls.ContainsKey("removeFriendlabel"))
            {
                if (singleUserPanel.InvokeRequired) singleUserPanel.Invoke(new Action(() => { singleUserPanel.Controls.RemoveByKey("removeFriendlabel"); }));
                else singleUserPanel.Controls.RemoveByKey("removeFriendlabel");
            }
            Label addButtonLabel = new Label();
            addButtonLabel.Name = "addFriendLabel";
            addButtonLabel.Image = new Bitmap(FileResources.Icon("add_friend.png"), new Size(singleUserPanel.Height / 2, singleUserPanel.Height / 2));
            addButtonLabel.Size = new Size(addButtonLabel.Image.Width + 5, addButtonLabel.Image.Width + 5);
            addButtonLabel.ImageAlign = ContentAlignment.MiddleCenter;
            addButtonLabel.Location = new Point(singleUserPanel.Right - addButtonLabel.Width - 20, (singleUserPanel.Height - addButtonLabel.Height) / 2);

            if (friendRequestStatus == null || friendRequestStatus.Length == 0)
            {
                addButtonLabel.Image = new Bitmap(FileResources.Icon("add_friend.png"), addButtonLabel.Image.Size);
                addButtonLabel.MouseEnter += delegate(Object sender, EventArgs me) { addButtonLabel.BackColor = Color.FromArgb(addButtonLabel.BackColor.R - 25, addButtonLabel.BackColor.G - 25, addButtonLabel.BackColor.B - 25); };
                addButtonLabel.MouseLeave += delegate(Object sender, EventArgs me) { addButtonLabel.BackColor = singleUserPanel.BackColor; };
                addButtonLabel.Click += delegate(Object sender, EventArgs me) { addButtonLabel.Visible = false; this.SendFriendRequestTo(consumer, addButtonLabel, singleUserPanel); };
            }
            else if (friendRequestStatus.Length > 0)
            {
                if (friendRequestStatus == "r_receiver")
                {
                    addButtonLabel.Image = new Bitmap(FileResources.Icon("add_friend_cancel.png"), addButtonLabel.Image.Size);
                    addButtonLabel.MouseEnter += delegate(Object sender, EventArgs me)
                    {
                        addButtonLabel.Image = new Bitmap(FileResources.Icon("add_friend_cancel.png"), addButtonLabel.Image.Size);
                    };
                    addButtonLabel.MouseLeave += delegate(Object sender, EventArgs me)
                    {
                        addButtonLabel.Image = new Bitmap(FileResources.Icon("add_friend_cancel_rev.png"), addButtonLabel.Image.Size);
                    };
                    addButtonLabel.Click += delegate(Object sender, EventArgs me) { addButtonLabel.Visible = false; this.CancelFriendRequestOf(consumer, addButtonLabel, singleUserPanel); };
                }
                else if (friendRequestStatus == "r_sender")
                {
                    addButtonLabel.Image = new Bitmap(FileResources.Icon("add_friend_accept.png"), addButtonLabel.Image.Size);
                    Label removeFriendLabel = new Label();
                    removeFriendLabel.Name = "removeFriendlabel";
                    removeFriendLabel.Image = new Bitmap(FileResources.Icon("add_friend_cancel.png"), addButtonLabel.Image.Size);
                    removeFriendLabel.Size = new Size(addButtonLabel.Image.Width + 5, addButtonLabel.Image.Width + 5);
                    removeFriendLabel.ImageAlign = ContentAlignment.MiddleCenter;
                    removeFriendLabel.Location = new Point(addButtonLabel.Left - removeFriendLabel.Width - 5, addButtonLabel.Top);
                    singleUserPanel.Controls.Add(removeFriendLabel);

                    addButtonLabel.Click += delegate(Object sender, EventArgs me) { addButtonLabel.Visible = false; this.AcceptFriendRequestOf(consumer, addButtonLabel, singleUserPanel); };
                    removeFriendLabel.Click += delegate(Object sender, EventArgs me) { addButtonLabel.Visible = false; removeFriendLabel.Visible = false; this.CancelFriendRequestOf(consumer, addButtonLabel, singleUserPanel); };

                    addButtonLabel.MouseEnter += delegate(Object sender, EventArgs me)
                    {
                        addButtonLabel.Image = new Bitmap(FileResources.Icon("add_friend_accept_rev.png"), addButtonLabel.Image.Size);
                    };
                    addButtonLabel.MouseLeave += delegate(Object sender, EventArgs me)
                    {
                        addButtonLabel.Image = new Bitmap(FileResources.Icon("add_friend_accept.png"), addButtonLabel.Image.Size);
                    };

                    removeFriendLabel.MouseEnter += delegate(Object sender, EventArgs me)
                    {
                        removeFriendLabel.Image = new Bitmap(FileResources.Icon("add_friend_cancel_rev.png"), removeFriendLabel.Image.Size);
                    };
                    removeFriendLabel.MouseLeave += delegate(Object sender, EventArgs me)
                    {
                        removeFriendLabel.Image = new Bitmap(FileResources.Icon("add_friend_cancel.png"), removeFriendLabel.Image.Size);
                    };
                }
                else if (friendRequestStatus == "friend")
                {
                    addButtonLabel.Image = new Bitmap(FileResources.Icon("add_friend_green.png"), addButtonLabel.Image.Size);
                }
            }

            if (singleUserPanel.InvokeRequired) singleUserPanel.Invoke(new Action(() => { singleUserPanel.Controls.Add(addButtonLabel); }));
            else singleUserPanel.Controls.Add(addButtonLabel);
        }

        private void SendFriendRequestTo(Consumer consumer, Label addButtonLabel, Panel singleUserPanel)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (s, e) =>
            {
                bool? success = ServerConnections.ServerRequest.SendFriendRequest(Consumer.LoggedIn.Id, consumer.Id);
                if (success == true)
                {
                    this.SetAddFriendButton(consumer, singleUserPanel, "r_receiver");
                }
                else
                {
                    if (success == null) MessageBox.Show("Database access denied!");
                }
            };
            backgroundWorker.RunWorkerCompleted += (s, e) => { backgroundWorker.Dispose(); };
            backgroundWorker.RunWorkerAsync();
        }

        private void CancelFriendRequestOf(Consumer consumer, Label addButtonLabel, Panel singleUserPanel)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (s, e) =>
            {
                bool? success = ServerRequest.DeleteFriendRequest(Consumer.LoggedIn.Id, consumer.Id);
                if (success == true)
                {
                    if (this.InvokeRequired) this.Invoke(new Action(() =>
                    {
                        this.SetAddFriendButton(consumer, singleUserPanel, "");
                    }));
                }
                else
                {
                    if (success == null) MessageBox.Show("Database access denied!");
                }
            };
            backgroundWorker.RunWorkerCompleted += (s, e) => { backgroundWorker.Dispose(); };
            backgroundWorker.RunWorkerAsync();
        }

        private void AcceptFriendRequestOf(Consumer consumer, Label addButtonLabel, Panel singleUserPanel)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (s, e) =>
            {
                bool? success = ServerRequest.AcceptFriendRequest(consumer.Id, Consumer.LoggedIn.Id);
                if (success == true)
                {
                    if (this.InvokeRequired) this.Invoke(new Action(() =>
                    {
                        this.SetAddFriendButton(consumer, singleUserPanel, "friend");
                    }));
                }
                else
                {
                    if (success == null) MessageBox.Show("Database access denied!");
                }
            };
            backgroundWorker.RunWorkerCompleted += (s, e) => { backgroundWorker.Dispose(); };
            backgroundWorker.RunWorkerAsync();

        }
    }
}
