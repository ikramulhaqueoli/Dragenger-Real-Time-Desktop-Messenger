using EntityLibrary;
using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CorePanels
{
    class EmailListPanel : Panel
    {
        private Panel parent;
        private List<Panel> singleEmailPanelList;
        private List<Object> emailList;

        public EmailListPanel(Panel parent)
        {
            this.parent = parent;
            this.BackColor = Color.FromArgb(176, 176, 176);
            this.emailList = this.FetchEamilList();
            this.singleEmailPanelList = new List<Panel>();

            Label underDevelopment = new Label();
            underDevelopment.Text = "Email feature is under development";
            underDevelopment.Font = CustomFonts.SmallerBold;
            underDevelopment.Size = underDevelopment.PreferredSize;

            this.Size = this.PreferredSize;
            this.Controls.Add(underDevelopment);
        }

        private List<Object> FetchEamilList()
        {
            return null;
        }
    }
}
