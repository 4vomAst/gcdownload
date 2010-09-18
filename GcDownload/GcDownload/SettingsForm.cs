/*
Copyright (c) 2010 Wolfgang Bruessler

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
using System.IO;

namespace GcDownload
{
    public partial class SettingsForm : Form
    {
        private CSettings settings;

        public SettingsForm(ref CSettings s)
        {
            settings = s;
            InitializeComponent();

            UpdateDriveLists();
            SelectConfiguredDrives();
            checkBoxStoreCachesOnSdCard.Checked = settings.StoreCachesOnSdCard;
            textBoxArchivPath.Text = settings.ArchivePath;
        }

        public void UpdateDriveLists()
        {
            string[] driveNames = settings.getDriveNames();

            comboBoxGarminDrive.ResetText();
            comboBoxSdCardDrive.ResetText();

            foreach (string name in driveNames)
            {
                comboBoxGarminDrive.Items.Add(name);
                comboBoxSdCardDrive.Items.Add(name);
            }
        }

        public void SelectConfiguredDrives()
        {
            int i;
            i = comboBoxGarminDrive.FindString(settings.GarminRootDir);
            if (i != -1)
            {
                comboBoxGarminDrive.SelectedIndex = i;
            }
            else
            {
                comboBoxGarminDrive.SelectedIndex = 0;
            }

            i = comboBoxSdCardDrive.FindString(settings.SdCardRootDir);
            if (i != -1)
            {
                comboBoxSdCardDrive.SelectedIndex = i;
            }
            else
            {
                comboBoxSdCardDrive.SelectedIndex = 0;
            }
        }

        private void buttonDetect_Click(object sender, EventArgs e)
        {
            if (settings != null)
            {
                UpdateDriveLists();

                if (settings.autoDetectGarmin())
                {
                    SelectConfiguredDrives();
                }
            }
        }

        private void buttonBrowseArchivDirectory_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.SelectedPath = settings.ArchivePath;
            folderBrowserDialog.Description = GcDownload.Strings.MessageArchiveBrowseFolder;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                settings.ArchivePath = folderBrowserDialog.SelectedPath;
                textBoxArchivPath.Text = settings.ArchivePath;
            }
        }

        string GetDriveName(ref ComboBox comboBox)
        {
            string[] parts = comboBox.Text.Split(' ');
            if (parts.Length > 0) return parts[0];
            return "";
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            bool cancel = false;
            settings.GarminRootDir = GetDriveName(ref comboBoxGarminDrive);
            settings.SdCardRootDir = GetDriveName(ref comboBoxSdCardDrive);
            settings.StoreCachesOnSdCard = checkBoxStoreCachesOnSdCard.Checked;
            if (!settings.isGarminConnected())
            {
                MessageBox.Show(GcDownload.Strings.ErrorGarminNotFound, GcDownload.Strings.TitleSettings, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cancel = true;
            }
            if (settings.StoreCachesOnSdCard && (!settings.isSdCardConnected()))
            {
                MessageBox.Show(GcDownload.Strings.ErrorSdCardNotFound, GcDownload.Strings.TitleSettings, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cancel = true;
            }
            if (!settings.isArchivePathValid())
            {
                MessageBox.Show(GcDownload.Strings.ErrorArchiveDirectoryNotFound, GcDownload.Strings.TitleSettings, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cancel = true;
            }
            if (!cancel)
            {
                settings.saveSettings();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }

    }
}
