using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ResourceLibrary;
using EntityLibrary;
using FileIOAccess;
using ServerConnections;

namespace CorePanels
{
    public class SlidebarPanel : Panel
    {
        private Form parent;
        private Panel mainPanelInSidebar, userViewPanel, optionBarPanel;
        private List<Label> userViewIcons;
        private bool closed;
        private Timer sidebarSliderTimer;
        internal Label userProfilePictureLabel, userFUllNameLabel;
        private List<Label> optionLabels;
        private List<Panel> listPanels;
        public SlidebarPanel(Form parent)
        {
            this.parent = parent;
            this.Width = parent.ClientSize.Width * 3 / 5;
            this.Height = parent.ClientSize.Height;
            this.BackColor = Color.FromArgb(200, 200, 200);
            this.Location = new Point(0 - this.Width, 0);

            this.mainPanelInSidebar = new Panel();
            this.mainPanelInSidebar.Size = this.Size;
            this.mainPanelInSidebar.BackColor = this.BackColor;
            this.Controls.Add(this.mainPanelInSidebar);

            this.closed = true;
            sidebarSliderTimer = new Timer();
            sidebarSliderTimer.Interval = 10;
            sidebarSliderTimer.Tick += SliderTicker;

            this.UserViewPanelInitialize();
            this.OptionBarPanelInitialize();
            this.ListPanelsSize = new Size(this.optionBarPanel.Width, this.Bottom - this.optionBarPanel.Bottom - 3);
            
            this.listPanels = new List<Panel>();
            this.InitialiseListPanelList();

            this.CurrentOpenOptionIndex = null;
            this.ShowListPanel(0);
        }

        private void InitialiseListPanelList()
        {
            //index 0 for ConversationListPanel
            ConversationListPanel.MyConversationListPanel = new ConversationListPanel(this);
            listPanels.Add(ConversationListPanel.MyConversationListPanel);

            //index 1 for FriendListPanel
            FriendListPanel.MyFriendListPanel = new FriendListPanel(this);
            listPanels.Add(FriendListPanel.MyFriendListPanel);

            //index 2 for EmailListPanel
            listPanels.Add(new EmailListPanel(this));
        }

        public int? CurrentOpenOptionIndex
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
            userProfilePictureLabel.MouseDoubleClick += delegate(Object sender, MouseEventArgs e) { this.ChangeProfilePicture(); };
            this.userViewPanel.Controls.Add(userProfilePictureLabel);

            userFUllNameLabel = new Label();
            userFUllNameLabel.Text = this.UserProfileName;
            userFUllNameLabel.Font = CustomFonts.SmallBold;
            userFUllNameLabel.Size = userFUllNameLabel.PreferredSize;
            userFUllNameLabel.Location = new Point((this.userViewPanel.Width - userFUllNameLabel.Width) / 2, userProfilePictureLabel.Bottom + 20);
            userFUllNameLabel.ForeColor = Color.FromArgb(240, 240, 240);
            this.userViewPanel.Controls.Add(userFUllNameLabel);

            Label dragengerEmailLabel = new Label();
            dragengerEmailLabel.Text = this.FetchDragengerEmail;
            dragengerEmailLabel.Font = CustomFonts.New(12.0f, 'i');
            dragengerEmailLabel.Size = dragengerEmailLabel.PreferredSize;
            dragengerEmailLabel.Location = new Point((this.userViewPanel.Width - dragengerEmailLabel.Width) / 2, userFUllNameLabel.Bottom + 5);
            dragengerEmailLabel.ForeColor = Color.FromArgb(240, 240, 240);
            this.userViewPanel.Controls.Add(dragengerEmailLabel);

            this.userViewPanel.Height = this.userViewPanel.PreferredSize.Height + 20;
            this.mainPanelInSidebar.Controls.Add(this.userViewPanel);
        }

        public void OptionBarPanelInitialize()
        {
            this.optionBarPanel = new Panel();
            this.optionBarPanel.Width = userViewPanel.Width;
            this.optionBarPanel.Height = 50;
            this.optionBarPanel.Location = new Point(this.userViewPanel.Left, this.userViewPanel.Bottom + 3);
            this.AddOptionsToBar();

            this.mainPanelInSidebar.Controls.Add(this.optionBarPanel);
        }

        public void ShowListPanel(int listIndex)
        {
            if (listIndex == this.CurrentOpenOptionIndex) return;

            if (this.CurrentOpenOptionIndex >= 0)
            {
                ReverseOptionLabelColor(this.optionLabels[(int)CurrentOpenOptionIndex], "black");
                this.mainPanelInSidebar.Controls.Remove(this.listPanels[(int)this.CurrentOpenOptionIndex]);
            }

            this.CurrentOpenOptionIndex = listIndex;
            Panel tempPanel = this.listPanels[listIndex];
            tempPanel.Top = this.optionBarPanel.Bottom + 2;
            tempPanel.Left = this.optionBarPanel.Left;
            tempPanel.Size = this.ListPanelsSize;
            this.mainPanelInSidebar.Controls.Add(tempPanel);
            this.ReverseOptionLabelColor(optionLabels[listIndex], "gray");
        }

