/*
Copyright (c) 2010-2012 Wolfgang Wallhaeuser

http://code.google.com/p/gcdownload/

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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GcDownload
{
    public partial class FieldLogForm : Form
    {
        public List<MainForm.FieldLogEntry> FieldLog = new List<MainForm.FieldLogEntry>();
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
                foreach (MainForm.FieldLogEntry logEntry in FieldLog)
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

            updateButtonStates();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (listViewFieldLog.SelectedItems.Count > 0)
            {
                CacheIdToSearch = listViewFieldLog.SelectedItems[0].Text;
                VisitedCacheIds.Add(CacheIdToSearch);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void listViewFieldLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateButtonStates();
        }

        private void listViewFieldLog_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewFieldLog.SelectedItems.Count == 1)
            {
                CacheIdToSearch = listViewFieldLog.SelectedItems[0].Text;
                VisitedCacheIds.Add(CacheIdToSearch);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void buttonDeleteLog_Click(object sender, EventArgs e)
        {
            foreach (MainForm.FieldLogEntry logEntry in FieldLog)
            {
                if (logEntry.Type.ToLower() == "found it")
                {
                    FoundCacheIds.Add(logEntry.CacheId);
                }
            }

            FieldLog.Clear();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonDeleteEntry_Click(object sender, EventArgs e)
        {
            bool removedSomething = false;

            do
            {
                removedSomething = false;

                foreach (ListViewItem item in listViewFieldLog.Items)
                {
                    if (item.Checked)
                    {
                        listViewFieldLog.Items.Remove(item);

                        foreach (MainForm.FieldLogEntry logEntry in FieldLog)
                        {
                            if (logEntry.CacheId == item.Text)
                            {
                                if (logEntry.Type.ToLower() == "found it")
                                {
                                    FoundCacheIds.Add(logEntry.CacheId);
                                }
                                FieldLog.Remove(logEntry);
                                break;
                            }
                        }

                        removedSomething = true;
                        break;
                    }
                }
            } while (removedSomething);

            updateButtonStates();
        }

        private void listViewFieldLog_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked)
            {
                VisitedCacheIds.Add(e.Item.Text);
            }
            else
            {
                VisitedCacheIds.Remove(e.Item.Text);
            }
            updateButtonStates();
        }

        private void updateButtonStates()
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
