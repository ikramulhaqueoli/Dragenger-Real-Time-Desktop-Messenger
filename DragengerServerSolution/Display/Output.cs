using EntityLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Display
{
    public class Output
    {
        private static string ServerLog { set; get; }
        private static int LogCount { set; get; }

        public static void ShowLog(Object log)
        {
            string logString = log.ToString();
            Output.LogCount++;
            if (LogTextboxContainer.WorkingInstance.InvokeRequired)
            {
                LogTextboxContainer.WorkingInstance.Invoke(new MethodInvoker(delegate
                {
                    LogTextboxContainer.WorkingInstance.logTextBox.Text += ("\r\nLog #" + Output.LogCount + " " + Time.CurrentTime.DateTimeShort + ":\r\n");
                    LogTextboxContainer.WorkingInstance.logTextBox.Text += (logString + "\r\n");
					LogTextboxContainer.WorkingInstance.logTextBox.SelectionStart = LogTextboxContainer.WorkingInstance.logTextBox.TextLength;
					LogTextboxContainer.WorkingInstance.logTextBox.ScrollToCaret();
                }));
            }
            else 
			{
                LogTextboxContainer.WorkingInstance.logTextBox.Text += ("\r\nLog " + Output.LogCount + " " + Time.CurrentTime.DateTimeShort + ":\r\n");
                LogTextboxContainer.WorkingInstance.logTextBox.Text += (logString + "\r\n");
                LogTextboxContainer.WorkingInstance.logTextBox.SelectionStart = LogTextboxContainer.WorkingInstance.logTextBox.TextLength;
                LogTextboxContainer.WorkingInstance.logTextBox.ScrollToCaret();
			}
        }

		public static void ShowTestItem(Control customControl)
		{
			try
			{
				if (TestPanelContainer.WorkingInstance.InvokeRequired)
				{
					TestPanelContainer.WorkingInstance.Invoke(new MethodInvoker(delegate
					{
						TestPanelContainer.WorkingInstance.testPanel.Controls.Add(customControl);
						customControl.Top = TestPanelContainer.WorkingInstance.LastBottom + 5;
						TestPanelContainer.WorkingInstance.LastBottom = customControl.Bottom;
					}));
				}
				else
				{
					TestPanelContainer.WorkingInstance.testPanel.Controls.Add(customControl);
					customControl.Top = TestPanelContainer.WorkingInstance.LastBottom + 5;
					TestPanelContainer.WorkingInstance.LastBottom = customControl.Bottom;
				}
			}
			catch (Exception e)
			{
				Output.ShowLog("Error occured to ShowTestItem() => " + e.Message);
			}
		}

		public static void ShowImage(Image img)
		{
			try
			{
				Label imgLabel = new Label();
				imgLabel.Image = img;
				imgLabel.Size = img.Size;
				ShowTestItem(imgLabel);
			}
			catch (Exception e)
			{
				Output.ShowLog("Error occured to ShowImage() => " + e.Message);
			}
		}

        public static void Error(string message)
        {
            MessageBox.Show(message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