        private void AddIconsToUserView()
        {
            this.userViewIcons = new List<Label>(3);

            //for sidebar closing button
            this.userViewIcons.Add(new Label());

            Label lastLabel = null;
            for (int i = 1; i < userViewIcons.Capacity; i++)
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

        private void AddOptionsToBar()
        {
            this.optionLabels = new List<Label>();
            this.optionLabels.Add(new Label());
            this.optionLabels.Add(new Label());
            //this.optionLabels.Add(new Label());

            Label lastLabel = null;

            for (int i = 0; i < this.optionLabels.Count; i++)
            {
                this.optionLabels[i].Name = i.ToString();
                this.optionLabels[i].Image = new Bitmap(FileResources.Icon(String.Format("option_{0}.png", i)), new Size(this.optionBarPanel.Height * 3 / 4, this.optionBarPanel.Height * 3 / 4));
                this.optionLabels[i].Size = new Size((this.optionBarPanel.Width / this.optionLabels.Count) - 1, this.optionBarPanel.Height);
                if (lastLabel != null) this.optionLabels[i].Left = lastLabel.Right + 2;
                else this.optionLabels[i].Left = 0;
                this.optionLabels[i].Top = 0;
                lastLabel = this.optionLabels[i];
                this.optionLabels[i].BackColor = Color.FromArgb(28, 28, 28);
                this.optionLabels[i].MouseEnter += new EventHandler(OnMouseEnterToOptionBar);
                this.optionLabels[i].MouseLeave += new EventHandler(OnMouseLeaveFromOptionBar);
                this.optionBarPanel.Controls.Add(this.optionLabels[i]);
            }

            this.optionLabels[0].Click += delegate(Object sender, EventArgs e) { this.ShowListPanel(0); };
            this.optionLabels[1].Click += delegate(Object sender, EventArgs e) { this.ShowListPanel(1); };
            //this.optionLabels[2].Click += delegate(Object sender, EventArgs e) { this.ShowListPanel(2); };
        }

        private void OnMouseEnterToOptionBar(Object sender, EventArgs e)
        {
            if (this.CurrentOpenOptionIndex.ToString() != ((Label)sender).Name) this.ReverseOptionLabelColor((Label)sender, "gray");
        }

        private void OnMouseLeaveFromOptionBar(Object sender, EventArgs e)
        {
            if (this.CurrentOpenOptionIndex.ToString() != ((Label)sender).Name) this.ReverseOptionLabelColor((Label)sender, "black");
        }

        public void ReverseOptionLabelColor(Label currentLabel, string changeColor)
        {
            if (changeColor == "black")
            {
                currentLabel.Image = new Bitmap(FileResources.Icon(String.Format("option_{0}.png", currentLabel.Name)), currentLabel.Image.Size);
                currentLabel.BackColor = Color.FromArgb(28, 28, 28);
            }
            else
            {
                currentLabel.Image = new Bitmap(FileResources.Icon(String.Format("option_{0}_rev.png", currentLabel.Name)), currentLabel.Image.Size);
                currentLabel.BackColor = Color.FromArgb(176, 176, 176);
            }
        }

        public void ChangeProfilePicture()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image Files (JPG, JPEG, PNG)|*.jpg;*.jpeg;*.png";
            fileDialog.InitialDirectory = FileResources.WindowsPicturePath;
            fileDialog.ShowDialog();
            if (fileDialog.FileName.Length == 0) return;
            BackgroundWorker loaderWorker = new BackgroundWorker();
            loaderWorker.DoWork += (s, e) =>
            {
                Image gotImg = Image.FromFile(fileDialog.FileName);
                string profileImgId = ServerFileRequest.ChangeProfileImage(gotImg);
                if (profileImgId != null)
                {
                    Universal.ParentForm.Invoke(new Action(() =>
                    {
                        Consumer.LoggedIn.ProfileImage = LocalDataFileAccess.GetProfileImgFromLocalData(profileImgId);
                        userProfilePictureLabel.Image = this.ResizedProfileImage;
                    }));
                }
                gotImg.Dispose();
            };
            loaderWorker.RunWorkerCompleted += (s, e) => { VisualizingTools.HideWaitingAnimation(); loaderWorker.Dispose(); };
            loaderWorker.RunWorkerAsync();
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

        internal Panel MainPanelInSidebar
        {
            get { return this.mainPanelInSidebar; }
        }

        public bool Closed
        {
            set { this.closed = value; }
            get { return this.closed; }
        }

        public void ChangeState()
        {
            sidebarSliderTimer.Start();
            this.Closed = !this.Closed;
        }

        private void SliderTicker(Object sender, EventArgs e)
        {
            int changeRate = 25;
            if (!Closed)
            {
                if (this.Location.X > -changeRate)
                {
                    this.Location = new Point(0, 0);
                    sidebarSliderTimer.Stop();
                }
                else this.Location = new Point(this.Left + changeRate, 0);
            }
            else
            {
                if (this.Location.X > -this.Width && Math.Abs(this.Location.X - this.Width) >= changeRate)
                {
                    this.Location = new Point(this.Left - changeRate, 0);
                }
                else
                {
                    this.Location = new Point(-this.Width, 0);
                    sidebarSliderTimer.Stop();
                }
            }
        }

        private void OnClick(Object sender, EventArgs e)
        {
            if (sender == this.userViewIcons[0])
            {
                this.ChangeState();
            }
            else if(sender == this.userViewIcons[1])
            {
                BackendManager.Logout();
                
            }
            else if (sender == this.userViewIcons[2])
            {
                this.Controls.Remove(this.mainPanelInSidebar);
                this.Controls.Add(new SettingsPanel(this));
            }
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

        public static SlidebarPanel MySidebarPanel
        {
            set;
            get;
        }
    }

}
