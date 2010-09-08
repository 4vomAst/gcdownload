using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GcDownload
{
    public partial class ListCachesForm : Form
    {
        public List<MainForm.GeocacheGpx> geocacheList = new List<MainForm.GeocacheGpx>();
        public string CacheIdToSearch = "";
        public List<string> DeletedCacheIds = new List<string>();

        public ListCachesForm()
        {
            InitializeComponent();
        }

        private void ListCaches_Load(object sender, EventArgs e)
        {
            CacheIdToSearch = "";
            DeletedCacheIds.Clear();

            listViewCaches.Items.Clear();

            if (geocacheList.Count > 0)
            {
                foreach (MainForm.GeocacheGpx geocache in geocacheList)
                {
                    ListViewItem item = new ListViewItem(geocache.GcId);
                    //item.SubItems.Add(logEntry.Timestamp.ToLocalTime().ToShortDateString() + " " + logEntry.Timestamp.ToLocalTime().ToShortTimeString());
                    //item.SubItems.Add(logEntry.Type);
                    item.SubItems.Add(geocache.Name);
                    item.SubItems.Add(geocache.Author);
                    item.SubItems.Add(geocache.ShortDescription);

                    //item.Checked = false;

                    listViewCaches.Items.Add(item);
                }

                listViewCaches.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                listViewCaches.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                listViewCaches.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }

            buttonSearch.Enabled = false;
            buttonDeleteEntry.Enabled = false;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (listViewCaches.SelectedItems.Count > 0)
            {
                CacheIdToSearch = listViewCaches.SelectedItems[0].Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void buttonDeleteEntry_Click(object sender, EventArgs e)
        {
            if (listViewCaches.SelectedItems.Count == 1)
            {
                DeletedCacheIds.Add(listViewCaches.SelectedItems[0].Text);
                listViewCaches.Items.Remove(listViewCaches.SelectedItems[0]);
            }
        }

        private void listViewCaches_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonSearch.Enabled = (listViewCaches.SelectedItems.Count == 1);
            buttonDeleteEntry.Enabled = (listViewCaches.SelectedItems.Count == 1);
        }

        private void listViewCaches_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewCaches.SelectedItems.Count == 1)
            {
                CacheIdToSearch = listViewCaches.SelectedItems[0].Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
