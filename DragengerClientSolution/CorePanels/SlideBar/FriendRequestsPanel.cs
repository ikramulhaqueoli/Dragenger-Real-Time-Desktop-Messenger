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
using System.Windows.Forms;

namespace CorePanels.SlideBar
{
    public class FriendRequestsPanel : ConsumerListPanel
    {
        public FriendRequestsPanel(Panel parent)
        {
            this.parent = parent;
            this.BackColor = Color.FromArgb(176, 176, 176);
            this.Size = SlidebarPanel.MySidebarPanel.ListPanelsSize;
            this.ShowBackButtonLabel();
            this.ShowSearchBar();
            this.InitilizeFoundUserListPanel();
            this.ShowAllFriendRequests();
        }

        private void ShowBackButtonLabel()
        {
            this.backButtonLabel = new Label();
            this.backButtonLabel.Name = "Back";
            this.backButtonLabel.Size = new Size(this.Width / 6, this.Height / 8);
            this.backButtonLabel.Location = new Point(3, 8);
            this.backButtonLabel.Image = new Bitmap(FileResources.Icon("back.png"), new Size(backButtonLabel.Height - 20, backButtonLabel.Height - 20));
            this.backButtonLabel.ImageAlign = ContentAlignment.MiddleCenter;
            this.backButtonLabel.MouseEnter += delegate (Object sender, EventArgs e) { ((Label)sender).BackColor = Color.FromArgb(((Label)sender).BackColor.R - 25, ((Label)sender).BackColor.G - 25, ((Label)sender).BackColor.B - 25); };
            this.backButtonLabel.MouseLeave += delegate (Object sender, EventArgs e) { ((Label)sender).BackColor = Color.FromArgb(((Label)sender).BackColor.R + 25, ((Label)sender).BackColor.G + 25, ((Label)sender).BackColor.B + 25); };
            this.backButtonLabel.Click += delegate (Object sender, EventArgs e) { this.parent.Controls.Remove(this); this.parent.Controls.Add(((FriendListPanel)parent).FriendListMainPanel); };
            this.Controls.Add(this.backButtonLabel);

            Label headline = new Label();
            headline.Text = "Friend Requests";
            headline.Font = CustomFonts.BigBold;
            headline.ForeColor = Color.FromArgb(95, 95, 95);
            headline.Size = headline.PreferredSize;
            headline.Location = new Point(this.backButtonLabel.Right + 5, 3 + (this.backButtonLabel.Bottom - headline.Height) / 2);
            this.Controls.Add(headline);
        }

        private void ShowSearchBar()
        {
            this.searchBox = new TextBox();
            this.searchBox.Font = CustomFonts.Small;
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchBox.Height = this.searchBox.PreferredHeight;
            this.searchBox.Text = "Search Friend Requests";
            this.searchBox.ForeColor = Color.FromArgb(150, 150, 150);
            this.searchBox.Font = CustomFonts.New(CustomFonts.SmallSize, 'I');
            this.searchBox.Top = this.backButtonLabel.Bottom + 5;
            this.searchBox.GotFocus += new EventHandler(OnGotFocus);
            this.searchBox.LostFocus += new EventHandler(OnLostFocus);
            this.searchBox.TextChanged += new EventHandler(OnTextChanged);
            this.Controls.Add(this.searchBox);

            this.searchIcon = new Label();
            this.searchIcon.Image = new Bitmap(FileResources.Icon("searchIcon.png"), new Size(this.searchBox.Height * 3 / 4, this.searchBox.Height * 3 / 4));
            this.searchIcon.Size = new Size(this.searchBox.Height, this.searchBox.Height);
            this.searchIcon.BackColor = Color.White;
            this.searchIcon.Location = new Point(3, this.searchBox.Top + (this.searchBox.Height - this.searchIcon.Height) / 2);
            this.searchBox.Left = searchIcon.Right;
            this.searchBox.Width = this.parent.Width - this.searchIcon.Width - this.searchIcon.Left - 8;
            this.Controls.Add(searchIcon);
        }

        private void ShowAllFriendRequests()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (s, e) =>
            {
                List<JObject> requestingUserJsonList = ServerRequest.GetFriendRequestsByKeyword(Consumer.LoggedIn.Id, "");
                if (this.InvokeRequired) this.Invoke(new Action(() => { ShowMatchedList(requestingUserJsonList); }));
                else ShowMatchedList(requestingUserJsonList);
            };
            backgroundWorker.RunWorkerAsync();
            backgroundWorker.RunWorkerCompleted += (s, e) => { backgroundWorker.Dispose(); };
        }

        private void OnTextChanged(object sender, EventArgs me)
        {
            string keyword = ((TextBox)sender).Text;
            if (keyword.Length >= 2)
            {
                VisualizingTools.ShowWaitingAnimation(new Point(this.searchIcon.Left, this.searchBox.Bottom + 5), new Size(this.searchIcon.Width + this.searchBox.Width, this.searchBox.Height / 2), this);
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += (s, e) =>
                {
                    List<JObject> matchedJsonList = ServerRequest.GetFriendRequestsByKeyword(User.LoggedIn.Id, keyword);
                    this.Invoke(new Action(() => { this.ShowMatchedList(matchedJsonList); }));
                };
                backgroundWorker.RunWorkerCompleted += (s, e) => { backgroundWorker.Dispose(); VisualizingTools.HideWaitingAnimation(); };
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void OnLostFocus(object sender, EventArgs e)
        {
            if (sender == this.searchBox && this.searchBox.Text.Length == 0)
            {
                this.searchBox.Text = "Search Friend Requests";
                this.searchBox.ForeColor = Color.FromArgb(150, 150, 150);
                this.searchBox.Font = CustomFonts.New(CustomFonts.SmallSize, 'I');
            }
        }

        private void OnGotFocus(object sender, EventArgs e)
        {
            if (sender == this.searchBox && this.searchBox.Text == "Search Friend Requests")
            {
                this.searchBox.Text = "";
                this.searchBox.ForeColor = Color.FromArgb(41, 41, 41);
                this.searchBox.Font = CustomFonts.Small;
            }
        }
    }
}