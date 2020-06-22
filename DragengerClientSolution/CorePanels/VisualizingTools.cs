using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileIOAccess;
using ResourceLibrary;

namespace CorePanels
{
    public static class VisualizingTools
    {
        //waiting Animator Essentials
        private static Panel waitBar;
        private static Panel waitBarParent;
        private static Timer timer;
        private static bool showing;
        public static void ShowWaitingAnimation(Point location, Size size, Panel parent)
        {
            waitBarParent = parent;
            waitBar = new Panel();
            waitBar.Size = size;
            waitBar.Location = location;
            waitBar.BackColor = Color.FromArgb(72, 68, 65);

            Label runningStick = new Label();
            runningStick.Width = waitBar.Width / 5;
            runningStick.Height = waitBar.Height;
            runningStick.Image = new Bitmap(FileResources.Icon("loading.png"), runningStick.Size);
            runningStick.Top = (waitBar.Height - runningStick.Height) / 2;
            runningStick.Left = 0;
            if (!Universal.ParentForm.InvokeRequired)
            {
                waitBar.Controls.Add(runningStick);
                waitBarParent.Controls.Add(waitBar);
            }
            else
            {
                Universal.ParentForm.Invoke(new Action(() =>
                {
                    waitBar.Controls.Add(runningStick);
                    waitBarParent.Controls.Add(waitBar);
                }));
            }

            timer = new Timer();
            timer.Interval = 30;
            bool forward = true;
            timer.Tick += delegate(Object sender, EventArgs e)
            {
                if (forward)
                {
                    if (waitBar.Width - runningStick.Right >= 5) runningStick.Left += 5;
                    else
                    {
                        runningStick.Left = waitBar.Width - runningStick.Width;
                        forward = false;
                    }
                }
                else
                {
                    if (runningStick.Left >= 5) runningStick.Left -= 5;
                    else
                    {
                        runningStick.Left = 0;
                        forward = true;
                    }
                }

            };
            showing = true;
            timer.Start();
        }

        public static void HideWaitingAnimation()
        {
            if (showing)
            {
                if (waitBarParent.InvokeRequired)
                {
                    waitBarParent.Invoke(new MethodInvoker(delegate
                    {
                        timer.Stop();
                        waitBar.Dispose();
                        showing = false;
                    }));
                }
                else
                {
                    timer.Stop();
                    waitBar.Dispose();
                    showing = false;
                }
            }
        }
    }
}
