namespace Main
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.titleLabel = new System.Windows.Forms.Label();
            this.urlLabel = new System.Windows.Forms.Label();
            this.stateSwitchButton = new System.Windows.Forms.Button();
            this.ServerUrlCombobox = new System.Windows.Forms.ComboBox();
            this.stateIndicatorLabel = new System.Windows.Forms.Label();
            this.serverDisplayTabControl = new System.Windows.Forms.TabControl();
            this.serverLogTab = new System.Windows.Forms.TabPage();
            this.testTabControl = new System.Windows.Forms.TabPage();
            this.sqlQueryTab = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.queryOptionButton2 = new System.Windows.Forms.RadioButton();
            this.queryOptionButton1 = new System.Windows.Forms.RadioButton();
            this.executeButton1 = new System.Windows.Forms.Button();
            this.resultLabel = new System.Windows.Forms.Label();
            this.resultBox = new System.Windows.Forms.TextBox();
            this.executeButton2 = new System.Windows.Forms.Button();
            this.nonSqlQueryBox = new System.Windows.Forms.TextBox();
            this.resultGridView = new System.Windows.Forms.DataGridView();
            this.readSelectLabel = new System.Windows.Forms.Label();
            this.selectQueryTextbox = new System.Windows.Forms.TextBox();
            this.restartButton = new System.Windows.Forms.Button();
            this.clearLogButton = new System.Windows.Forms.Button();
            this.clearTestButton = new System.Windows.Forms.Button();
            this.serverDisplayTabControl.SuspendLayout();
            this.sqlQueryTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.titleLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.SystemColors.InfoText;
            this.titleLabel.Location = new System.Drawing.Point(199, 21);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(435, 57);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Server | Dragenger";
            // 
            // urlLabel
            // 
            this.urlLabel.AutoSize = true;
            this.urlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.urlLabel.Location = new System.Drawing.Point(241, 121);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(46, 20);
            this.urlLabel.TabIndex = 11;
            this.urlLabel.Text = "URL:";
            // 
            // stateSwitchButton
            // 
            this.stateSwitchButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.stateSwitchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stateSwitchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stateSwitchButton.Location = new System.Drawing.Point(633, 118);
            this.stateSwitchButton.Name = "stateSwitchButton";
            this.stateSwitchButton.Size = new System.Drawing.Size(104, 26);
            this.stateSwitchButton.TabIndex = 10;
            this.stateSwitchButton.Text = "Run Server";
            this.stateSwitchButton.UseVisualStyleBackColor = false;
            this.stateSwitchButton.Click += new System.EventHandler(this.stateSwitchButton_Click);
            // 
            // ServerUrlCombobox
            // 
            this.ServerUrlCombobox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerUrlCombobox.FormattingEnabled = true;
            this.ServerUrlCombobox.Items.AddRange(new object[] {
            "http://",
            "http://localhost:8080/",
            "http://*:8080/",
            "http://localhost:53855/",
            "http://*:53855/",
            "http://127.0.0.1:8080/",
            "http://127.0.0.1:53855/"});
            this.ServerUrlCombobox.Location = new System.Drawing.Point(287, 118);
            this.ServerUrlCombobox.Name = "ServerUrlCombobox";
            this.ServerUrlCombobox.Size = new System.Drawing.Size(346, 26);
            this.ServerUrlCombobox.TabIndex = 15;
            // 
            // stateIndicatorLabel
            // 
            this.stateIndicatorLabel.AutoSize = true;
            this.stateIndicatorLabel.Font = new System.Drawing.Font("Calibri", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stateIndicatorLabel.ForeColor = System.Drawing.Color.Green;
            this.stateIndicatorLabel.Location = new System.Drawing.Point(16, 115);
            this.stateIndicatorLabel.Name = "stateIndicatorLabel";
            this.stateIndicatorLabel.Size = new System.Drawing.Size(122, 26);
            this.stateIndicatorLabel.TabIndex = 16;
            this.stateIndicatorLabel.Text = "⦿ Running...";
            this.stateIndicatorLabel.Visible = false;
            // 
            // serverDisplayTabControl
            // 
            this.serverDisplayTabControl.Controls.Add(this.serverLogTab);
            this.serverDisplayTabControl.Controls.Add(this.testTabControl);
            this.serverDisplayTabControl.Controls.Add(this.sqlQueryTab);
            this.serverDisplayTabControl.Font = new System.Drawing.Font("Corbel", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverDisplayTabControl.Location = new System.Drawing.Point(17, 144);
            this.serverDisplayTabControl.Name = "serverDisplayTabControl";
            this.serverDisplayTabControl.SelectedIndex = 0;
            this.serverDisplayTabControl.Size = new System.Drawing.Size(726, 351);
            this.serverDisplayTabControl.TabIndex = 19;
            // 
            // serverLogTab
            // 
            this.serverLogTab.Location = new System.Drawing.Point(4, 27);
            this.serverLogTab.Name = "serverLogTab";
            this.serverLogTab.Padding = new System.Windows.Forms.Padding(3);
            this.serverLogTab.Size = new System.Drawing.Size(718, 320);
            this.serverLogTab.TabIndex = 0;
            this.serverLogTab.Text = "Server Logs";
            this.serverLogTab.UseVisualStyleBackColor = true;
            // 
            // testTabControl
            // 
            this.testTabControl.Location = new System.Drawing.Point(4, 27);
            this.testTabControl.Name = "testTabControl";
            this.testTabControl.Padding = new System.Windows.Forms.Padding(3);
            this.testTabControl.Size = new System.Drawing.Size(718, 320);
            this.testTabControl.TabIndex = 1;
            this.testTabControl.Text = "Test Tab";
            this.testTabControl.UseVisualStyleBackColor = true;
            // 
            // sqlQueryTab
            // 
            this.sqlQueryTab.Controls.Add(this.label1);
            this.sqlQueryTab.Controls.Add(this.queryOptionButton2);
            this.sqlQueryTab.Controls.Add(this.queryOptionButton1);
            this.sqlQueryTab.Controls.Add(this.executeButton1);
            this.sqlQueryTab.Controls.Add(this.resultLabel);
            this.sqlQueryTab.Controls.Add(this.resultBox);
            this.sqlQueryTab.Controls.Add(this.executeButton2);
            this.sqlQueryTab.Controls.Add(this.nonSqlQueryBox);
            this.sqlQueryTab.Controls.Add(this.resultGridView);
            this.sqlQueryTab.Controls.Add(this.readSelectLabel);
            this.sqlQueryTab.Controls.Add(this.selectQueryTextbox);
            this.sqlQueryTab.Location = new System.Drawing.Point(4, 27);
            this.sqlQueryTab.Name = "sqlQueryTab";
            this.sqlQueryTab.Padding = new System.Windows.Forms.Padding(3);
            this.sqlQueryTab.Size = new System.Drawing.Size(718, 320);
            this.sqlQueryTab.TabIndex = 2;
            this.sqlQueryTab.Text = "Execute SQL";
            this.sqlQueryTab.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 244);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 18);
            this.label1.TabIndex = 12;
            this.label1.Text = "Execute : ";
            // 
            // queryOptionButton2
            // 
            this.queryOptionButton2.AutoSize = true;
            this.queryOptionButton2.Location = new System.Drawing.Point(194, 242);
            this.queryOptionButton2.Name = "queryOptionButton2";
            this.queryOptionButton2.Size = new System.Drawing.Size(98, 22);
            this.queryOptionButton2.TabIndex = 11;
            this.queryOptionButton2.Text = "SQL Scalar";
            this.queryOptionButton2.UseVisualStyleBackColor = true;
            // 
            // queryOptionButton1
            // 
            this.queryOptionButton1.AutoSize = true;
            this.queryOptionButton1.Checked = true;
            this.queryOptionButton1.Location = new System.Drawing.Point(93, 242);
            this.queryOptionButton1.Name = "queryOptionButton1";
            this.queryOptionButton1.Size = new System.Drawing.Size(95, 22);
            this.queryOptionButton1.TabIndex = 10;
            this.queryOptionButton1.TabStop = true;
            this.queryOptionButton1.Text = "Non-query";
            this.queryOptionButton1.UseVisualStyleBackColor = true;
            // 
            // executeButton1
            // 
            this.executeButton1.Location = new System.Drawing.Point(637, 17);
            this.executeButton1.Name = "executeButton1";
            this.executeButton1.Size = new System.Drawing.Size(75, 33);
            this.executeButton1.TabIndex = 9;
            this.executeButton1.Text = "Execute";
            this.executeButton1.UseVisualStyleBackColor = true;
            this.executeButton1.Click += new System.EventHandler(this.executeButton1_Click);
            // 
            // resultLabel
            // 
            this.resultLabel.AutoSize = true;
            this.resultLabel.Location = new System.Drawing.Point(6, 299);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(49, 18);
            this.resultLabel.TabIndex = 8;
            this.resultLabel.Text = "Result";
            // 
            // resultBox
            // 
            this.resultBox.BackColor = System.Drawing.SystemColors.MenuBar;
            this.resultBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.resultBox.Location = new System.Drawing.Point(59, 295);
            this.resultBox.Name = "resultBox";
            this.resultBox.ReadOnly = true;
            this.resultBox.Size = new System.Drawing.Size(572, 22);
            this.resultBox.TabIndex = 7;
            // 
            // executeButton2
            // 
            this.executeButton2.Location = new System.Drawing.Point(637, 265);
            this.executeButton2.Name = "executeButton2";
            this.executeButton2.Size = new System.Drawing.Size(75, 33);
            this.executeButton2.TabIndex = 6;
            this.executeButton2.Text = "Execute";
            this.executeButton2.UseVisualStyleBackColor = true;
            this.executeButton2.Click += new System.EventHandler(this.executeButton2_Click);
            // 
            // nonSqlQueryBox
            // 
            this.nonSqlQueryBox.Font = new System.Drawing.Font("Consolas", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nonSqlQueryBox.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.nonSqlQueryBox.Location = new System.Drawing.Point(7, 266);
            this.nonSqlQueryBox.Name = "nonSqlQueryBox";
            this.nonSqlQueryBox.Size = new System.Drawing.Size(624, 25);
            this.nonSqlQueryBox.TabIndex = 5;
            // 
            // resultGridView
            // 
            this.resultGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultGridView.Location = new System.Drawing.Point(7, 53);
            this.resultGridView.Name = "resultGridView";
            this.resultGridView.Size = new System.Drawing.Size(705, 183);
            this.resultGridView.TabIndex = 4;
            // 
            // readSelectLabel
            // 
            this.readSelectLabel.AutoSize = true;
            this.readSelectLabel.Location = new System.Drawing.Point(6, 5);
            this.readSelectLabel.Name = "readSelectLabel";
            this.readSelectLabel.Size = new System.Drawing.Size(182, 18);
            this.readSelectLabel.TabIndex = 1;
            this.readSelectLabel.Text = "Execute Select Statement";
            // 
            // selectQueryTextbox
            // 
            this.selectQueryTextbox.Font = new System.Drawing.Font("Consolas", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectQueryTextbox.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.selectQueryTextbox.Location = new System.Drawing.Point(7, 24);
            this.selectQueryTextbox.Name = "selectQueryTextbox";
            this.selectQueryTextbox.Size = new System.Drawing.Size(624, 25);
            this.selectQueryTextbox.TabIndex = 0;
            // 
            // restartButton
            // 
            this.restartButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.restartButton.Location = new System.Drawing.Point(570, 498);
            this.restartButton.Name = "restartButton";
            this.restartButton.Size = new System.Drawing.Size(170, 28);
            this.restartButton.TabIndex = 13;
            this.restartButton.Text = "Restart Application";
            this.restartButton.UseVisualStyleBackColor = true;
            this.restartButton.Click += new System.EventHandler(this.restartButton_Click);
            // 
            // clearLogButton
            // 
            this.clearLogButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearLogButton.Location = new System.Drawing.Point(442, 498);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(122, 28);
            this.clearLogButton.TabIndex = 8;
            this.clearLogButton.Text = "Clear Logs";
            this.clearLogButton.UseVisualStyleBackColor = true;
            this.clearLogButton.Click += new System.EventHandler(this.clearLogButton_Click);
            // 
            // clearTestButton
            // 
            this.clearTestButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearTestButton.Location = new System.Drawing.Point(442, 498);
            this.clearTestButton.Name = "clearTestButton";
            this.clearTestButton.Size = new System.Drawing.Size(122, 28);
            this.clearTestButton.TabIndex = 20;
            this.clearTestButton.Text = "Clear Test";
            this.clearTestButton.UseVisualStyleBackColor = true;
            this.clearTestButton.Click += new System.EventHandler(this.clearTestButton_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.stateSwitchButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(758, 535);
            this.Controls.Add(this.clearTestButton);
            this.Controls.Add(this.serverDisplayTabControl);
            this.Controls.Add(this.stateIndicatorLabel);
            this.Controls.Add(this.ServerUrlCombobox);
            this.Controls.Add(this.restartButton);
            this.Controls.Add(this.urlLabel);
            this.Controls.Add(this.stateSwitchButton);
            this.Controls.Add(this.clearLogButton);
            this.Controls.Add(this.titleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Dragenger Server";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.serverDisplayTabControl.ResumeLayout(false);
            this.sqlQueryTab.ResumeLayout(false);
            this.sqlQueryTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label urlLabel;
		private System.Windows.Forms.Button stateSwitchButton;
        private System.Windows.Forms.ComboBox ServerUrlCombobox;
		private System.Windows.Forms.Label stateIndicatorLabel;
		private System.Windows.Forms.TabControl serverDisplayTabControl;
		private System.Windows.Forms.TabPage serverLogTab;
		private System.Windows.Forms.TabPage testTabControl;
		private System.Windows.Forms.Button restartButton;
		private System.Windows.Forms.Button clearLogButton;
		private System.Windows.Forms.Button clearTestButton;
        private System.Windows.Forms.TabPage sqlQueryTab;
        private System.Windows.Forms.Button executeButton1;
        private System.Windows.Forms.Label resultLabel;
        private System.Windows.Forms.TextBox resultBox;
        private System.Windows.Forms.Button executeButton2;
        private System.Windows.Forms.TextBox nonSqlQueryBox;
        private System.Windows.Forms.DataGridView resultGridView;
        private System.Windows.Forms.Label readSelectLabel;
        private System.Windows.Forms.TextBox selectQueryTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton queryOptionButton2;
        private System.Windows.Forms.RadioButton queryOptionButton1;

    }
}

