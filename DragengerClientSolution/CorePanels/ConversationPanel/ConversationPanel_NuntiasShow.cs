using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ResourceLibrary;
using EntityLibrary;
using System.Diagnostics;
using FileIOAccess;
using System.IO;

namespace CorePanels
{
    public partial class ConversationPanel : Panel
    {
        public void ShowNuntias(Nuntias nuntias, bool playConversationInnerSound)
        {
            string processesedText = Universal.ProcessStringToShow(nuntias.Text, this.maxCharactersPerLine);
            if (Universal.ParentForm.InvokeRequired)
            {
                Universal.ParentForm.Invoke(new Action(() =>
                {
                    ShowNuntiasTasks(nuntias, playConversationInnerSound, processesedText);
                    ConversationListPanel.MyConversationListPanel.RefreshConversationList();
                }));
            }
            else
            {
                ShowNuntiasTasks(nuntias, playConversationInnerSound, processesedText);
                ConversationListPanel.MyConversationListPanel.RefreshConversationList();
            }
        }

        private void ShowNuntiasTasks(Nuntias nuntias, bool playConversationInnerSound, string processesedText)
        {
            bool nuntiasIsImage = (nuntias.ContentFileId != null && nuntias.ContentFileId.Length > 0 && nuntias.Text.Length >= 5 && nuntias.Text.Substring(0,5) == "Image");     //Nuntias.Text format for image file is 'Image: image_file_name'
            SyncAssets.NuntiasSortedList[nuntias.Id] = nuntias;
            if (playConversationInnerSound) ConversationPanel.newMessageSound.Play();
            bool newTimeStampShowed = this.ShowTimeStampIfNecessary(nuntias);
            bool profileImageShowed = this.ShowProfileImageIfNecessary(nuntias, newTimeStampShowed);
            LinkLabel nuntiasLabel = SyncAssets.ShowedNuntiasLabelSortedList[nuntias.Id] = new LinkLabel();
            nuntiasLabel.Name = nuntias.Id.ToString();
            nuntiasLabel.LinkArea = new LinkArea(0, 0);
            if (nuntias.ContentFileId != null && nuntias.ContentFileId.Length > 0)
            {
                try
                {
                    if(nuntias.ContentFileId == "deleted")
                    {
                        nuntiasLabel.Text = processesedText;
                        nuntiasLabel.ForeColor = Color.FromArgb(95, 95, 95);
                        nuntiasLabel.Padding = new Padding(7, 7, 7, 7);
                        nuntiasLabel.Font = CustomFonts.New(CustomFonts.SmallerSize, 'i');
                        nuntiasLabel.Size = nuntiasLabel.PreferredSize;
                    }
                    else
                    {
                        string contentFilePath = LocalDataFileAccess.GetContentPathFromLocalData(nuntias.ContentFileId);
                        if (nuntiasIsImage)
                        {
                            if(contentFilePath != null)
                            {
                                using (FileStream imgStream = new FileStream(contentFilePath, FileMode.Open))
                                {
                                    Image img = Image.FromStream(imgStream);
                                    nuntiasLabel.Image = new Bitmap(img, new Size((int)((150.0 / img.Height) * img.Width), 150));
                                    img.Dispose();
                                    imgStream.Close();
                                    nuntiasLabel.Size = nuntiasLabel.Image.Size;
                                }
                            }
                            nuntiasLabel.Padding = new Padding(7, 7, 7, 7);
                        }
                        else
                        {
                            nuntiasLabel.Text = processesedText;
                            nuntiasLabel.LinkArea = new LinkArea(6, nuntiasLabel.Text.Length - 6);
                            nuntiasLabel.LinkColor = Color.FromArgb(94, 92, 0);
                            nuntiasLabel.LinkClicked += delegate (Object sender, LinkLabelLinkClickedEventArgs e) 
                            {
                                try
                                {
                                    Process.Start(contentFilePath);
                                    nuntiasLabel.LinkVisited = true;
                                }
                                catch
                                {
                                    Universal.ShowErrorMessage("Failed to open the file!");
                                }
                            };
                            nuntiasLabel.ForeColor = Color.FromArgb(95, 95, 95);
                            nuntiasLabel.Padding = new Padding(7, 7, 7, 7);
                            nuntiasLabel.Font = CustomFonts.New(CustomFonts.SmallerSize, 'i');
                            nuntiasLabel.Size = nuntiasLabel.PreferredSize;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception occured in conversation panel ShowNuntias() method due to: " + e.Message + " exception type " + e.GetType());
                }
            }
            else
            {
                nuntiasLabel.Text = processesedText;
                Universal.SetLinkAreaIfLinkFound(nuntiasLabel);
                nuntiasLabel.LinkClicked += delegate(Object sender, LinkLabelLinkClickedEventArgs e) 
                {
                    try
                    {
                        Process.Start(e.Link.LinkData.ToString()); ; e.Link.Visited = true;
                    }
                    catch
                    {
                        Universal.ShowErrorMessage("Failed to open the link!");
                    }
                };
                nuntiasLabel.Font = CustomFonts.Smaller;
                nuntiasLabel.Padding = new Padding(7, 7, 7, 7);
                nuntiasLabel.Size = nuntiasLabel.PreferredSize;
            }

            if (!nuntiasIsImage && nuntias.ContentFileId != "deleted")
            {
                if (nuntias.SenderId == Consumer.LoggedIn.Id) nuntiasLabel.BackColor = Color.FromArgb(231, 255, 234);
                else nuntiasLabel.BackColor = Color.FromArgb(229, 231, 255);
            }
            if (profileImageShowed) nuntiasLabel.Top = this.lastShowedLabel.Bottom - this.profileImageSize.Height + 5;
            else if (this.lastShowedNuntias == null || this.lastShowedNuntias.SenderId != nuntias.SenderId) nuntiasLabel.Top = this.lastShowedLabel.Bottom + 25;
            else nuntiasLabel.Top = this.lastShowedLabel.Bottom + 5;
            if (nuntias.SenderId == Consumer.LoggedIn.Id) nuntiasLabel.Left = this.selfProfileImageLocation.X - nuntiasLabel.Width - this.profileImageSize.Width / 4;
            else nuntiasLabel.Left = this.othersProfileImageLocation.X + this.profileImageSize.Width + this.profileImageSize.Width / 4;
            this.nuntiasBossPanel.Controls.Add(nuntiasLabel);
            this.lastShowedNuntias = nuntias;
            this.lastShowedLabel = nuntiasLabel;

            int x = 0;
            Timer makeXzeroTimer = new Timer() { Interval = 500 };
            makeXzeroTimer.Tick += delegate(Object sender, EventArgs e) { x = 0; };

            NuntiasInfoPanel respectiveInfoPanel = null;
            NuntiasOptionsPanel respectiveOptionPanel = null;
            if (nuntias.ContentFileId != "deleted")
            {
                respectiveInfoPanel = SyncAssets.NuntiasInfoPanelSortedList[nuntias.Id] = new NuntiasInfoPanel(nuntias.Id);
                this.nuntiasBossPanel.Controls.Add(respectiveInfoPanel);

                respectiveOptionPanel = SyncAssets.NuntiasOptionPanelSortedList[nuntias.Id] = new NuntiasOptionsPanel(nuntias.Id);
                this.nuntiasBossPanel.Controls.Add(respectiveOptionPanel);
            }

            nuntiasLabel.MouseEnter += (s, me) =>
            {
                if (nuntiasIsImage) return;
                Color current = ((Label)s).BackColor;
                if (current == Color.Transparent) return;
                ((Label)s).BackColor = Color.FromArgb(current.R - 25, current.G - 25, current.B - 25);
            };
            nuntiasLabel.MouseLeave += (s, me) =>
            {
                if (nuntiasIsImage) return;
                Color current = ((Label)s).BackColor;
                if (current == Color.Transparent) return;
                ((Label)s).BackColor = Color.FromArgb(current.R + 25, current.G + 25, current.B + 25);
            };
            nuntiasLabel.MouseClick += delegate(Object sender, MouseEventArgs e)
            {
                if (respectiveOptionPanel == null) return;
                if (respectiveInfoPanel == null) return;
                if (e.Button == MouseButtons.Left)
                {
                    if (nuntias.ContentFileId != null && nuntias.ContentFileId.Length > 0)
                    {
                        makeXzeroTimer.Start();
                        x++;
                    }
                    if (x == 2)
                    {
                        if (nuntias.ContentFileId != null && nuntias.ContentFileId.Length > 0)
                        {
                            try
                            {
                                Process.Start(LocalDataFileAccess.GetFilePathInLocalData(nuntias.ContentFileId));
                            }
                            catch
                            {
                                Universal.ShowErrorMessage("Failed to open the file!");
                            }
                        }
                        makeXzeroTimer.Stop();
                        x = 0;
                    }
                    if (respectiveOptionPanel.Visible) respectiveOptionPanel.ChangeNuntiasOptionPanelState();
                    respectiveInfoPanel.ChangeNuntiasInfoPanelState();
                    this.OnClick(sender, e);
                }
                else
                {
                    if (respectiveInfoPanel.Visible) respectiveInfoPanel.ChangeNuntiasInfoPanelState();
                    respectiveOptionPanel.ChangeNuntiasOptionPanelState();
                    this.OnClick(sender, e);
                }
            };

            Label keepSpaceAtBottom = new Label();
            keepSpaceAtBottom.Location = new Point(0, this.lastShowedLabel.Bottom + 25);
            keepSpaceAtBottom.Size = new Size(0, 0);
            this.nuntiasBossPanel.Controls.Add(keepSpaceAtBottom);
            FilePreviewPanelHeight(0);
            this.nuntiasBossPanel.VerticalScroll.Value = this.nuntiasBossPanel.VerticalScroll.Maximum;
        }

        private bool ShowProfileImageIfNecessary(Nuntias nuntias, bool newTimeStampShowed)
        {
            if (this.lastShowedNuntias == null || this.lastShowedNuntias.SenderId != nuntias.SenderId)
            {
                Label profileImageLabel = new Label();
                profileImageLabel.Image = null;
                if (nuntias.SenderId == this.receiver.Id) profileImageLabel.Image = new Bitmap(this.receiver.ProfileImage, this.profileImageSize);
                else if (nuntias.SenderId == Consumer.LoggedIn.Id) profileImageLabel.Image = new Bitmap(Consumer.LoggedIn.ProfileImage, this.profileImageSize);
                profileImageLabel.Size = this.profileImageSize;
                if (nuntias.SenderId == Consumer.LoggedIn.Id) profileImageLabel.Left = this.selfProfileImageLocation.X;
                else profileImageLabel.Left = this.othersProfileImageLocation.X;
                if (newTimeStampShowed) profileImageLabel.Top = this.nuntiasBossPanel.PreferredSize.Height;
                else profileImageLabel.Top = this.lastShowedLabel.Bottom + this.profileImageSize.Height;

                Label keepSpaceAtBottom = new Label();
                keepSpaceAtBottom.Location = new Point(0, this.lastShowedLabel.Bottom + 25);
                keepSpaceAtBottom.Size = new Size(0, 0);

                if (Universal.ParentForm.InvokeRequired)
                {
                    Universal.ParentForm.Invoke(new Action(() =>
                    {
                        this.nuntiasBossPanel.Controls.Add(profileImageLabel);
                        this.lastShowedLabel = profileImageLabel;
                        this.nuntiasBossPanel.Controls.Add(keepSpaceAtBottom);
                    }));
                }
                else
                {
                    this.nuntiasBossPanel.Controls.Add(profileImageLabel);
                    this.lastShowedLabel = profileImageLabel;
                    this.nuntiasBossPanel.Controls.Add(keepSpaceAtBottom);
                }

                return true;
            }
            return false;
        }

        private bool ShowTimeStampIfNecessary(Nuntias nuntias)
        {
            if (this.lastShowedNuntias == null || Time.TimeDistanceInMinute(this.lastShowedNuntias.SentTime, nuntias.SentTime) > 5)
            {
                Label timeLabel = new Label();
                timeLabel.Text = nuntias.SentTime.DateTimeLong;
                timeLabel.Size = timeLabel.PreferredSize;
                if (this.lastShowedNuntias == null) timeLabel.Top = 20;
                else if (this.lastShowedNuntias.SenderId != nuntias.SenderId) timeLabel.Top = this.lastShowedLabel.Bottom + 20;
                else timeLabel.Top = this.lastShowedLabel.Bottom + 10;
                timeLabel.Left = (this.nuntiasBossPanel.Width - timeLabel.Width) / 2;
                if (Universal.ParentForm.InvokeRequired) Universal.ParentForm.Invoke(new MethodInvoker(delegate
                {
                    this.nuntiasBossPanel.Controls.Add(timeLabel);
                }));
                else this.nuntiasBossPanel.Controls.Add(timeLabel);
                this.lastShowedLabel = timeLabel;

                return true;
            }
            return false;
        }
    }
}
