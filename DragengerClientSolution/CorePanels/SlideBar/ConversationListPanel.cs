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
using LocalRepository;
using Newtonsoft.Json.Linq;
using ServerConnections;

namespace CorePanels
{
    class ConversationListPanel : Panel
    {
        private Panel parent;
        private List<JObject> conversationHeaderJsonList;
        private int previousConversationBottom;

        private Dictionary<string, Panel> singleConversationPanelList, conversationDetailsPanelList;
        private Dictionary<string, Label> singleConversationIconLabelList, singleConversationNameLabelList, singleConversationLastNuntiasLabelList;

        private Panel emptyConversationWarnPanel;

        public ConversationListPanel(Panel parent)
        {
            this.parent = parent;
            this.BackColor = Color.FromArgb(176, 176, 176);
            this.singleConversationPanelList = new Dictionary<string, Panel>();
			this.RefreshConversationList();
            this.Size = this.PreferredSize;
        }

        internal void RefreshConversationList()
        {
            try
            {
                Console.WriteLine("RefreshConversationList()");
				this.conversationHeaderJsonList = this.FetchConversationList();
                if (this.conversationHeaderJsonList == null || this.conversationHeaderJsonList.Count == 0)
                {
                    this.ShowEmptyConversationWarning();
                    return;
                }
                if (Universal.ParentForm.InvokeRequired) Universal.ParentForm.Invoke(new Action(() => { this.ShowConversationsInPanel(); }));
                else this.ShowConversationsInPanel();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error occured in RefreshConversationList: \n" + ex.StackTrace + "\n" + ex.Message);
            }
        }

        private void ShowEmptyConversationWarning()
        {
            emptyConversationWarnPanel = new Panel();
            emptyConversationWarnPanel.Size = this.parent.Size;
            this.Controls.Add(emptyConversationWarnPanel);

            Label emptyIconLabel = new Label();
            emptyIconLabel.Image = new Bitmap(FileResources.Icon("empty.png"), new Size(this.Width / 2, this.Width / 2));
            emptyIconLabel.Size = emptyIconLabel.Image.Size;
            emptyIconLabel.Location = new Point((this.parent.Width - emptyIconLabel.Image.Width) / 2, 100);
            emptyConversationWarnPanel.Controls.Add(emptyIconLabel);

            Label emptyTextLabel = new Label();
            emptyTextLabel.Text = "You have no \r\nconversations yet!";
            emptyTextLabel.Font = CustomFonts.RegularBold;
            emptyTextLabel.ForeColor = Color.FromArgb(77, 77, 77);
            emptyTextLabel.Size = emptyTextLabel.PreferredSize;
            emptyTextLabel.TextAlign = ContentAlignment.MiddleCenter;
            emptyTextLabel.Location = new Point((this.parent.Width - emptyTextLabel.Width) / 2, emptyIconLabel.Bottom + 10);
            emptyConversationWarnPanel.Controls.Add(emptyTextLabel);
        }

