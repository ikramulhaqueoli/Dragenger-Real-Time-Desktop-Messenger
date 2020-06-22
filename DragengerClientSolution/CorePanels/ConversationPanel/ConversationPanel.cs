using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ResourceLibrary;
using EntityLibrary;
using FileIOAccess;
using System.IO;
using System.Media;
using System.Drawing;
using System.ComponentModel;
using ServerConnections;
using System.Runtime.InteropServices;
using LocalRepository;
using System.Drawing.Text;

namespace CorePanels
{
    public partial class ConversationPanel : Panel, IDisposable
    {
        private Conversation theConversation;
        private Consumer receiver;
        private Form parent;
        private Panel conversationTitleBar, typingSpaceBar;
        private Panel nuntiasSpaceParentPanel, nuntiasBossPanel, filePreviewPanel;
        private TextBox nuntiasTextBox;
        private Label somethingBeingTypedLabel, sidebarSwitchLabel, lastShowedLabel, currentStatusLabel, currentStatusIcon, conversationNameLabel, dropPromptLabel;
        private Button sendButton, chooseFileButton, snapshotButton;
        private Nuntias lastShowedNuntias;
        private Point selfProfileImageLocation, othersProfileImageLocation;
        private Size profileImageSize;
        private int maxCharactersPerLine;
        private static SoundPlayer newMessageSound;

        public ConversationPanel(Form parent, Conversation theConversation)
        {
            this.parent = parent;
            this.Size = parent.ClientSize;
            this.theConversation = theConversation;
            if (ConversationPanel.newMessageSound == null) ConversationPanel.newMessageSound = FileResources.Audio("when.wav");
            if (CurrentDisplayedConversationPanel != null && CurrentDisplayedConversationPanel.theConversation != null)
            {
                if (this.theConversation.ConversationID == CurrentDisplayedConversationPanel.theConversation.ConversationID) return;
            }
            this.Click += new EventHandler(OnClick);
            if (theConversation == null)
            {
                this.ThreeLinesOnConversationTitleBar();
                this.ShowSmilyWelcome();
                this.LoadNewConversation();
                return;
            }
            if (theConversation.Type == "duet" && ((DuetConversation)theConversation).Member1.Id == User.LoggedIn.Id) this.receiver = ((DuetConversation)theConversation).Member2;
            else this.receiver = ((DuetConversation)theConversation).Member1;
            this.PrimaryStageInitializer();
            this.LoadNewConversation();
        }

        public ConversationPanel(Form parent, Consumer receiver)
        {
            this.parent = parent;
            this.Size = parent.ClientSize;
            this.receiver = receiver;
            if (CurrentDisplayedConversationPanel != null && CurrentDisplayedConversationPanel.receiver != null)
            {
                if (this.receiver.Id == CurrentDisplayedConversationPanel.receiver.Id) return;
            }
            this.PrimaryStageInitializer();
            this.LoadNewConversation();
        }

        private void PrimaryStageInitializer()
        {
            this.maxCharactersPerLine = 40;
            this.ConversationTitleBarInitialize();
            this.somethingBeingTypedLabelInitialize();
            this.TypingSpaceBarInitialize();
            this.NuntiasSpaceParentPanelInitialize();
            this.LoadPerviousNuntiasList();
        }

        private void LoadPerviousNuntiasList()
        {
            VisualizingTools.ShowWaitingAnimation(new Point(this.Width / 4, this.conversationTitleBar.Bottom + 30), new Size(this.Width / 2, 10), this);
            BackgroundWorker loaderWorker = new BackgroundWorker();
            loaderWorker.DoWork += (s, e) =>
            {
                if (this.TheConversation == null)
                {
                    long? conversationId = ServerRequest.GetDuetConversationId(User.LoggedIn, this.receiver);
                    if (conversationId == null)
                    {
                        MessageBox.Show("Server connection failed!\r\nPlease retry.");
                        return;
                    }
                    this.TheConversation = new DuetConversation(Consumer.LoggedIn, this.receiver);
                    this.TheConversation.ConversationID = (long)conversationId;
                }

                List<Nuntias> previousNuntii = ConversationRepository.Instance.GetLastNuntias(this.theConversation);
                List<Nuntias> previousPendingNuntii = ConversationRepository.Instance.GetPendingNuntii(this.theConversation);
                previousNuntii.AddRange(previousPendingNuntii);

                if (previousNuntii != null && previousNuntii.Count > 0)
                {
                    foreach (Nuntias item in previousNuntii)
                    {
                        SyncAssets.NuntiasSortedList[item.Id] = item;
                        if(item.Id > 0) ServerFileRequest.DownloadAndStoreContentFile(item);
                        this.ShowNuntias(item, false);
                    }
                }
                this.ConversationPanelLoadingOk = true;
            };
            loaderWorker.RunWorkerCompleted += (s, e) => { VisualizingTools.HideWaitingAnimation(); loaderWorker.Dispose(); };
            loaderWorker.RunWorkerAsync();
        }

