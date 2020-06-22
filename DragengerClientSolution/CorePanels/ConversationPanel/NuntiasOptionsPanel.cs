using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ResourceLibrary;
using EntityLibrary;
using FileIOAccess;
using System.IO;
using System.Diagnostics;
using System.Media;
using System.ComponentModel;
using ServerConnections;
using System.Collections.Specialized;

namespace CorePanels
{
    internal class NuntiasOptionsPanel : Panel
    {
        internal Label deleteNuntiasLabel, copyNuntiasTextLabel;
        private LinkLabel parentNuntiasLabel;
        private Timer timerToAnimateNuntiasInfo;
        private long nuntiasId;

        private static Image deleteIcon, copyIcon;
        private static Size labelSize;

        static NuntiasOptionsPanel()
        {
            Label dummy = new Label();
            dummy.Font = CustomFonts.Smallest;
            dummy.Text = "codelete";
            labelSize = dummy.PreferredSize;

            Size iconSize = new Size(dummy.PreferredHeight - 5, dummy.PreferredHeight - 5);
            deleteIcon = new Bitmap(FileResources.Icon("deleteIcon.png"), iconSize);
            copyIcon = new Bitmap(FileResources.Icon("copyIcon.png"), iconSize);
            dummy.Dispose();
        }
        public NuntiasOptionsPanel(long nuntiasId)
        {
            this.nuntiasId = nuntiasId;
            Nuntias nuntias = SyncAssets.NuntiasSortedList[this.nuntiasId];
            this.parentNuntiasLabel = SyncAssets.ShowedNuntiasLabelSortedList[nuntias.Id];
            this.BackColor = Color.FromArgb(51, 51, 51);
            this.ForeColor = Color.FromArgb(220, 220, 220);
            if (nuntias.SenderId == Consumer.LoggedIn.Id) this.Name = "own";
            else this.Name = "other";

            copyNuntiasTextLabel = new Label();
            deleteNuntiasLabel = new Label();

            this.Controls.Add(copyNuntiasTextLabel);
            this.Controls.Add(deleteNuntiasLabel);

            copyNuntiasTextLabel.Text = "";
            copyNuntiasTextLabel.Image = copyIcon;
            copyNuntiasTextLabel.ImageAlign = ContentAlignment.MiddleLeft;
            copyNuntiasTextLabel.TextAlign = ContentAlignment.MiddleRight;
            copyNuntiasTextLabel.Font = CustomFonts.Smallest;
            copyNuntiasTextLabel.Size = labelSize;
            copyNuntiasTextLabel.MouseEnter += (s,e) => { copyNuntiasTextLabel.BackColor = Color.FromArgb(128,128,128); };
            copyNuntiasTextLabel.MouseLeave += (s,e) => { copyNuntiasTextLabel.BackColor = Color.Transparent; };
            copyNuntiasTextLabel.Click += (s,e) => 
            {
                if (nuntias.ContentFileId == null || nuntias.ContentFileId.Length == 0) Clipboard.SetText(nuntias.Text);
                else if (nuntias.Text.Length >= 5 && nuntias.Text.Substring(0, 5) == "Image") Clipboard.SetImage(Image.FromFile(LocalDataFileAccess.GetContentPathFromLocalData(nuntias.ContentFileId)));
                else if (nuntias.Text.Length >= 4 && nuntias.Text.Substring(0, 4) == "File")
                {
                    StringCollection paths = new StringCollection();
                    paths.Add(LocalDataFileAccess.GetContentPathFromLocalData(nuntias.ContentFileId));
                    Clipboard.SetFileDropList(paths);
                }
                this.ChangeNuntiasOptionPanelState();
            };
            copyNuntiasTextLabel.Visible = false;

            deleteNuntiasLabel.Text = "";
            deleteNuntiasLabel.Image = deleteIcon;
            deleteNuntiasLabel.ImageAlign = ContentAlignment.MiddleLeft;
            deleteNuntiasLabel.TextAlign = ContentAlignment.MiddleRight;
            deleteNuntiasLabel.Font = CustomFonts.Smallest;
            deleteNuntiasLabel.Size = labelSize;
            deleteNuntiasLabel.MouseEnter += (s, e) => { deleteNuntiasLabel.BackColor = Color.FromArgb(128, 128, 128); };
            deleteNuntiasLabel.MouseLeave += (s, e) => { deleteNuntiasLabel.BackColor = Color.Transparent; };
            deleteNuntiasLabel.Click += (s, e) =>
            {
                if (nuntias.SenderId == Consumer.LoggedIn.Id && Time.TimeDistanceInSecond(Time.CurrentTime,nuntias.SentTime) <= 300)
                {
                    //server allows deletion of a message that was sent 5 minutes ago or later.
                    DialogResult result = MessageBox.Show("Delete the message from both side?\r\nClick Yes. Else click No.","Message Deletion",MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Yes || result == DialogResult.No)
                    {
                        BackgroundWorker bworker = new BackgroundWorker();
                        bworker.DoWork += (ss, ee) =>
                        {
                            bool success = ServerRequest.DeleteNuntias(Consumer.LoggedIn.Id, this.NuntiasId, result == DialogResult.Yes);
                            if(success && result == DialogResult.No)
                            {
                                SyncAssets.DeleteNuntiasAssets(this.NuntiasId);
                            }
                        };
                        bworker.RunWorkerAsync();
                        bworker.RunWorkerCompleted += (ss, ee) => { bworker.Dispose(); };
                    }
                    else return;
                }
                else
                {
                    DialogResult result = MessageBox.Show("Delete the message for you?", "Message Deletion", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        BackgroundWorker bworker = new BackgroundWorker();
                        bworker.DoWork += (ss, ee) =>
                        {
                            bool success = ServerRequest.DeleteNuntias(Consumer.LoggedIn.Id, this.NuntiasId, false);
                            if(success)
                            {
                                SyncAssets.DeleteNuntiasAssets(this.NuntiasId);
                            }
                        };
                        bworker.RunWorkerAsync();
                        bworker.RunWorkerCompleted += (ss, ee) => { bworker.Dispose(); };
                    }
                    else return;
                }
                this.ChangeNuntiasOptionPanelState();
            };
            deleteNuntiasLabel.Visible = false;

            this.UpdateOptionPanel();
        }

