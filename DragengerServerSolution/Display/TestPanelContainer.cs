using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Display
{
	public partial class TestPanelContainer : UserControl
	{
		public int LastBottom
		{
			set;
			get;
		}
		public TestPanelContainer()
		{
			InitializeComponent();
		}

		public static TestPanelContainer WorkingInstance { get; set; }
	}
}