        public void RefreshCurrentConversationReceiver()
        {
            if (this.theConversation.Type != "duet") return;
            this.receiver.LastActive = ServerRequest.ReceiverLastActiveTime(this.receiver.Id);
            Universal.ParentForm.Invoke(new MethodInvoker(delegate
            {
                if (this.receiver.LastActive == null)
                {
                    this.currentStatusLabel.Visible = false;
                    this.currentStatusIcon.Visible = false;
                    return;
                }
                if (Time.TimeDistanceInSecond(Time.CurrentTime, this.receiver.LastActive) > 5)
                {
                    long timeDistanceInMinute = Time.TimeDistanceInMinute(Time.CurrentTime, this.receiver.LastActive);
                    if (timeDistanceInMinute < 60) this.currentStatusLabel.Text = "Active " + timeDistanceInMinute + " minutes ago";
                    else if (timeDistanceInMinute < 1440) this.currentStatusLabel.Text = "Active about " + timeDistanceInMinute / 60 + " hours ago";
                    else if (timeDistanceInMinute < 10080) this.currentStatusLabel.Text = "Active about " + timeDistanceInMinute / 1440 + " days ago";
                    else if (timeDistanceInMinute < 40320) this.currentStatusLabel.Text = "Active about " + timeDistanceInMinute / 10080 + " weeks ago";
                    else this.currentStatusLabel.Text = "Active about " + timeDistanceInMinute / 40320 + " months ago";
                    this.currentStatusIcon.Visible = false;
                }
                else
                {
                    this.currentStatusLabel.Text = "Active Now";
                    this.currentStatusIcon.Visible = true;
                }
                this.currentStatusLabel.Visible = true;
                this.currentStatusLabel.Size = this.currentStatusLabel.PreferredSize;
                this.currentStatusLabel.Left = this.conversationNameLabel.Right - currentStatusLabel.PreferredWidth;
                this.currentStatusIcon.Left = this.currentStatusLabel.Left - 15;
            }));
        }

        private void ShowSmilyWelcome()
        {
            Label smilyIconLabel = new Label();
            smilyIconLabel.Image = new Bitmap(FileResources.Icon("smile.png"), new Size(this.Width / 2, this.Width / 2));
            smilyIconLabel.Size = smilyIconLabel.Image.Size;
            smilyIconLabel.Location = new Point((this.Width - smilyIconLabel.Image.Width) / 2, (Universal.ParentForm.Height - smilyIconLabel.Height) / 4);
            smilyIconLabel.Click += new EventHandler(OnClick);
            this.Controls.Add(smilyIconLabel);

            Label welcomeMessageLabel = new Label();
            welcomeMessageLabel.Text = "Hey, Welcome to dragenger!\r\n\r\nLet's make\r\nConversations with fun.";
            welcomeMessageLabel.TextAlign = ContentAlignment.MiddleCenter;
            welcomeMessageLabel.Font = CustomFonts.BiggerBold;
            welcomeMessageLabel.Size = welcomeMessageLabel.PreferredSize;
            welcomeMessageLabel.ForeColor = Color.FromArgb(128, 128, 128);
            welcomeMessageLabel.Location = new Point((this.Width - welcomeMessageLabel.Width) / 2, smilyIconLabel.Bottom + 15);
            welcomeMessageLabel.Click += new EventHandler(OnClick);
            this.Controls.Add(welcomeMessageLabel);
        }

