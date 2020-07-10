using EntityLibrary;
using FileIOAccess;
using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CorePanels.SlideBar;
using System.ComponentModel;
using ServerConnections;

namespace CorePanels
{
    class FriendListPanel : Panel
    {
        private Panel parent, friendsListMainPanel;
        private Panel friendCarryListPanel;
        private List<Label> notFriendOptionLabels;
        private int previousFriendBottom;

        private SortedList<long, Panel> singleFriendPanelList;
        private SortedList<long, Label> singleFriendIconLabelList, singleFriendNameLabelList, singleFriendUsernameLabelList;

        private TextBox searchBox;
        private Label searchIcon;

        public FriendListPanel(Panel parent)
        {
            this.parent = parent;
            this.friendsListMainPanel = new Panel();
            this.friendsListMainPanel.Size = ((SlidebarPanel)this.parent).ListPanelsSize;
            this.BackColor = Color.FromArgb(176, 176, 176);
            this.ShowNotFriendOptions();
            this.ShowSearchBar();
            this.InitializeFriendListPanel();
            this.Controls.Add(friendsListMainPanel);
            this.RefreshFriendlist();               //testing
        }

        internal void RefreshFriendlist()
        {
            try
            {
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += (s, e) =>
                {
                    List<Consumer> friendList = ServerRequest.GetTop20FriendListOf(Consumer.LoggedIn.Id, "");
                    Universal.ParentForm.Invoke(new Action(() => { this.ShowFriendsInPanel(friendList); }));
                };
                backgroundWorker.RunWorkerCompleted += (s, e) => { backgroundWorker.Dispose(); };
                backgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }

        private void InitializeFriendListPanel()
        {
            this.friendCarryListPanel = new Panel();
            this.friendCarryListPanel.Width = this.parent.Width - 10;
            this.friendCarryListPanel.Top = this.searchBox.Bottom + 3;
            this.friendCarryListPanel.Left = 3;
            this.friendsListMainPanel.Controls.Add(this.friendCarryListPanel);
        }

        internal Panel FriendListMainPanel
        {
            get { return this.friendsListMainPanel; }
        }

        private void ShowNotFriendOptions()
        {
            this.notFriendOptionLabels = new List<Label>();
            this.notFriendOptionLabels.Add(new Label());
            this.notFriendOptionLabels.Add(new Label());
            this.notFriendOptionLabels.Add(new Label());

            Size originalImgSize = new Size(200, 77);
            float buttonWidth = (this.parent.Width - 20) / 3, buttonHeight = originalImgSize.Height * (buttonWidth / (float)originalImgSize.Width);

            this.notFriendOptionLabels[0].Name = "AddFriend";
            this.notFriendOptionLabels[0].Image = new Bitmap(FileResources.Icon("addFriend.png"), new Size((int)buttonWidth, (int)buttonHeight));
            this.notFriendOptionLabels[0].Size = new Size((int)buttonWidth, this.notFriendOptionLabels[0].Image.Height);
            this.notFriendOptionLabels[0].BackColor = Color.FromArgb(234,234,234);
            this.notFriendOptionLabels[0].ImageAlign = ContentAlignment.MiddleCenter;
            this.notFriendOptionLabels[0].Location = new Point(3, 6);
            this.notFriendOptionLabels[0].MouseEnter += delegate(Object sender, EventArgs e) { ((Label)sender).BackColor = Color.FromArgb(((Label)sender).BackColor.R - 25, ((Label)sender).BackColor.G - 25, ((Label)sender).BackColor.B - 25); };
            this.notFriendOptionLabels[0].MouseLeave += delegate(Object sender, EventArgs e) { ((Label)sender).BackColor = Color.FromArgb(((Label)sender).BackColor.R + 25, ((Label)sender).BackColor.G + 25, ((Label)sender).BackColor.B + 25); };
            this.notFriendOptionLabels[0].Click += delegate(Object sender, EventArgs e) { this.Controls.Remove(this.friendsListMainPanel); this.Controls.Add(new AddFriendPanel(this)); };
            this.friendsListMainPanel.Controls.Add(this.notFriendOptionLabels[0]);

            this.notFriendOptionLabels[1].Name = "FriendRequests";
            this.notFriendOptionLabels[1].Image = new Bitmap(FileResources.Icon("friendRequests.png"), new Size((int)buttonWidth, (int)buttonHeight));
            this.notFriendOptionLabels[1].Size = new Size((int)buttonWidth, this.notFriendOptionLabels[1].Image.Height);
            this.notFriendOptionLabels[1].Location = new Point(this.notFriendOptionLabels[0].Right + 5, 6);
            this.notFriendOptionLabels[1].BackColor = Color.FromArgb(234, 234, 234);
            this.notFriendOptionLabels[1].ImageAlign = ContentAlignment.MiddleCenter;
            this.notFriendOptionLabels[1].MouseEnter += delegate(Object sender, EventArgs e) { ((Label)sender).BackColor = Color.FromArgb(((Label)sender).BackColor.R - 25, ((Label)sender).BackColor.G - 25, ((Label)sender).BackColor.B - 25); };
            this.notFriendOptionLabels[1].MouseLeave += delegate(Object sender, EventArgs e) { ((Label)sender).BackColor = Color.FromArgb(((Label)sender).BackColor.R + 25, ((Label)sender).BackColor.G + 25, ((Label)sender).BackColor.B + 25); };
            this.notFriendOptionLabels[1].Click += delegate(Object sender, EventArgs e) { this.Controls.Remove(this.friendsListMainPanel); this.Controls.Add(new FriendRequestsPanel(this)); };
            this.friendsListMainPanel.Controls.Add(this.notFriendOptionLabels[1]);

            this.notFriendOptionLabels[2].Name = "BlockedPersons";
            this.notFriendOptionLabels[2].Image = new Bitmap(FileResources.Icon("blockedPersons.png"), new Size((int)buttonWidth, (int)buttonHeight));
            this.notFriendOptionLabels[2].Size = new Size((int)buttonWidth, this.notFriendOptionLabels[2].Image.Height);
            this.notFriendOptionLabels[2].Location = new Point(this.notFriendOptionLabels[1].Right + 5, 6);
            this.notFriendOptionLabels[2].BackColor = Color.FromArgb(234, 234, 234);
            this.notFriendOptionLabels[2].ImageAlign = ContentAlignment.MiddleCenter;
            this.notFriendOptionLabels[2].MouseEnter += delegate(Object sender, EventArgs e) { ((Label)sender).BackColor = Color.FromArgb(((Label)sender).BackColor.R - 25, ((Label)sender).BackColor.G - 25, ((Label)sender).BackColor.B - 25); };
            this.notFriendOptionLabels[2].MouseLeave += delegate(Object sender, EventArgs e) { ((Label)sender).BackColor = Color.FromArgb(((Label)sender).BackColor.R + 25, ((Label)sender).BackColor.G + 25, ((Label)sender).BackColor.B + 25); };
            this.friendsListMainPanel.Controls.Add(this.notFriendOptionLabels[2]);
        }

        private void ShowSearchBar()
        {
            this.searchBox = new TextBox();
            this.searchBox.Font = CustomFonts.Small;
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchBox.Height = this.searchBox.PreferredHeight;
            this.searchBox.Text = "Search friends";
            this.searchBox.ForeColor = Color.FromArgb(150, 150, 150);
            this.searchBox.Font = CustomFonts.New(CustomFonts.SmallSize, 'I');
            this.searchBox.Top = this.notFriendOptionLabels[0].Bottom + 5;
            this.searchBox.GotFocus += new EventHandler(OnGotFocus);
            this.searchBox.LostFocus += new EventHandler(OnLostFocus);
            this.searchBox.TextChanged += new EventHandler(OnTextChanged);
            this.friendsListMainPanel.Controls.Add(this.searchBox);

            searchIcon = new Label();
            searchIcon.Image = new Bitmap(FileResources.Icon("searchIcon.png"), new Size(this.searchBox.Height * 3 / 4, this.searchBox.Height * 3 / 4));
            searchIcon.Size = new Size(this.searchBox.Height, this.searchBox.Height);
            searchIcon.BackColor = Color.White;
            searchIcon.Location = new Point(3, this.searchBox.Top + (this.searchBox.Height - searchIcon.Height)/2);
            this.searchBox.Left = searchIcon.Right;
            this.searchBox.Width = this.parent.Width - searchIcon.Width - searchIcon.Left - 8;
            this.friendsListMainPanel.Controls.Add(searchIcon);
        }

        private void OnTextChanged(object sender, EventArgs me)
        {
            try
            {
                string keyword = ((TextBox)sender).Text;
                if (keyword.Length >= 2)
                {
                    VisualizingTools.ShowWaitingAnimation(new Point(this.searchIcon.Left, this.searchBox.Bottom + 5), new Size(this.searchIcon.Width + this.searchBox.Width, this.searchBox.Height / 2), this);
                    BackgroundWorker backgroundWorker = new BackgroundWorker();
                    backgroundWorker.DoWork += (s, e) =>
                    {
                        List<Consumer> matchList = ServerRequest.SearchFriendsByKeyword(Consumer.LoggedIn.Id, keyword);
                        if (matchList.Count > 0)
                        {
                            this.Invoke(new Action(() => { ShowFriendsInPanel(matchList); }));
                        }
                    };
                    backgroundWorker.RunWorkerCompleted += (s, e) => 
                    {
                        backgroundWorker.Dispose();
                        VisualizingTools.HideWaitingAnimation();
                    };
                    backgroundWorker.RunWorkerAsync();
                }
                else RefreshFriendlist();
            }
            catch (Exception ex) { Console.WriteLine("Error in firendlist search box : " + ex.Message); }
        }

        private void ShowFriendsInPanel(List<Consumer> friendList)
        {
            previousFriendBottom = 0;
            if (this.singleFriendPanelList == null) this.singleFriendPanelList = new SortedList<long, Panel>();
            if (this.singleFriendIconLabelList == null) this.singleFriendIconLabelList = new SortedList<long, Label>();
            if (this.singleFriendUsernameLabelList == null) this.singleFriendUsernameLabelList = new SortedList<long, Label>();
            if (this.singleFriendNameLabelList == null) this.singleFriendNameLabelList = new SortedList<long, Label>();
            foreach (KeyValuePair<long, Panel> item in this.singleFriendPanelList)
            {
                item.Value.Name = "invalid";
            }

            SortedList<long, Consumer> validFriendList = new SortedList<long, Consumer>();

            if(friendList != null)
            {
                foreach (Consumer friend in friendList)
                {
                    Panel currentFriendPanel;
                    if (!this.singleFriendPanelList.ContainsKey(friend.Id))
                    {
                        this.singleFriendPanelList[friend.Id] = new Panel();
                        currentFriendPanel = this.singleFriendPanelList[friend.Id];
                        friendCarryListPanel.Controls.Add(currentFriendPanel);

                        currentFriendPanel.Width = this.friendCarryListPanel.Width;

                        Label friendIconLabel = singleFriendIconLabelList[friend.Id] = new Label();
                        friendIconLabel.Image = new Bitmap(friend.ProfileImage, new Size(50, 50));
                        friendIconLabel.Size = friendIconLabel.Image.Size;
                        currentFriendPanel.Height = friendIconLabel.Height + 10;
                        friendIconLabel.Location = new Point(5, (currentFriendPanel.Height - friendIconLabel.Height) / 2);
                        currentFriendPanel.Controls.Add(friendIconLabel);

                        Label friendNameLabel = singleFriendNameLabelList[friend.Id] = new Label();
                        friendNameLabel.Text = friend.Name;
                        friendNameLabel.Font = CustomFonts.Smaller;
                        friendNameLabel.Size = friendNameLabel.PreferredSize;
                        friendNameLabel.Location = new Point(friendIconLabel.Right + 5, 5);
                        currentFriendPanel.Controls.Add(friendNameLabel);

                        Label usernameLabel = singleFriendUsernameLabelList[friend.Id] = new Label();
                        usernameLabel.Text = friend.Username;
                        usernameLabel.Font = CustomFonts.Smallest;
                        usernameLabel.Size = usernameLabel.PreferredSize;
                        usernameLabel.Location = new Point(friendIconLabel.Right + 5, friendNameLabel.Bottom + 5);
                        currentFriendPanel.Controls.Add(usernameLabel);

                        currentFriendPanel.Height = Math.Max(currentFriendPanel.PreferredSize.Height, friendIconLabel.Height) + 10;
                        currentFriendPanel.BackColor = Colors.DragengerTileColor;

                        friendIconLabel.Click += (s, e) => { this.OpenConversationWith(friend); };
                        friendIconLabel.MouseEnter += (s, e) => { currentFriendPanel.BackColor = Color.FromArgb(currentFriendPanel.BackColor.R - 25, currentFriendPanel.BackColor.G - 25, currentFriendPanel.BackColor.B - 25); };
                        friendIconLabel.MouseLeave += (s, e) => { currentFriendPanel.BackColor = Color.FromArgb(currentFriendPanel.BackColor.R + 25, currentFriendPanel.BackColor.G + 25, currentFriendPanel.BackColor.B + 25); };

                        friendNameLabel.Click += (s, e) => { this.OpenConversationWith(friend); };
                        friendNameLabel.MouseEnter += (s, e) => { currentFriendPanel.BackColor = Color.FromArgb(currentFriendPanel.BackColor.R - 25, currentFriendPanel.BackColor.G - 25, currentFriendPanel.BackColor.B - 25); };
                        friendNameLabel.MouseLeave += (s, e) => { currentFriendPanel.BackColor = Color.FromArgb(currentFriendPanel.BackColor.R + 25, currentFriendPanel.BackColor.G + 25, currentFriendPanel.BackColor.B + 25); };

                        usernameLabel.Click += (s, e) => { this.OpenConversationWith(friend); };
                        usernameLabel.MouseEnter += (s, e) => { currentFriendPanel.BackColor = Color.FromArgb(currentFriendPanel.BackColor.R - 25, currentFriendPanel.BackColor.G - 25, currentFriendPanel.BackColor.B - 25); };
                        usernameLabel.MouseLeave += (s, e) => { currentFriendPanel.BackColor = Color.FromArgb(currentFriendPanel.BackColor.R + 25, currentFriendPanel.BackColor.G + 25, currentFriendPanel.BackColor.B + 25); };

                        currentFriendPanel.Click += (s, e) => 
                        {
                            if (ConversationPanel.CurrentDisplayedConversationPanel.TheConversation != null && ((DuetConversation)(ConversationPanel.CurrentDisplayedConversationPanel.TheConversation)).OtherMember.Id == friend.Id)
                            {
                                SlidebarPanel.MySidebarPanel.ChangeState();
                                return;
                            }
                            this.OpenConversationWith(friend);
                        };
                        currentFriendPanel.MouseEnter += (s, e) => { ((Panel)s).BackColor = Color.FromArgb(((Panel)s).BackColor.R - 25, ((Panel)s).BackColor.G - 25, ((Panel)s).BackColor.B - 25); };
                        currentFriendPanel.MouseLeave += (s, e) => { ((Panel)s).BackColor = Color.FromArgb(((Panel)s).BackColor.R + 25, ((Panel)s).BackColor.G + 25, ((Panel)s).BackColor.B + 25); };
                    }
                    else { currentFriendPanel = this.singleFriendPanelList[friend.Id]; }
                    currentFriendPanel.Name = "valid";
                    validFriendList[friend.Id] = friend;
                }
            }

            List<long> invalidKeyList = new List<long>();
            foreach (KeyValuePair<long, Panel> item in this.singleFriendPanelList)
            {
                if (item.Value.Name == "invalid")
                {
                    friendCarryListPanel.Controls.Remove(item.Value);
                    item.Value.Dispose();
                    invalidKeyList.Add(item.Key);
                }
            }
            foreach (long key in invalidKeyList)
            {
                this.singleFriendPanelList.Remove(key);
                this.singleFriendIconLabelList.Remove(key);
                this.singleFriendNameLabelList.Remove(key);
                this.singleFriendUsernameLabelList.Remove(key);
            }

            foreach (KeyValuePair<long, Panel> item in this.singleFriendPanelList)
            {
                Panel singleFriendPanel = item.Value;
                singleFriendPanel.Top = previousFriendBottom + 2;
                previousFriendBottom = singleFriendPanel.Bottom;

                Consumer friend = validFriendList[item.Key];
                singleFriendIconLabelList[item.Key].Image = new Bitmap(friend.ProfileImage, new Size(50, 50));
                singleFriendNameLabelList[item.Key].Text = friend.Name;
                singleFriendUsernameLabelList[item.Key].Text = friend.Username;
            }

            this.friendCarryListPanel.Size = this.friendCarryListPanel.PreferredSize;
        }

        private bool OpenConversationWith(Consumer friend)
        {
            if (!SlidebarPanel.MySidebarPanel.Closed) SlidebarPanel.MySidebarPanel.ChangeState();
            try { ConversationPanel.CurrentDisplayedConversationPanel.Dispose(); } catch { }
            ConversationPanel.CurrentDisplayedConversationPanel = new ConversationPanel(Universal.ParentForm, friend);
            return true;
        }

        //event handlers
        private void OnGotFocus(Object sender, EventArgs e)
        {
            if(sender == this.searchBox)
            {
                this.searchBox.Text = "";
                this.searchBox.ForeColor = Color.FromArgb(41, 41, 41);
                this.searchBox.Font = CustomFonts.Small;
            }
            
        }
        private void OnLostFocus(Object sender, EventArgs e)
        {
            if (sender == this.searchBox)
            {
                this.searchBox.Text = "Search friends";
                this.searchBox.ForeColor = Color.FromArgb(150, 150, 150);
                this.searchBox.Font = CustomFonts.New(CustomFonts.SmallSize, 'I');
            }

        }
        internal static FriendListPanel MyFriendListPanel
        {
            set;
            get;
        }
    }
}
