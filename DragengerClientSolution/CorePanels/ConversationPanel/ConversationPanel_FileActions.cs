using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ResourceLibrary;
using EntityLibrary;
using FileIOAccess;
using System.IO;
using System.ComponentModel;
using ServerConnections;
using LocalRepository;

namespace CorePanels
{
    public partial class ConversationPanel
    {
        private Label fileNameLabel, imgPreview;
        private Button sendFileButton, cancelFileButton;

        private void SendTypedNuntias()
        {
            string rawText = this.nuntiasTextBox.Text;
            BackgroundWorker loaderWorker = new BackgroundWorker();
            Nuntias newNuntias = null;
            loaderWorker.DoWork += (s, e) =>
            {
                try
                {
                    string processedText = Universal.ProcessValidMessageText(rawText);
                    if (processedText == null) return;
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
                    newNuntias = new Nuntias(processedText, User.LoggedIn.Id, Time.CurrentTime, this.theConversation.ConversationID);
                    long? nuntiasTmpID = NuntiasRepository.Instance.StoreTmpNuntias(newNuntias);
                    if (nuntiasTmpID == null) return;
                    newNuntias.Id = nuntiasTmpID ?? 0;
                    SyncAssets.NuntiasSortedList[(long)nuntiasTmpID] = newNuntias;
                    this.Invoke(new Action(() => 
                    {
                        this.nuntiasTextBox.Clear();
                    }));
                    this.ShowNuntias(newNuntias, true);
                    BackendManager.SendPendingNuntii();
                }
                catch (Exception ex) { Console.WriteLine("Exception in SendTypedNuntias() => " + ex.Message); }
            };
            loaderWorker.RunWorkerAsync();
            loaderWorker.RunWorkerCompleted += (s, e) => { loaderWorker.Dispose(); };
            if (ConversationListPanel.MyConversationListPanel != null) ConversationListPanel.MyConversationListPanel.RefreshConversationList();
        }