        private void ThreeLinesOnConversationTitleBar()
        {
            this.conversationTitleBar = new Panel();
            this.conversationTitleBar.Size = new Size(this.Width, this.Height / 9);
            this.conversationTitleBar.BackColor = Color.FromArgb(100, 100, 100);
            this.conversationTitleBar.Location = new Point(0, 0);
            this.conversationTitleBar.Click += new EventHandler(OnClick);

            Image threeLines = FileResources.Icon("threeLines.png");
            this.sidebarSwitchLabel = new Label();
            this.sidebarSwitchLabel.Image = new Bitmap(threeLines, conversationTitleBar.Height / 2 - conversationTitleBar.Height / 8, conversationTitleBar.Height / 2 - conversationTitleBar.Height / 8);
            this.sidebarSwitchLabel.Size = this.sidebarSwitchLabel.Image.Size;
            this.sidebarSwitchLabel.Location = new Point((this.conversationTitleBar.Height - this.sidebarSwitchLabel.Height) / 2, (this.conversationTitleBar.Height - this.sidebarSwitchLabel.Height) / 2);
            this.sidebarSwitchLabel.Click += new EventHandler(OnClick);
            this.sidebarSwitchLabel.MouseEnter += new EventHandler(OnMouseEnter);
            this.sidebarSwitchLabel.MouseLeave += new EventHandler(OnMouseLeave);
            this.conversationTitleBar.Controls.Add(sidebarSwitchLabel);
            this.Controls.Add(conversationTitleBar);
        }
        public bool ConversationPanelLoadingOk
        {
            set;
            get;
        }
        private void LoadNewConversation()
        {
            if (ConversationPanel.CurrentDisplayedConversationPanel != null)
            {   
                Universal.ParentForm.Controls.Remove(ConversationPanel.CurrentDisplayedConversationPanel);
                ConversationPanel.CurrentDisplayedConversationPanel.Dispose();
                ConversationPanel.CurrentDisplayedConversationPanel = null;
            }
            ConversationPanel.CurrentDisplayedConversationPanel = this;
            Universal.ParentForm.Controls.Add(this);
        }

        public Conversation TheConversation
        {
            set { this.theConversation = value; }
            get { return this.theConversation; }
        }

        private void ConversationTitleBarInitialize()
        {
            this.ThreeLinesOnConversationTitleBar();

            Label conversationIcon = new Label();
            conversationIcon.Image = FetchConversationIcon();
            conversationIcon.Size = conversationIcon.Image.Size;
            conversationIcon.Location = new Point(this.conversationTitleBar.Width - (this.conversationTitleBar.Height - conversationIcon.Height) / 2 - conversationIcon.Image.Width, (this.conversationTitleBar.Height - conversationIcon.Height) / 2);
            this.conversationTitleBar.Controls.Add(conversationIcon);

            conversationNameLabel = new Label();
            conversationNameLabel.Text = FetchConversationName();
            conversationNameLabel.Font = CustomFonts.RegularBold;
            conversationNameLabel.ForeColor = Color.FromArgb(240, 240, 240);
            conversationNameLabel.Size = conversationNameLabel.PreferredSize;
            conversationNameLabel.Location = new Point(conversationIcon.Left - conversationNameLabel.Width - 5, this.conversationTitleBar.Height / 6);
            this.conversationTitleBar.Controls.Add(conversationNameLabel);

            currentStatusLabel = new Label();
            currentStatusLabel.Visible = false;
            currentStatusLabel.Font = CustomFonts.SmallerBold;
            currentStatusLabel.ForeColor = Color.FromArgb(240, 240, 240);
            currentStatusLabel.Size = currentStatusLabel.PreferredSize;
            currentStatusLabel.Location = new Point(conversationNameLabel.Right - currentStatusLabel.PreferredWidth - 5, this.conversationTitleBar.Height - currentStatusLabel.PreferredHeight - this.conversationTitleBar.Height / 6);
            this.conversationTitleBar.Controls.Add(currentStatusLabel);

            currentStatusIcon = new Label();
            currentStatusIcon.Visible = false;
            currentStatusIcon.Image = new Bitmap(FileResources.Icon("greenDot.png"), new Size(10, 10));
            currentStatusIcon.Size = new Size(10,10);
            currentStatusIcon.Location = new Point(currentStatusLabel.Left - 13, currentStatusLabel.Bottom - this.conversationTitleBar.Height / 6 - 1);
            this.conversationTitleBar.Controls.Add(currentStatusIcon);

            this.Controls.Add(conversationTitleBar);
        }

