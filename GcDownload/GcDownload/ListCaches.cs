/*
Copyright (c) 2010-2019 Wolfgang Wallhaeuser

https://github.com/4vomast/gcdownload

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GcDownload
{
    public partial class ListCachesForm : Form
    {
        public List<GeocacheGpx> geocacheList = new List<GeocacheGpx>();
        public string cacheIdToSearch = "";
        public List<string> deletedCacheIds = new List<string>();
        public List<string> updateCacheIds = new List<string>();

        public ListCachesForm()
        {
            InitializeComponent();
        }

        private void ListCaches_Load(object sender, EventArgs e)
        {
            cacheIdToSearch = "";
            deletedCacheIds.Clear();
            updateCacheIds.Clear();

            listViewCaches.Items.Clear();

            if (geocacheList.Count > 0)
            {
                foreach (GeocacheGpx geocache in geocacheList)
                {
                    ListViewItem item = new ListViewItem(geocache.GcId);
                    item.SubItems.Add(geocache.Name);
                    item.SubItems.Add(geocache.Author);
                    item.SubItems.Add(geocache.FileTimestamp.ToShortDateString());
                    item.SubItems.Add(geocache.ShortDescription);

                    listViewCaches.Items.Add(item);
                }

                listViewCaches.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                listViewCaches.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                listViewCaches.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                listViewCaches.Columns[4].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }

            buttonSearch.Enabled = false;
            buttonDeleteEntry.Enabled = false;
            buttonUpdateAll.Enabled = listViewCaches.Items.Count > 0;
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            if (listViewCaches.SelectedItems.Count == 0)
            {
                return;
            }

            cacheIdToSearch = listViewCaches.SelectedItems[0].Text;
            DialogResult = DialogResult.OK;
        }

        private void ButtonDeleteEntry_Click(object sender, EventArgs e)
        {
            if (listViewCaches.SelectedItems.Count == 0)
            {
                return;
            }

            foreach (ListViewItem item in listViewCaches.SelectedItems)
            {
                deletedCacheIds.Add(item.Text);
                listViewCaches.Items.Remove(item);
            }
        }

        private void ListViewCaches_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonSearch.Enabled = (listViewCaches.SelectedItems.Count == 1);
            buttonDeleteEntry.Enabled = (listViewCaches.SelectedItems.Count > 0);
            buttonUpdateAll.Enabled = listViewCaches.Items.Count > 0;
        }

        private void ListViewCaches_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewCaches.SelectedItems.Count != 1)
            {
                return;
            }

            cacheIdToSearch = listViewCaches.SelectedItems[0].Text;
            DialogResult = DialogResult.OK;
        }

        private void ButtonUpdateAll_Click(object sender, EventArgs e)
        {
            updateCacheIds.Clear();
            foreach (ListViewItem item in listViewCaches.SelectedItems)
            {
                updateCacheIds.Add(item.Text);
            };

            DialogResult = DialogResult.OK;
        }
    }
}