        private void SendChoosenFile(string choosenSafeFileName, string localPath, string extension)
        {
            if (File.ReadAllBytes(localPath).Length > 1024 * 1024 * 5)
            {
                Universal.ShowErrorMessage("Please choose a file below 5 MB.");
                return;
            }
            this.sendFileButton.Visible = false;
            this.cancelFileButton.Visible = false;
            int animationLeft = 0, animationPadding = 40;
            if (imgPreview != null) animationLeft = imgPreview.Right + animationPadding;
            else animationLeft = Math.Max(animationLeft, fileNameLabel.Right + animationPadding);
            VisualizingTools.ShowWaitingAnimation(new Point(animationLeft, (filePreviewPanel.Height - sendButton.Height / 2) / 2), new Size(this.filePreviewPanel.Right - animationLeft - animationPadding - 20, sendButton.Height / 2), filePreviewPanel);
            BackgroundWorker loaderWorker = new BackgroundWorker();
            loaderWorker.DoWork += (s, e) =>
            {
                try
                {
                    using (FileStream gotFileStream = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        string fileIdName = this.theConversation.ConversationID + "_" + Universal.SystemMACAddress + "_" + Time.CurrentTime.TimeStampString + "-" + choosenSafeFileName;
                        MemoryStream gotFileMemoryStream = new MemoryStream();
                        gotFileStream.CopyTo(gotFileMemoryStream);
                        bool storeToLocalSucceed = LocalDataFileAccess.SaveNuntiasContentToLocal(gotFileMemoryStream, fileIdName);
                        if (storeToLocalSucceed)
                        {
                            if (this.theConversation == null)
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
                            Nuntias newNuntias = new Nuntias("File: " + choosenSafeFileName, Consumer.LoggedIn.Id, Time.CurrentTime, this.theConversation.ConversationID);
                            newNuntias.ContentFileId = fileIdName;
                            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp") newNuntias.Text = "Image : " + choosenSafeFileName;
                            long? nuntiasTmpID = NuntiasRepository.Instance.StoreTmpNuntias(newNuntias);
                            if (nuntiasTmpID == null) newNuntias = null;
                            else newNuntias.Id = nuntiasTmpID ?? 0;
                            if (newNuntias == null) return;

                            this.ShowNuntias(newNuntias, true);
                            BackendManager.SendPendingNuntii();

                            string localStoredPath = LocalDataFileAccess.GetContentPathFromLocalData(fileIdName);

                            Universal.ParentForm.Invoke(new Action(() =>
                            {
                                VisualizingTools.HideWaitingAnimation();
                                if (localStoredPath != null)
                                {
                                    this.FilePreviewPanelHeight(0);
                                }
                                else Universal.ShowErrorMessage("Failed to send the file!");
                            }));

                            gotFileStream.Close();
                            gotFileMemoryStream.Dispose();
                            loaderWorker.Dispose();
                            this.DisposeUnmanaged();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured while sending a file: " + ex.Message);
                };
            };
            loaderWorker.RunWorkerAsync();
            loaderWorker.RunWorkerCompleted += (s,e) => { loaderWorker.Dispose(); };
            if (ConversationListPanel.MyConversationListPanel != null) ConversationListPanel.MyConversationListPanel.RefreshConversationList();
        }

        private void FilePreviewPanelHeight(int height)
        {
            try { this.Controls.Remove(filePreviewPanel); filePreviewPanel.Dispose(); }
            catch { }
            if (height <= 0)
            {
                this.DisposeUnmanaged();
                this.nuntiasTextBox.Visible = true;
                this.sendButton.Visible = true;
                this.parent.AcceptButton = sendButton;
                this.parent.CancelButton = null;
                this.nuntiasTextBox.Clear();
                this.nuntiasTextBox.Multiline = false;
                this.nuntiasTextBox.Multiline = true;
                this.AdjustTypingBarSize();
                return;
            }
            this.filePreviewPanel = new Panel();
            this.filePreviewPanel.Width = this.typingSpaceBar.Width;
            this.filePreviewPanel.Left = 0;
            this.Controls.Add(filePreviewPanel);
            filePreviewPanel.Height = height;
            filePreviewPanel.Top = this.typingSpaceBar.Top - filePreviewPanel.Height;
            if (this.nuntiasBossPanel != null)
            {
                this.nuntiasBossPanel.Height = this.Height - (this.conversationTitleBar.Height + this.typingSpaceBar.Height + filePreviewPanel.Height + 8);
                this.nuntiasSpaceParentPanel.Height = this.nuntiasBossPanel.Height;
                this.dropPromptLabel.Height = this.nuntiasBossPanel.Height;
                this.nuntiasBossPanel.VerticalScroll.Value = this.nuntiasBossPanel.VerticalScroll.Maximum;
            }
        }

        private void ChooseFileAndShowPreview()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = FileResources.WindowsDocumentsPath;
            fileDialog.ShowDialog();
            if (fileDialog.FileName.Length == 0) return;
            string choosenSafeFileName = fileDialog.SafeFileName;
            ShowNuntiasContentPreview(choosenSafeFileName, fileDialog.FileName);
        }

        private void TakeSnapshotOfForm()
        {
            string path = null;
            using (Bitmap snapshotBmp = new Bitmap(Universal.ParentForm.Width, Universal.ParentForm.Height))
            {
                Universal.ParentForm.DrawToBitmap(snapshotBmp, new Rectangle(0, 0, snapshotBmp.Width, snapshotBmp.Height));
                path = FileResources.TempFolderPath + "snapshot-" + Time.CurrentTime.TimeStampString + ".bmp";
                snapshotBmp.Save(path);
            }
            if(path != null)
            {
                ShowNuntiasContentPreview(Path.GetFileName(path), path);
            }
        }

        private void ShowNuntiasContentPreview(string choosenSafeFileName, string localPath)
        {
            this.nuntiasTextBox.Multiline = false;
            this.nuntiasTextBox.Multiline = true;
            this.nuntiasTextBox.Visible = false;
            this.sendButton.Visible = false;

            int availableTop = 0;
            string extension = Path.GetExtension(localPath).ToLower();
            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp")
            {
                int targetHeight = sendButton.Height * 5;
                imgPreview = new Label();
                imgPreview.Image = GraphicsStudio.ResizeImageByHeight(Image.FromFile(localPath), targetHeight);
                imgPreview.Size = imgPreview.Image.Size;
                FilePreviewPanelHeight(imgPreview.Height + 40);
                imgPreview.Location = new Point(10, 5);
                availableTop = 5 + imgPreview.Height;
                this.filePreviewPanel.Controls.Add(imgPreview);
            }
            else FilePreviewPanelHeight(40);
            fileNameLabel = new Label();
            fileNameLabel.Text = this.ReformatFileNameString(choosenSafeFileName);
            fileNameLabel.Font = CustomFonts.New(CustomFonts.SmallerSize, 'i');
            fileNameLabel.ForeColor = Color.FromArgb(102, 51, 0);
            fileNameLabel.Size = fileNameLabel.PreferredSize;
            this.filePreviewPanel.Controls.Add(fileNameLabel);

            Label attachmentLabel = new Label();
            Image attachImg = FileResources.Icon("attachment.png");
            double attachHght = attachImg.Size.Height, targetHght = fileNameLabel.Height;
            attachmentLabel.Image = new Bitmap(attachImg, new Size((int)((targetHght / attachHght) * attachImg.Size.Width), (int)targetHght));
            attachmentLabel.Size = attachmentLabel.Image.Size;
            attachmentLabel.Location = new Point(10, availableTop + 5);
            fileNameLabel.Location = new Point(attachmentLabel.Right + 5, availableTop + 5);
            this.filePreviewPanel.Controls.Add(attachmentLabel);

            Size fileButtonSize = new Size(filePreviewPanel.Height * 7 / 10, filePreviewPanel.Height * 7 / 10);
            if (imgPreview != null && imgPreview.Visible) fileButtonSize = new Size(filePreviewPanel.Height * 2 / 5, filePreviewPanel.Height * 2 / 5);

            sendFileButton = FileButtonsDefine("sendfile", fileButtonSize);
            this.parent.AcceptButton = sendFileButton;
            sendFileButton.Location = new Point(filePreviewPanel.Right - sendFileButton.Width - 30, (filePreviewPanel.Height - sendFileButton.Height)/2);
            sendFileButton.Click += (s, e) => { this.SendChoosenFile(choosenSafeFileName, localPath, extension); };

            cancelFileButton = this.FileButtonsDefine("cancelfile", fileButtonSize);
            this.parent.CancelButton = cancelFileButton;
            cancelFileButton.Location = new Point(sendFileButton.Left - cancelFileButton.Width, (filePreviewPanel.Height - cancelFileButton.Height) / 2);
            cancelFileButton.Click += delegate(Object sender, EventArgs e)
            {
                this.FilePreviewPanelHeight(0);
            };
        }

        private Button FileButtonsDefine(string buttonName, Size buttonImgSize)
        {
            Button newFileButton = new Button();
            newFileButton.Image = new Bitmap(FileResources.Icon(buttonName + ".png"), buttonImgSize);
            newFileButton.FlatStyle = FlatStyle.Flat;
            newFileButton.FlatAppearance.MouseOverBackColor = newFileButton.BackColor;
            newFileButton.BackColorChanged += (s, e) => { newFileButton.FlatAppearance.MouseOverBackColor = newFileButton.BackColor; };
            newFileButton.FlatAppearance.BorderSize = 0;
            newFileButton.TabStop = false;
            newFileButton.Height = newFileButton.PreferredSize.Height;
            newFileButton.Width = newFileButton.Height + newFileButton.Height / 2;
            newFileButton.ImageAlign = ContentAlignment.MiddleCenter;
            filePreviewPanel.Controls.Add(newFileButton);
            newFileButton.MouseEnter += delegate (Object sender, EventArgs e)
            {
                newFileButton.Image = new Bitmap(FileResources.Icon(buttonName + "_rev.png"), newFileButton.Image.Size);
            };
            newFileButton.MouseLeave += delegate (Object sender, EventArgs e)
            {
                newFileButton.Image = new Bitmap(FileResources.Icon(buttonName + ".png"), newFileButton.Image.Size);
            };
            return newFileButton;
        }

        private string ReformatFileNameString(string safeFileName)
        {
            string extention = Path.GetExtension(safeFileName);
            string nameWithoutExtention = safeFileName.Substring(0, safeFileName.Length - extention.Length);
            if(safeFileName.Length > 30)
            {
                if (extention.Length > 20)
                {
                    extention = extention.Substring(0, 4);
                    extention += "...";
                }
                nameWithoutExtention = nameWithoutExtention.Substring(0, 30 - extention.Length);
                nameWithoutExtention += "... ";
            }
            return nameWithoutExtention + extention;
        }

        private void DisposeUnmanaged()
        {
            if (sendFileButton != null && sendFileButton.Image != null) sendFileButton.Image.Dispose();
            if (cancelFileButton != null && cancelFileButton.Image != null) cancelFileButton.Image.Dispose();
            if (imgPreview != null && imgPreview.Image != null) imgPreview.Image.Dispose();

            sendFileButton = null;
            cancelFileButton = null;
            imgPreview = null;
        }
    }
}