        public void UpdateOptionPanel()
        {
            int availableTop = 0;
            Nuntias nuntias = SyncAssets.NuntiasSortedList[this.nuntiasId];
            if (nuntias.SentTime != null)
            {
                copyNuntiasTextLabel.Text = "Copy";
                if (copyNuntiasTextLabel.Text.Length > 0)
                {
                    availableTop = copyNuntiasTextLabel.Bottom;
                    copyNuntiasTextLabel.Visible = true;
                }
            }

            deleteNuntiasLabel.Text = "Delete";
            deleteNuntiasLabel.Top = availableTop - 2;
            if (deleteNuntiasLabel.Text.Length > 0)
            {
                availableTop = deleteNuntiasLabel.Bottom;
                deleteNuntiasLabel.Visible = true;
            }

            this.Size = this.PreferredSize;
            this.Visible = false;
        }

        internal void ChangeNuntiasOptionPanelState()
        {
            if (this.timerToAnimateNuntiasInfo != null)
            {
                this.timerToAnimateNuntiasInfo.Dispose();
                this.timerToAnimateNuntiasInfo = null;
            }
            this.timerToAnimateNuntiasInfo = new System.Windows.Forms.Timer();
            this.timerToAnimateNuntiasInfo.Interval = 10;
            this.Top = this.parentNuntiasLabel.Top;
            if (this.Visible == false)
            {
                this.parentNuntiasLabel.BorderStyle = BorderStyle.FixedSingle;
                if (this.parentNuntiasLabel.Image != null) this.parentNuntiasLabel.Size = this.parentNuntiasLabel.Image.Size;
                else this.parentNuntiasLabel.Size = this.parentNuntiasLabel.PreferredSize;
                if (this.Name == "own") this.Left = this.parentNuntiasLabel.Left;
                else this.Left = this.parentNuntiasLabel.Right - this.Width;
                timerToAnimateNuntiasInfo.Tick += delegate (Object sender, EventArgs e) { this.NuntiasInfoOpenAnimator(); };
                timerToAnimateNuntiasInfo.Start();
            }
            else
            {
                timerToAnimateNuntiasInfo.Tick += delegate (Object sender, EventArgs e) { this.NuntiasInfoCloseAnimator(); };
                timerToAnimateNuntiasInfo.Start();
            }
        }

        private void NuntiasInfoOpenAnimator()
        {
            int changeRate = 5;
            if (this.Name == "own")
            {
                if (this.Right <= this.parentNuntiasLabel.Right) this.Visible = true;
                if (this.Left - this.parentNuntiasLabel.Left + this.PreferredSize.Width >= changeRate) this.Left -= changeRate;
                else
                {
                    this.Left = this.parentNuntiasLabel.Left - this.PreferredSize.Width;
                    timerToAnimateNuntiasInfo.Stop();
                    this.timerToAnimateNuntiasInfo.Dispose();
                    this.timerToAnimateNuntiasInfo = null;
                    return;
                }
            }
            else
            {
                if (this.Left >= this.parentNuntiasLabel.Left) this.Visible = true;
                if (this.parentNuntiasLabel.Right - this.Left >= changeRate) this.Left += changeRate;
                else
                {
                    this.Left = this.parentNuntiasLabel.Right;
                    timerToAnimateNuntiasInfo.Stop();
                    this.timerToAnimateNuntiasInfo.Dispose();
                    this.timerToAnimateNuntiasInfo = null;
                    return;
                }
            }
        }

        private void NuntiasInfoCloseAnimator()
        {
            int changeRate = 5;
            if (this.Name == "own")
            {
                if (this.parentNuntiasLabel.Left - this.Left < changeRate || this.Right + changeRate > this.parentNuntiasLabel.Right)
                {
                    this.Left = this.parentNuntiasLabel.Left;
                    timerToAnimateNuntiasInfo.Stop();
                    this.Visible = false;
                    this.parentNuntiasLabel.BorderStyle = BorderStyle.None;
                    if (this.parentNuntiasLabel.Image != null) this.parentNuntiasLabel.Size = this.parentNuntiasLabel.Image.Size;
                    else this.parentNuntiasLabel.Size = this.parentNuntiasLabel.PreferredSize;
                    return;
                }
                this.Left += changeRate;
            }
            else
            {
                if (this.Right - this.parentNuntiasLabel.Right < changeRate || this.Left - changeRate < this.parentNuntiasLabel.Left)
                {
                    this.Left = this.parentNuntiasLabel.Right - this.Width;
                    timerToAnimateNuntiasInfo.Stop();
                    this.Visible = false;
                    this.parentNuntiasLabel.BorderStyle = BorderStyle.None;
                    if (this.parentNuntiasLabel.Image != null) this.parentNuntiasLabel.Size = this.parentNuntiasLabel.Image.Size;
                    else this.parentNuntiasLabel.Size = this.parentNuntiasLabel.PreferredSize;
                    return;
                }
                this.Left -= changeRate;
            }
        }

        public long NuntiasId
        {
            set { this.nuntiasId = value; }
            get { return this.nuntiasId; }
        }
    }
}
