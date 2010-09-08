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

            UpdateDriveList();
            SelectConfiguredDrive();

            textBoxArchivPath.Text = settings.ArchivePath;
        }

        public void UpdateDriveList()
        {
            string[] driveNames = settings.getDriveNames();

            comboBoxGarminDrive.ResetText();

            foreach (string name in driveNames)
            {
                comboBoxGarminDrive.Items.Add(name);
            }
        }

        public void SelectConfiguredDrive()
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
        }

        private void buttonDetect_Click(object sender, EventArgs e)
        {
            if (settings != null)
            {
                UpdateDriveList();

                if (settings.autoDetectGarmin())
                {
                    SelectConfiguredDrive();
                }
            }
        }

        private void buttonBrowseArchivDirectory_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.SelectedPath = settings.ArchivePath;
            folderBrowserDialog.Description = Properties.Resources.MessageArchiveBrowseFolder;

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
            if (!settings.isArchivePathValid())
            {
                MessageBox.Show(Properties.Resources.ErrorGarminNotFound, Properties.Resources.TitleSettings, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cancel = true;
            }
            if (!settings.isGarminDirectoryOk())
            {
                MessageBox.Show(Properties.Resources.ErrorArchiveDirectoryNotFound, Properties.Resources.TitleSettings, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cancel = true;
            }
            if (!cancel)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }

    }
}