        private void NuntiasSpaceParentPanelInitialize()
        {
            this.nuntiasSpaceParentPanel = new Panel();
            this.nuntiasSpaceParentPanel.Width = this.Width - 8;
            this.nuntiasSpaceParentPanel.Height = this.Height - (this.conversationTitleBar.Height + this.typingSpaceBar.Height + 8);
            this.nuntiasSpaceParentPanel.Location = new Point(4, this.conversationTitleBar.Bottom + 4);
            this.Controls.Add(this.nuntiasSpaceParentPanel);

            this.nuntiasBossPanel = new Panel();
            this.nuntiasBossPanel.Size = this.nuntiasSpaceParentPanel.Size;
            this.nuntiasBossPanel.BackColor = Color.FromArgb(221, 221, 221);
            this.nuntiasBossPanel.AutoScroll = false;
            this.nuntiasBossPanel.HorizontalScroll.Enabled = false;
            this.nuntiasBossPanel.HorizontalScroll.Visible = false;
            this.nuntiasBossPanel.HorizontalScroll.Maximum = 0;
            this.nuntiasBossPanel.AutoScroll = true;
            this.nuntiasBossPanel.AllowDrop = true;
            this.SetSizeLocation();
            this.nuntiasBossPanel.Click += new EventHandler(OnClick);

            this.dropPromptLabel = new Label();
            this.dropPromptLabel.Font = CustomFonts.New(CustomFonts.SmallerSize, 'i');
            this.dropPromptLabel.BackColor = Color.FromArgb(255, 253, 221);
            this.dropPromptLabel.ForeColor = Color.FromArgb(dropPromptLabel.BackColor.R - 100, dropPromptLabel.BackColor.G - 100, dropPromptLabel.BackColor.B - 100);
            this.dropPromptLabel.Text = "Drop the file to send it to\r\n" + this.receiver.Name + "\r\n\r\n[You will see a preview\r\nof the file before sending.]";
            this.dropPromptLabel.Image = new Bitmap(FileResources.Icon("dragdrop.png"), new Size(100,150));
            this.dropPromptLabel.ImageAlign = ContentAlignment.TopCenter;
            this.dropPromptLabel.Size = this.nuntiasBossPanel.Size;
            this.dropPromptLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.dropPromptLabel.Visible = false;
            this.dropPromptLabel.AllowDrop = true;

            this.somethingBeingTypedLabel.BackColor = this.nuntiasBossPanel.BackColor;

            this.nuntiasSpaceParentPanel.Controls.Add(this.nuntiasBossPanel);
            this.nuntiasSpaceParentPanel.Controls.Add(this.dropPromptLabel);

            this.nuntiasBossPanel.DragEnter += (s, e) =>
            {
                Console.WriteLine("OnDragEnter nuntiasBossPanel");
                this.nuntiasBossPanel.Visible = false;
                this.dropPromptLabel.Visible = true;
                e.Effect = DragDropEffects.All;
            };

            this.dropPromptLabel.DragEnter += (s, e) =>
            {
                Console.WriteLine("OnDragEnter dropPromptLabel");
                e.Effect = DragDropEffects.All;
            };

            this.dropPromptLabel.DragLeave += (s, e) =>
            {
                Console.WriteLine("OnDragLeave dropPromptLabel");
                this.dropPromptLabel.Visible = false;
                this.nuntiasBossPanel.Visible = true;
            };

            this.dropPromptLabel.DragDrop += (s, e) =>
            {
                Console.WriteLine("OnDragDrop dropPromptLabel");
                this.dropPromptLabel.Visible = false;
                this.nuntiasBossPanel.Visible = true;

                try
                {
                    string filePath = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
                    this.ShowNuntiasContentPreview(Path.GetFileName(filePath), filePath);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error on dropping the file: " + ex.Message);
                }
            };
        }

        private void TypingSpaceBarInitialize()
        {
            this.typingSpaceBar = new Panel();
            this.typingSpaceBar.BackColor = Color.FromArgb(128, 128, 128);
            this.typingSpaceBar.Width = this.Width;
            this.NuntiasTextBoxInitialize();
            this.SendButtonsInitialize();
            this.typingSpaceBar.Controls.Add(nuntiasTextBox);
            this.Controls.Add(typingSpaceBar);
            this.parent.AcceptButton = this.sendButton;
            this.AdjustTypingBarSize();
        }

        private void somethingBeingTypedLabelInitialize()
        {
            this.somethingBeingTypedLabel = new Label();
            this.somethingBeingTypedLabel.Font = CustomFonts.New(12, 'i');
            this.somethingBeingTypedLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.somethingBeingTypedLabel.ForeColor = Color.FromArgb(80,80,80);
            this.somethingBeingTypedLabel.Width = this.Width;
            this.somethingBeingTypedLabel.Padding = new Padding(30,5,30,5);
            this.somethingBeingTypedLabel.Left = (this.Width - this.somethingBeingTypedLabel.Width)/2;
            this.Controls.Add(this.somethingBeingTypedLabel);
        }

