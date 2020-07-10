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
    public class AddFriendPanel : ConsumerListPanel
    {
        public AddFriendPanel(Panel parent)
        {
            this.parent = parent;
            this.Name = "AddFriendPanel";
            this.BackColor = Color.FromArgb(176, 176, 176);
            this.Size = SlidebarPanel.MySidebarPanel.ListPanelsSize;
            this.ShowBackButtonLabel();
            this.ShowHeadlineLabel("Add New Friends");
            this.ShowSearchBar();
            this.InitilizeFoundUserListPanel();
        }

        private void ShowSearchBar()
        {
            this.searchBox = new TextBox();
            this.searchBox.Font = CustomFonts.Small;
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchBox.Height = this.searchBox.PreferredHeight;
            this.searchBox.Text = "Search Users";
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

        private void OnTextChanged(object sender, EventArgs me)
        {
            string keyword = ((TextBox)sender).Text;
            if (keyword.Length >= 2)
            {
                VisualizingTools.ShowWaitingAnimation(new Point(this.searchIcon.Left, this.searchBox.Bottom + 5), new Size(this.searchIcon.Width + this.searchBox.Width, this.searchBox.Height / 2), this);
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += (s, e) =>
                {
                    List<JObject> matchedJsonList = ServerRequest.SearchTop20PersonsByKeyword(User.LoggedIn.Id, keyword);
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
                this.searchBox.Text = "Search Users";
                this.searchBox.ForeColor = Color.FromArgb(150, 150, 150);
                this.searchBox.Font = CustomFonts.New(CustomFonts.SmallSize, 'I');
            }
        }

        private void OnGotFocus(object sender, EventArgs e)
        {
            if (sender == this.searchBox && this.searchBox.Text == "Search Users")
            {
                this.searchBox.Text = "";
                this.searchBox.ForeColor = Color.FromArgb(41, 41, 41);
                this.searchBox.Font = CustomFonts.Small;
            }
        }
    }
}
