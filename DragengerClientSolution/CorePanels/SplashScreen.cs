using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ResourceLibrary;
using EntityLibrary;
using FileIOAccess;

namespace CorePanels
{
    public class SplashScreen : Panel
    {
        private static SplashScreen instance;
        private Label logoLabel;

        public SplashScreen()
        {
            this.Size = Universal.ParentForm.Size;
            this.WelcomeLabelInitialize();
            this.ShowWaitingAnimation();
        }

        private void WelcomeLabelInitialize()
        {
            logoLabel = new Label();
            Image logoImg = FileResources.Icon("logo_with_name.png");
            double logoHeight = this.Height / 3.0;
            double logoWidth = logoImg.Width * (logoHeight / logoImg.Height);
            logoLabel.Image = new Bitmap(FileResources.Icon("logo_with_name.png"), new Size((int)logoWidth, (int)logoHeight));
            logoLabel.Size = logoLabel.Image.Size;
            logoLabel.Location = new Point((this.Width - logoLabel.Width) / 2, this.Height / 10);
            this.Controls.Add(logoLabel);
        }

        public void ShowWaitingAnimation()
        {
            Size waitBarSize = new Size(logoLabel.Width, logoLabel.Height / 28);
            Point waiBarLocation = new Point((this.Width - waitBarSize.Width) / 2, logoLabel.Bottom + logoLabel.Height);
            VisualizingTools.ShowWaitingAnimation(waiBarLocation, waitBarSize, this);
        }

        new public void Show()
        {
            if (Universal.ParentForm.InvokeRequired)
            {
                Universal.ParentForm.Invoke(new Action(() =>
                { this.Show(); }));
                return;
            }
            this.Hidden = false;
            ShowWaitingAnimation();
            Universal.ParentForm.Controls.Add(this);
        }

        new public void Hide()
        {
            if (Universal.ParentForm.InvokeRequired)
            {
                Universal.ParentForm.Invoke(new Action(() =>
                { this.Hide(); }));
                return;
            }
            if (this.Hidden) return;
            Universal.ParentForm.Controls.Remove(this);
            VisualizingTools.HideWaitingAnimation();
            this.Hidden = true;
        }

        private bool Hidden
        {
            set;
            get;
        }

        public static SplashScreen Instance
        {
            get 
            {
                if (SplashScreen.instance == null) return SplashScreen.instance = new SplashScreen();
                return SplashScreen.instance;
            }
        }
    }
}