        private void ShowConversationsInPanel()
        {
			// here conversation icon is dummy yet. replace Consumer.LoggedIn.ProfileImage with proper image
			// here conversationDetailsPanel works only for duetConversations. fix it later.
            if (this.conversationHeaderJsonList.Count > 0)
            {
                try
                {
                    this.Controls.Remove(emptyConversationWarnPanel);
                    emptyConversationWarnPanel.Dispose();
                }
                catch { };
            }
            previousConversationBottom = 0;
            int allowedCharsOnNameLabel = 30, allowedCharsOnLastNuntiasLabel = 37;
            if (this.singleConversationPanelList == null) this.singleConversationPanelList = new Dictionary<string, Panel>();
            if (this.conversationDetailsPanelList == null) this.conversationDetailsPanelList = new Dictionary<string, Panel>();
            if (this.singleConversationIconLabelList == null) this.singleConversationIconLabelList = new Dictionary<string, Label>();
            if (this.singleConversationNameLabelList == null) this.singleConversationNameLabelList = new Dictionary<string, Label>();
            if (this.singleConversationLastNuntiasLabelList == null) this.singleConversationLastNuntiasLabelList = new Dictionary<string, Label>();
            foreach (KeyValuePair<string, Panel> item in this.singleConversationPanelList)
            {
                item.Value.Name = "invalid";
            }
            SortedList<string, JObject> validConversationHeaderJsonList = new SortedList<string, JObject>();
            foreach (JObject chatJson in conversationHeaderJsonList)
            {
                Panel currentConversationTitlePanel;
                long conversationId = (long)chatJson["id"];
                if (!this.singleConversationPanelList.ContainsKey(conversationId.ToString()))
                {
                    this.singleConversationPanelList[chatJson["id"].ToString()] = new Panel();
                    currentConversationTitlePanel = this.singleConversationPanelList[conversationId.ToString()];

                    this.Controls.Add(currentConversationTitlePanel);

                    currentConversationTitlePanel.Width = this.parent.Width - 10;
                    Consumer consumer = ServerRequest.GetConsumer((long)chatJson["other_member_id"]); 
                    Panel conversationDetailsPanel = this.conversationDetailsPanelList[conversationId.ToString()] = new UserProfilePanel(consumer, this);
 
                    conversationDetailsPanel.Visible = false;
					this.Controls.Add(conversationDetailsPanel);

                    Label conversationIconLabel = singleConversationIconLabelList[conversationId.ToString()] = new Label();

                    conversationIconLabel.Image = new Bitmap(LocalDataFileAccess.GetConversationImageFromLocalData(chatJson["icon_file_id"].ToString(), chatJson["type"].ToString()), 50, 50);
                    conversationIconLabel.Size = conversationIconLabel.Image.Size;
                    currentConversationTitlePanel.Height = conversationIconLabel.Height + 10;
                    conversationIconLabel.Location = new Point(5, (currentConversationTitlePanel.Height - conversationIconLabel.Height) / 2);
                    currentConversationTitlePanel.Controls.Add(conversationIconLabel);

                    Label conversationNameLabel = singleConversationNameLabelList[conversationId.ToString()] = new Label();
                    conversationNameLabel.Text = chatJson["name"].ToString();
                    if (conversationNameLabel.Text.Length > allowedCharsOnNameLabel) conversationNameLabel.Text = conversationNameLabel.Text.Substring(0, allowedCharsOnNameLabel) + "...";
                    conversationNameLabel.Font = CustomFonts.Smaller;
                    conversationNameLabel.Size = conversationNameLabel.PreferredSize;
                    conversationNameLabel.Location = new Point(conversationIconLabel.Right + 5, 5);
                    currentConversationTitlePanel.Controls.Add(conversationNameLabel);

                    Label lastNuntiasLabel = singleConversationLastNuntiasLabelList[conversationId.ToString()] = new Label();
					if (chatJson.ContainsKey("last_text"))
					{
						lastNuntiasLabel.Text = chatJson["last_text"].ToString();
						if (lastNuntiasLabel.Text.Length > allowedCharsOnLastNuntiasLabel) lastNuntiasLabel.Text = lastNuntiasLabel.Text.Substring(0, allowedCharsOnLastNuntiasLabel) + "...";
					}
                    lastNuntiasLabel.Font = CustomFonts.Smallest;
                    if (chatJson["last_text_has_content"].ToString() == "true")
                    {
                        lastNuntiasLabel.Font = CustomFonts.New(CustomFonts.SmallestSize, 'i');
                        lastNuntiasLabel.ForeColor = Color.FromArgb(95, 95, 95);
                    }
                    lastNuntiasLabel.Size = lastNuntiasLabel.PreferredSize;
                    lastNuntiasLabel.Location = new Point(conversationIconLabel.Right + 5, conversationNameLabel.Bottom + 5);
                    currentConversationTitlePanel.Controls.Add(lastNuntiasLabel);

                    currentConversationTitlePanel.Height = Math.Max(currentConversationTitlePanel.PreferredSize.Height, conversationIconLabel.Height) + 10;
                    currentConversationTitlePanel.BackColor = Colors.DragengerTileColor;

                    conversationIconLabel.Click += (s, e) => { this.conversationDetailsPanelList[conversationId.ToString()].Visible = true; };
                    conversationIconLabel.MouseEnter += (s, e) => { currentConversationTitlePanel.BackColor = Color.FromArgb(currentConversationTitlePanel.BackColor.R - 25, currentConversationTitlePanel.BackColor.G - 25, currentConversationTitlePanel.BackColor.B - 25); };
                    conversationIconLabel.MouseLeave += (s, e) => { currentConversationTitlePanel.BackColor = Color.FromArgb(currentConversationTitlePanel.BackColor.R + 25, currentConversationTitlePanel.BackColor.G + 25, currentConversationTitlePanel.BackColor.B + 25); };

                    conversationNameLabel.Click += (s, e) => { this.OpenConversation(ConversationRepository.Instance.Get(conversationId)); };
                    conversationNameLabel.MouseEnter += (s, e) => { currentConversationTitlePanel.BackColor = Color.FromArgb(currentConversationTitlePanel.BackColor.R - 25, currentConversationTitlePanel.BackColor.G - 25, currentConversationTitlePanel.BackColor.B - 25); };
                    conversationNameLabel.MouseLeave += (s, e) => { currentConversationTitlePanel.BackColor = Color.FromArgb(currentConversationTitlePanel.BackColor.R + 25, currentConversationTitlePanel.BackColor.G + 25, currentConversationTitlePanel.BackColor.B + 25); };

                    lastNuntiasLabel.Click += (s, e) => { this.OpenConversation(ConversationRepository.Instance.Get(conversationId)); };
                    lastNuntiasLabel.MouseEnter += (s, e) => { currentConversationTitlePanel.BackColor = Color.FromArgb(currentConversationTitlePanel.BackColor.R - 25, currentConversationTitlePanel.BackColor.G - 25, currentConversationTitlePanel.BackColor.B - 25); };
                    lastNuntiasLabel.MouseLeave += (s, e) => { currentConversationTitlePanel.BackColor = Color.FromArgb(currentConversationTitlePanel.BackColor.R + 25, currentConversationTitlePanel.BackColor.G + 25, currentConversationTitlePanel.BackColor.B + 25); };

                    currentConversationTitlePanel.Click += (s, e) => 
                    {
                        if (ConversationPanel.CurrentDisplayedConversationPanel.TheConversation != null && ((DuetConversation)(ConversationPanel.CurrentDisplayedConversationPanel.TheConversation)).ConversationID == conversationId)
                        {
                            SlidebarPanel.MySidebarPanel.ChangeState();
                            return;
                        }
                        this.OpenConversation(ConversationRepository.Instance.Get(conversationId));
                    };
                    currentConversationTitlePanel.MouseEnter += (s, e) => { ((Panel)s).BackColor = Color.FromArgb(((Panel)s).BackColor.R - 25, ((Panel)s).BackColor.G - 25, ((Panel)s).BackColor.B - 25); };
                    currentConversationTitlePanel.MouseLeave += (s, e) => { ((Panel)s).BackColor = Color.FromArgb(((Panel)s).BackColor.R + 25, ((Panel)s).BackColor.G + 25, ((Panel)s).BackColor.B + 25); };
                }
                else { currentConversationTitlePanel = this.singleConversationPanelList[conversationId.ToString()]; }
                currentConversationTitlePanel.Name = "valid";
                validConversationHeaderJsonList[conversationId.ToString()] = chatJson;
                Console.WriteLine("top" +currentConversationTitlePanel.Top);
            }

            List<string> invalidKeyList = new List<string>();
            foreach (KeyValuePair<string, Panel> item in this.singleConversationPanelList)
            {
                if (item.Value.Name == "invalid")
                {
                    this.Controls.Remove(item.Value);
                    item.Value.Dispose();
                    invalidKeyList.Add(item.Key);
                }
            }
            foreach (string key in invalidKeyList)
            {
                this.singleConversationPanelList.Remove(key);
                this.singleConversationIconLabelList.Remove(key);
                this.singleConversationNameLabelList.Remove(key);
                this.singleConversationLastNuntiasLabelList.Remove(key);
            }

            var singleConversationPanelListRev = singleConversationPanelList.Reverse();

            foreach (KeyValuePair<string, Panel> item in singleConversationPanelListRev)
            {
                Panel singleConversationPanel = item.Value;
                singleConversationPanel.Top = previousConversationBottom + 2;
                previousConversationBottom = singleConversationPanel.Bottom;

                conversationDetailsPanelList[item.Key].Location = singleConversationPanel.Location;
                JObject chatJson = validConversationHeaderJsonList[item.Key];
                singleConversationIconLabelList[item.Key].Image = new Bitmap(LocalDataFileAccess.GetConversationImageFromLocalData(chatJson["icon_file_id"].ToString(), chatJson["type"].ToString()), new Size(50, 50));
                singleConversationNameLabelList[item.Key].Text = chatJson["name"].ToString();
                if (singleConversationNameLabelList[item.Key].Text.Length > allowedCharsOnNameLabel) singleConversationNameLabelList[item.Key].Text = singleConversationNameLabelList[item.Key].Text.Substring(0, allowedCharsOnNameLabel) + "...";
                singleConversationNameLabelList[item.Key].Width = singleConversationNameLabelList[item.Key].PreferredWidth;
				if (chatJson.ContainsKey("last_text"))
				{
					Label lastNuntiasLabel = singleConversationLastNuntiasLabelList[item.Key];
					lastNuntiasLabel.Text = chatJson["last_text"].ToString();
					if (lastNuntiasLabel.Text.Length > allowedCharsOnLastNuntiasLabel) lastNuntiasLabel.Text = lastNuntiasLabel.Text.Substring(0, allowedCharsOnLastNuntiasLabel) + "...";
				}
                singleConversationLastNuntiasLabelList[item.Key].Width = singleConversationLastNuntiasLabelList[item.Key].PreferredWidth;
            }

            this.Size = this.PreferredSize;

            foreach (JObject chatJson in conversationHeaderJsonList)
            {
                BackgroundWorker bworker = new BackgroundWorker();
                bworker.DoWork += (s, e) =>
                {
                    if (chatJson["type"].ToString() == "duet")
                    {
                        ServerRequest.SyncConsumer((long)chatJson["other_member_id"]);
                        ServerFileRequest.RefetchProfileImage(chatJson["icon_file_id"].ToString());
                        try
                        {
                            this.Invoke(new MethodInvoker(delegate
                            {
                                singleConversationIconLabelList[chatJson["id"].ToString()].Image = new Bitmap(LocalDataFileAccess.GetConversationImageFromLocalData(chatJson["icon_file_id"].ToString(), chatJson["type"].ToString()), new Size(50, 50));
                            }));
                        }
                        catch { }
                    }

                };
                bworker.RunWorkerAsync();
                bworker.RunWorkerCompleted += (s, e) => { bworker.Dispose(); };
            }
        }

        private bool OpenConversation(Conversation conversation)
        {
            if (!SlidebarPanel.MySidebarPanel.Closed) SlidebarPanel.MySidebarPanel.ChangeState();
            ConversationPanel c_panel = new ConversationPanel(Universal.ParentForm, conversation); ;
            return true;
        }

        private List<JObject> FetchConversationList()
        {
            return ConversationRepository.Instance.GetConversationsHeaderJson();
        }

        public static ConversationListPanel MyConversationListPanel
        {
            set;
            get;
        }
    }
}