        private void NuntiasTextBoxInitialize()
        {
            this.nuntiasTextBox = new TextBox();
            this.nuntiasTextBox.Font = CustomFonts.Regular;
            this.nuntiasTextBox.BackColor = Color.FromArgb(255, 255, 255);
            this.nuntiasTextBox.BorderStyle = BorderStyle.None;
            this.nuntiasTextBox.Multiline = true;
            this.nuntiasTextBox.WordWrap = true;
            this.nuntiasTextBox.AcceptsTab = true;
            this.nuntiasTextBox.AcceptsReturn = false;
            this.nuntiasTextBox.Width = this.typingSpaceBar.Width - this.typingSpaceBar.Width / 4;
            this.nuntiasTextBox.Height = this.nuntiasTextBox.PreferredHeight;
            this.nuntiasTextBox.Location = new Point(6, 6);
            this.nuntiasTextBox.Focus();
            this.nuntiasTextBox.KeyPress += (s, e) => { Console.WriteLine("hey"); };
            this.nuntiasTextBox.Click += new EventHandler(OnClick);
            this.nuntiasTextBox.TextChanged += new EventHandler(OnTextChangeNuntiasTextBox);
        }

        private void SendButtonsInitialize()
        {
            this.sendButton = new Button();
            this.sendButton.Image = new Bitmap(FileResources.Icon("sendButton.png"), new Size(this.nuntiasTextBox.Height, this.nuntiasTextBox.Height));
            this.sendButton.Font = this.nuntiasTextBox.Font;
            this.sendButton.Size = this.sendButton.Image.Size;
            this.sendButton.Left = this.nuntiasTextBox.Width + 15;
            this.sendButton.FlatStyle = FlatStyle.Flat;
            this.sendButton.FlatAppearance.BorderSize = 0;
            this.sendButton.Click += new EventHandler(this.OnClick);
            this.typingSpaceBar.Controls.Add(this.sendButton);

            this.chooseFileButton = new Button();
            this.chooseFileButton.Image = GraphicsStudio.ResizeImageByHeight(FileResources.Icon("choosefiles.png"), sendButton.Image.Height);
            this.chooseFileButton.Font = this.nuntiasTextBox.Font;
            this.chooseFileButton.Size = this.chooseFileButton.Image.Size;
            this.chooseFileButton.Left = this.sendButton.Right + 15;
            this.chooseFileButton.FlatStyle = FlatStyle.Flat;
            this.chooseFileButton.FlatAppearance.BorderSize = 0;
            this.chooseFileButton.Click += new EventHandler(this.OnClick);
            this.typingSpaceBar.Controls.Add(this.chooseFileButton);

            this.snapshotButton = new Button();
            this.snapshotButton.Image = GraphicsStudio.ResizeImageByHeight(FileResources.Icon("snapshot.png"), sendButton.Image.Height);
            this.snapshotButton.Font = this.nuntiasTextBox.Font;
            this.snapshotButton.Size = this.snapshotButton.Image.Size;
            this.snapshotButton.Left = this.chooseFileButton.Right + 15;
            this.snapshotButton.FlatStyle = FlatStyle.Flat;
            this.snapshotButton.FlatAppearance.BorderSize = 0;
            this.snapshotButton.Click += new EventHandler(this.OnClick);
            this.typingSpaceBar.Controls.Add(this.snapshotButton);
        }

        private Image FetchConversationIcon()
        {
            Image icon = FileResources.NullProfileImage;
            if (this.receiver != null) icon = this.receiver.ProfileImage;
            Size size = new Size(this.conversationTitleBar.Height - this.conversationTitleBar.Height / 3, this.conversationTitleBar.Height - this.conversationTitleBar.Height / 3);
            Bitmap objBitmap = new Bitmap(icon, size);
            return objBitmap;
        }

        private string FetchConversationName()
        {
            if (this.receiver != null) return this.receiver.Name;
            return "N/A";
        }

