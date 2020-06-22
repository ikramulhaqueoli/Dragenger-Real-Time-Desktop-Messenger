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

namespace CorePanels
{
    internal class NuntiasInfoPanel : Panel
    {
        internal Label sentTimeLabel, deliveredTimeLabel, seenTimeLabel;
        private LinkLabel parentNuntiasLabel;
        private Timer timerToAnimateNuntiasInfo;
        private long nuntiasId;

        private static Image sentIcon, deliveredIcon, seenIcon;
        private static Size labelSize;

        static NuntiasInfoPanel()
        {
            Label dummy = new Label();
            dummy.Font = CustomFonts.Smallest;
            dummy.Text = "1234567890";
            labelSize = dummy.PreferredSize;

            Size iconSize = new Size(dummy.PreferredHeight - 5, dummy.PreferredHeight - 5);
            sentIcon = new Bitmap(FileResources.Icon("sentIcon.png"), iconSize);
            deliveredIcon = new Bitmap(FileResources.Icon("deliveredIcon.png"), iconSize);
            seenIcon = new Bitmap(FileResources.Icon("seenIcon.png"), iconSize);
            dummy.Dispose();
        }
        public NuntiasInfoPanel(long nuntiasId)
        {
            this.nuntiasId = nuntiasId;
            Nuntias nuntias = SyncAssets.NuntiasSortedList[this.nuntiasId];
            this.parentNuntiasLabel = SyncAssets.ShowedNuntiasLabelSortedList[nuntias.Id];
            this.BackColor = Color.FromArgb(51, 51, 51);
            this.ForeColor = Color.FromArgb(220, 220, 220);
            if (nuntias.SenderId == Consumer.LoggedIn.Id) this.Name = "own";
            else this.Name = "other";

            sentTimeLabel = new Label();
            deliveredTimeLabel = new Label();
            seenTimeLabel = new Label();

            this.Controls.Add(sentTimeLabel);
            this.Controls.Add(deliveredTimeLabel);
            this.Controls.Add(seenTimeLabel);

            sentTimeLabel.Text = "";
            sentTimeLabel.Image = sentIcon;
            sentTimeLabel.ImageAlign = ContentAlignment.MiddleLeft;
            sentTimeLabel.TextAlign = ContentAlignment.MiddleRight;
            sentTimeLabel.Font = CustomFonts.Smallest;
            sentTimeLabel.Size = labelSize;
            sentTimeLabel.Visible = false;

            deliveredTimeLabel.Text = "";
            deliveredTimeLabel.Image = deliveredIcon;
            deliveredTimeLabel.ImageAlign = ContentAlignment.MiddleLeft;
            deliveredTimeLabel.TextAlign = ContentAlignment.MiddleRight;
            deliveredTimeLabel.Font = CustomFonts.Smallest;
            deliveredTimeLabel.Size = labelSize;
            deliveredTimeLabel.Visible = false;

            seenTimeLabel.Text = "";
            seenTimeLabel.Image = seenIcon;
            seenTimeLabel.ImageAlign = ContentAlignment.MiddleLeft;
            seenTimeLabel.TextAlign = ContentAlignment.MiddleRight;
            seenTimeLabel.Font = CustomFonts.Smallest;
            seenTimeLabel.Size = labelSize;
            seenTimeLabel.Visible = false;

            this.UpdateInfoPanel();
        }

        public void UpdateInfoPanel()
        {
            int availableTop = 0;
            Nuntias nuntias = SyncAssets.NuntiasSortedList[this.nuntiasId];
            if (nuntias.SentTime != null)
            {
                sentTimeLabel.Text = nuntias.SentTime.Time12;
                if (nuntias.Id < 0) sentTimeLabel.Text = "sending...";
                if (sentTimeLabel.Text.Length > 0)
                {
                    availableTop = sentTimeLabel.Bottom;
                    sentTimeLabel.Visible = true;
                }
            }

            if (nuntias.SeenTime != null)
            {
                seenTimeLabel.Text = nuntias.SeenTime.Time12;
                seenTimeLabel.Top = availableTop - 2;
                if (seenTimeLabel.Text.Length > 0)
                {
                    availableTop = seenTimeLabel.Bottom;
                    seenTimeLabel.Visible = true;
                }
            }
            else if (nuntias.DeliveryTime != null)
            {
                deliveredTimeLabel.Text = nuntias.DeliveryTime.Time12;
                deliveredTimeLabel.Top = availableTop - 2;
                if (deliveredTimeLabel.Text.Length > 0)
                {
                    availableTop = deliveredTimeLabel.Bottom;
                    deliveredTimeLabel.Visible = true;
                }
            }

            this.Size = this.PreferredSize;
            this.Visible = false;
        }

        internal void ChangeNuntiasInfoPanelState()
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
                timerToAnimateNuntiasInfo.Tick += delegate(Object sender, EventArgs e) { this.NuntiasInfoOpenAnimator(); };
                timerToAnimateNuntiasInfo.Start();
            }
            else
            {
                timerToAnimateNuntiasInfo.Tick += delegate(Object sender, EventArgs e) { this.NuntiasInfoCloseAnimator(); };
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
