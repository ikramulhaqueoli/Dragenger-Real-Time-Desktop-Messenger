namespace Display
{
	partial class TestPanelContainer
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.testPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // testPanel
            // 
            this.testPanel.BackColor = System.Drawing.Color.DarkGray;
            this.testPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testPanel.Location = new System.Drawing.Point(0, 0);
            this.testPanel.Name = "testPanel";
            this.testPanel.Size = new System.Drawing.Size(541, 235);
            this.testPanel.TabIndex = 0;
            // 
            // TestPanelContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.testPanel);
            this.Name = "TestPanelContainer";
            this.Size = new System.Drawing.Size(541, 235);
            this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.Panel testPanel;
	}
}