        //to adjust the height of the nuntiasTextBox
        private const int EM_GETLINECOUNT = 0xba;
        [DllImport("user32", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TextBoxHeightMeasure(int hwnd, int wMsg, int wParam, int lParam);
        public void AdjustTypingBarSize()
        {
            int maxHeight = this.Height / 5;
            var numberOfLines = TextBoxHeightMeasure(nuntiasTextBox.Handle.ToInt32(), EM_GETLINECOUNT, 0, 0);
            int measuredHeight = (nuntiasTextBox.Font.Height + 2) * numberOfLines;

            this.nuntiasTextBox.Height = Math.Min(maxHeight, measuredHeight);
            if (measuredHeight > maxHeight) this.nuntiasTextBox.ScrollBars = ScrollBars.Vertical;
            else this.nuntiasTextBox.ScrollBars = ScrollBars.None;

            this.typingSpaceBar.Height = this.nuntiasTextBox.Height + 12;
            this.typingSpaceBar.Top = this.Height - typingSpaceBar.Height;
            this.sendButton.Top = this.chooseFileButton.Top = this.snapshotButton.Top = this.nuntiasTextBox.Top;

            this.somethingBeingTypedLabel.Height = 0;
            if(this.somethingBeingTypedLabel.Text.Length > 0) this.somethingBeingTypedLabel.Height = this.somethingBeingTypedLabel.PreferredHeight + 20;
            this.somethingBeingTypedLabel.Top = this.typingSpaceBar.Top - this.somethingBeingTypedLabel.Height;

            if (this.nuntiasBossPanel != null)
            {
                this.nuntiasSpaceParentPanel.Height = this.dropPromptLabel.Height = this.nuntiasBossPanel.Height = this.Height - (this.conversationTitleBar.Height + this.typingSpaceBar.Height + this.somethingBeingTypedLabel.Height + 8);
                this.nuntiasBossPanel.VerticalScroll.Value = this.nuntiasBossPanel.VerticalScroll.Maximum;
            }
        }

        private void SetSizeLocation()
        {
            this.profileImageSize = new Size(this.sidebarSwitchLabel.Width * 3 / 2, this.sidebarSwitchLabel.Width * 3 / 2);
            this.selfProfileImageLocation = new Point(this.nuntiasBossPanel.Width - profileImageSize.Width - profileImageSize.Width * 13 / 20, 0);
            this.othersProfileImageLocation = new Point(profileImageSize.Width * 13 / 20, 0);
        }

        private void OnClick(Object sender, EventArgs e)
        {
            if (!SlidebarPanel.MySidebarPanel.Closed) SlidebarPanel.MySidebarPanel.ChangeState();
            if (sender == this.sidebarSwitchLabel)
            {
                SlidebarPanel sidebar = SlidebarPanel.MySidebarPanel;
                sidebar.ChangeState();
                string RunningPath = System.AppDomain.CurrentDomain.BaseDirectory;
                return;
            }
            else if (sender == this.sendButton)
            {
                this.SendTypedNuntias();
            }
            else if (sender == this.chooseFileButton)
            {
                this.ChooseFileAndShowPreview();
            }
            else if(sender == this.snapshotButton)
            {
                this.TakeSnapshotOfForm();
            }
        }

        private void OnMouseEnter(Object sender, EventArgs me)
        {
            if (sender == sidebarSwitchLabel)
            {
                this.sidebarSwitchLabel.Image = new Bitmap(FileResources.Icon("threeLinesrev.png"), this.sidebarSwitchLabel.Image.Size);
            }
        }

        private void OnMouseLeave(Object sender, EventArgs me)
        {
            if (sender == sidebarSwitchLabel)
            {
                this.sidebarSwitchLabel.Image = new Bitmap(FileResources.Icon("threeLines.png"), this.sidebarSwitchLabel.Image.Size);
            }
        }

        private void OnTextChangeNuntiasTextBox(Object sender, EventArgs eargs)
        {
            TextBox nuntiasBox = (TextBox)sender;
            string text = nuntiasBox.Text;
            if (text.Length > maxCharactersPerLine) text = "..." + text.Substring(text.Length - maxCharactersPerLine);
            string sendText = "typing:       " + text + "... |";
            if (text.Length == 0) sendText = "";
            BackgroundWorker loaderWorker = new BackgroundWorker();
            loaderWorker.DoWork += (s, e) =>
            {
                ServerRequest.SomethingTypingOnConversationFor(this.theConversation.ConversationID, sendText);
            };
            loaderWorker.RunWorkerCompleted += (s, e) => { loaderWorker.Dispose(); };
            loaderWorker.RunWorkerAsync();
        }

        public Label SomethingBeingTypedLabel
        {
            get { return this.somethingBeingTypedLabel; }
        }

        public Consumer Receiver
        {
            get { return this.receiver; }
        }

        public static ConversationPanel CurrentDisplayedConversationPanel
        {
            set;
            get;
        }
    }
}
