using Display;
using Repositories;
using ServerConnections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public TextBox LogTextBox
        {
            set;
            get;
        }

		public Panel TestPanel
		{
			set;
			get;
		}

        public bool ServerRunning
        {
            set;
            get;
        }

        private void MainForm_Load(object sender, EventArgs ergs)
        {
            LogTextboxContainer.WorkingInstance = new LogTextboxContainer();
            this.LogTextBox = LogTextboxContainer.WorkingInstance.logTextBox;
            this.serverDisplayTabControl.TabPages[0].Controls.Add(LogTextboxContainer.WorkingInstance);

			TestPanelContainer.WorkingInstance = new TestPanelContainer();
			this.TestPanel = TestPanelContainer.WorkingInstance.testPanel;
			this.serverDisplayTabControl.TabPages[1].Controls.Add(TestPanelContainer.WorkingInstance);

			this.clearTestButton.Visible = false;			//by default, the first tab is selected. so clearTestButton is invisible by default.
			this.serverDisplayTabControl.Selecting += (s,e) => 
			{
				if (this.serverDisplayTabControl.SelectedIndex == 0) 
				{
					this.clearTestButton.Visible = false;
					this.clearLogButton.Visible = true;
				}
				else if (this.serverDisplayTabControl.SelectedIndex == 1) 
				{
					this.clearLogButton.Visible = false;
					this.clearTestButton.Visible = true;
				}
			};
        }

        private void stateSwitchButton_Click(object sender, EventArgs e)
        {
            bool success = false;
            if(this.ServerRunning)
            {
                success = stopServer();
            }
            else
            {
                success = runServer();
            }
            if (success) this.ServerRunning = !this.ServerRunning;
        }

        private bool stopServer()
        {
            bool success = ServerManager.TryStopServer();
            if (success)
            {
                this.ServerUrlCombobox.Enabled = true;
                this.stateSwitchButton.BackColor = Color.FromArgb(255, 255, 192);
                this.stateSwitchButton.Text = "Run Server";
                this.stateIndicatorLabel.Visible = false;
            }
            return success;
        }

        private bool runServer()
        {
            string url = ServerUrlCombobox.Text;
            bool success = ServerManager.StartServer(url);
            if (success)
            {
                this.ServerUrlCombobox.Enabled = false;
                this.stateSwitchButton.BackColor = Color.FromArgb(153, 255, 153);
                this.stateSwitchButton.Text = "Stop Server";
                this.stateIndicatorLabel.Visible = true;
                ClientManager.InitializeStaticDictionaries();
            }
            return success;
        }

        private void restartButton_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void clearLogButton_Click(object sender, EventArgs e)
        {
            this.LogTextBox.Text = "";
        }

		private void clearTestButton_Click(object sender, EventArgs e)
		{
			this.TestPanel.Controls.Clear();
			TestPanelContainer.WorkingInstance.LastBottom = 0;
			TestPanelContainer.WorkingInstance.VerticalScroll.Value = TestPanelContainer.WorkingInstance.VerticalScroll.Maximum;
		}

        private void executeButton1_Click(object sender, EventArgs e)
        {
            string query = this.selectQueryTextbox.Text;
            if (query.Length == 0) return;
            SqlDataReader data = DatabaseAccess.Instance.ReadSqlData(query);
            if (data == null)
            {
                this.resultBox.Text = "Invalid Select Statement!";
                return;
            }
            DataTable dataTable = new DataTable();
            if (data.Read())
            {
                dataTable.Load(data);
            }
            this.resultGridView.DataSource = dataTable;
            this.resultGridView.Update();
        }

        private void executeButton2_Click(object sender, EventArgs e)
        {
            string query = this.nonSqlQueryBox.Text;
            if (query.Length == 0) return;
            string result = null;
            if (queryOptionButton1.Checked) result = DatabaseAccess.Instance.ExecuteSqlQueryAndGiveResultString(query);
            else if (queryOptionButton2.Checked) result = DatabaseAccess.Instance.ExecuteSqlScalarAndGiveResultString(query);
            else result = "No query option selected!";
            this.resultBox.Text = result;
        }
    }
}
