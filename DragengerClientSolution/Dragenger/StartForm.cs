using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ResourceLibrary;
using CorePanels;
using System.Threading;
using ProcessManagement;

namespace Dragenger
{
    public class StartForm : Form
    {
        public static BackgroundWorker backgroundWorker;
        public StartForm()
        {
            this.InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void StartForm_Load(object sender, EventArgs ea)
        {
            this.TopMost = false;
            this.RedefineStartFormProperties();
            Universal.ParentForm = this;
            SplashScreen.Instance.Show();

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (s, e) =>
            {
                ConnectionManager.EstablishConnection();
                BackendManager.LoginProcessRun();
            };
            backgroundWorker.RunWorkerAsync();
        }

        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // StartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 755);
            this.MaximizeBox = false;
            this.Name = "StartForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome | Dragenger";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.StartForm_Load);
            this.FormClosing += StartForm_FormClosing;
            this.ResumeLayout(false);
        }

        private void StartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            BackendManager.KeepRefreshingRunning = false;
            Application.Exit();
        }

        private void RedefineStartFormProperties()
        {
            this.SuspendLayout();
            // 
            // StartForm
            // 
            Resolutions.SetDefaultFormSize();
            this.ClientSize = Resolutions.DefaultFormSize;
            this.ResumeLayout(false);
        }
    }
}
