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
    public partial class FieldLogForm : Form
    {
        public List<FieldLogEntry> FieldLog = new List<FieldLogEntry>();
        public string CacheIdToSearch = "";
        static HashSet<string> VisitedCacheIds = new HashSet<string>();
        public HashSet<string> FoundCacheIds = new HashSet<string>();

        public FieldLogForm()
        {
            InitializeComponent();
        }

        private void FieldLogForm_Load(object sender, EventArgs e)
        {
            CacheIdToSearch = "";
            listViewFieldLog.Items.Clear();

            if (FieldLog.Count > 0)
            {
                foreach (FieldLogEntry logEntry in FieldLog)
                {
                    ListViewItem item = new ListViewItem(logEntry.CacheId);
                    item.SubItems.Add(logEntry.Timestamp.ToLocalTime().ToShortDateString() + " " + logEntry.Timestamp.ToLocalTime().ToShortTimeString());
                    item.SubItems.Add(logEntry.Type);
                    item.SubItems.Add(logEntry.Text);

                    item.Checked = VisitedCacheIds.Contains(logEntry.CacheId);

                    listViewFieldLog.Items.Add(item);
                }

                listViewFieldLog.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }

            UpdateButtonStates();
        }

        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            if (listViewFieldLog.SelectedItems.Count == 0)
            {
                return;
            }

            CacheIdToSearch = listViewFieldLog.SelectedItems[0].Text;
            VisitedCacheIds.Add(CacheIdToSearch);
            DialogResult = DialogResult.OK;
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void ListViewFieldLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void ListViewFieldLog_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewFieldLog.SelectedItems.Count != 1)
            {
                return;
            }

            CacheIdToSearch = listViewFieldLog.SelectedItems[0].Text;
            VisitedCacheIds.Add(CacheIdToSearch);
            DialogResult = DialogResult.OK;
        }

        private void ButtonDeleteLog_Click(object sender, EventArgs e)
        {
            foreach (FieldLogEntry logEntry in FieldLog)
            {
                if (logEntry.Type.ToLower() == "found it")
                {
                    FoundCacheIds.Add(logEntry.CacheId);
                }
            }

            FieldLog.Clear();
            DialogResult = DialogResult.OK;
        }

        private void ButtonDeleteEntry_Click(object sender, EventArgs e)
        {
            var removedSomething = false;

            do
            {
                removedSomething = false;

                foreach (ListViewItem item in listViewFieldLog.Items)
                {
                    if (!item.Checked)
                    {
                        continue;
                    }

                    listViewFieldLog.Items.Remove(item);

                    foreach (FieldLogEntry logEntry in FieldLog)
                    {
                        if (logEntry.CacheId != item.Text)
                        {
                            continue;
                        }

                        if (logEntry.Type.ToLower() == "found it")
                        {
                            FoundCacheIds.Add(logEntry.CacheId);
                        }

                        FieldLog.Remove(logEntry);
                        break;
                    }

                    removedSomething = true;
                    break;
                }
            } while (removedSomething);

            UpdateButtonStates();
        }

        private void ListViewFieldLog_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked)
            {
                VisitedCacheIds.Add(e.Item.Text);
            }
            else
            {
                VisitedCacheIds.Remove(e.Item.Text);
            }

            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            buttonDeleteLog.Enabled = listViewFieldLog.Items.Count > 0;
            buttonDeleteEntry.Enabled = false;

            foreach (ListViewItem item in listViewFieldLog.Items)
            {
                if (item.Checked)
                {
                    buttonDeleteEntry.Enabled = true;
                    break;
                }
            }

            buttonSearch.Enabled = (listViewFieldLog.SelectedItems.Count == 1);
        }
    }
}
