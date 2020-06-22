using EntityLibrary;
using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CorePanels
{
    public static class SyncAssets
    {
        internal static SortedList<long, LinkLabel> ShowedNuntiasLabelSortedList { private set; get; }
        internal static SortedList<long, NuntiasInfoPanel> NuntiasInfoPanelSortedList { private set; get; }
        internal static SortedList<long, NuntiasOptionsPanel> NuntiasOptionPanelSortedList { private set; get; }
        public static SortedList<long, Nuntias> NuntiasSortedList { private set; get; }
        static SyncAssets()
        {
            ShowedNuntiasLabelSortedList = new SortedList<long, LinkLabel>();
            NuntiasInfoPanelSortedList = new SortedList<long, NuntiasInfoPanel>();
            NuntiasOptionPanelSortedList = new SortedList<long, NuntiasOptionsPanel>();
            NuntiasSortedList = new SortedList<long, Nuntias>();
        }

        public static void ConvertLocalNuntiasToGlobal(Nuntias updatedNuntias, long localNuntiasId)
        {
            try { SyncAssets.NuntiasSortedList.Remove(localNuntiasId); }
            catch { return; }
            SyncAssets.NuntiasSortedList[updatedNuntias.Id] = updatedNuntias;

            try
            {
                SyncAssets.ShowedNuntiasLabelSortedList[updatedNuntias.Id] = SyncAssets.ShowedNuntiasLabelSortedList[localNuntiasId];
                SyncAssets.ShowedNuntiasLabelSortedList[localNuntiasId] = null;
                SyncAssets.ShowedNuntiasLabelSortedList.Remove(localNuntiasId);
            }
            catch { }

            try
            {
                NuntiasInfoPanel nuntiasInfoPanel = SyncAssets.NuntiasInfoPanelSortedList[updatedNuntias.Id] = SyncAssets.NuntiasInfoPanelSortedList[localNuntiasId];
                nuntiasInfoPanel.NuntiasId = updatedNuntias.Id;
                if (Universal.ParentForm.InvokeRequired)
                {
                    Universal.ParentForm.Invoke(new Action(() =>
                    {
                        nuntiasInfoPanel.UpdateInfoPanel();
                    }));
                }
                else
                {
                    nuntiasInfoPanel.UpdateInfoPanel();
                }
                SyncAssets.NuntiasInfoPanelSortedList[localNuntiasId] = null;
                SyncAssets.NuntiasInfoPanelSortedList.Remove(localNuntiasId);
            }
            catch { }

            try
            {
                NuntiasOptionsPanel nuntiasOptionPanel = SyncAssets.NuntiasOptionPanelSortedList[updatedNuntias.Id] = SyncAssets.NuntiasOptionPanelSortedList[localNuntiasId];
                nuntiasOptionPanel.NuntiasId = updatedNuntias.Id;
                if (Universal.ParentForm.InvokeRequired)
                {
                    Universal.ParentForm.Invoke(new Action(() =>
                    {
                        nuntiasOptionPanel.UpdateOptionPanel();
                    }));
                }
                else
                {
                    nuntiasOptionPanel.UpdateOptionPanel();
                }
                SyncAssets.NuntiasOptionPanelSortedList[localNuntiasId] = null;
                SyncAssets.NuntiasOptionPanelSortedList.Remove(localNuntiasId);
            }
            catch { }
        }

        public static void UpdateNuntias(Nuntias nuntias)
        {
            SyncAssets.NuntiasSortedList[nuntias.Id] = nuntias;

            if (Universal.ParentForm.InvokeRequired)
            {
                Universal.ParentForm.Invoke(new Action(() =>
                {
                    try
                    {
                        NuntiasLocalUpdateWorks(nuntias);
                    }
                    catch { }
                }));
            }
            else
            {
                try
                {
                    NuntiasLocalUpdateWorks(nuntias);
                }
                catch { }
            }
        }
        private static void NuntiasLocalUpdateWorks(Nuntias nuntias)
        {
            if (nuntias.ContentFileId == null || nuntias.ContentFileId.Length == 0 || nuntias.ContentFileId == "deleted")
            {
                SyncAssets.ShowedNuntiasLabelSortedList[nuntias.Id].Text = nuntias.Text;
                if (nuntias.ContentFileId == "deleted")
                {
                    NuntiasLocalDeletionWorks(nuntias.Id);
                    return;
                }
            }
            SyncAssets.NuntiasInfoPanelSortedList[nuntias.Id].UpdateInfoPanel();
            SyncAssets.NuntiasOptionPanelSortedList[nuntias.Id].UpdateOptionPanel();
        }
        private static void NuntiasLocalDeletionWorks(long nuntiasId)
        {
            SyncAssets.NuntiasInfoPanelSortedList[nuntiasId].Dispose();
            SyncAssets.NuntiasOptionPanelSortedList[nuntiasId].Dispose();

            SyncAssets.NuntiasInfoPanelSortedList[nuntiasId] = null;
            SyncAssets.NuntiasOptionPanelSortedList[nuntiasId] = null;

            LinkLabel showedNuntiasLinkLabel = SyncAssets.ShowedNuntiasLabelSortedList[nuntiasId];
            showedNuntiasLinkLabel.BackColor = System.Drawing.Color.Transparent;
            showedNuntiasLinkLabel.ForeColor = System.Drawing.Color.FromArgb(102, 51, 0);
            showedNuntiasLinkLabel.Font = CustomFonts.New(CustomFonts.SmallerSize, 'i');
            showedNuntiasLinkLabel.LinkArea = new LinkArea(0,0);
            if (showedNuntiasLinkLabel.Image != null)
            {
                showedNuntiasLinkLabel.Image.Dispose();
                showedNuntiasLinkLabel.Image = null;
            }
            showedNuntiasLinkLabel.BorderStyle = BorderStyle.FixedSingle;
            if (SyncAssets.NuntiasSortedList[nuntiasId].SenderId == Consumer.LoggedIn.Id)
                showedNuntiasLinkLabel.Left += showedNuntiasLinkLabel.Width - showedNuntiasLinkLabel.PreferredWidth;

            showedNuntiasLinkLabel.Size = showedNuntiasLinkLabel.PreferredSize;

            SyncAssets.ShowedNuntiasLabelSortedList[nuntiasId] = null;
            SyncAssets.NuntiasSortedList[nuntiasId] = null;
        }

        internal static void DeleteNuntiasAssets(long nuntiasId)
        {
            if (Universal.ParentForm.InvokeRequired)
            {
                Universal.ParentForm.Invoke(new Action(() =>
                {
                    DeleteNuntiasAssets(nuntiasId);
                    return;
                }));
            }
            else
            {
                try
                {
                    SyncAssets.ShowedNuntiasLabelSortedList[nuntiasId].Text = "the message is deleted from your end";
                    NuntiasLocalDeletionWorks(nuntiasId);
                }
                catch { }
            }
            SyncAssets.NuntiasSortedList.Remove(nuntiasId);
        }
    }
}
