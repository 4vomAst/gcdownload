/*
Copyright (c) 2010-2019 Wolfgang Wallhaeuser

http://github.com/4vomAst/gcdownload

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Sofdtware.

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
using System.Text;
using System.Windows.Forms;
using System.IO;
using NLog;

namespace GcDownload
{
    public partial class MainForm : Form
    {
        private static Logger m_logger = LogManager.GetCurrentClassLogger();
        private CSettings m_settings = new CSettings();
        private List<string> m_chacheIdsToBeDownloaded = new List<string>();

        enum Provider
        {
            ProviderGeocachingCom,
            ProviderOpencachingDe,
            ProviderUnknown
        }

        public MainForm()
        {
            m_logger.Info(GetType().Assembly.GetName().Version.ToString(3));
            m_logger.Info(System.Environment.OSVersion.VersionString);
            m_logger.Info(System.Environment.Version.ToString());

            InitializeComponent();

            webBrowserPreview.ScriptErrorsSuppressed = true;

            comboBoxWebsite.SelectedIndex = 1;

            m_settings.ReadSettings();
            m_settings.AutoDetectGarmin();
        }

        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxGeocacheId.Text)) return;

            Search(textBoxGeocacheId.Text);
        }

        private void Search(string geocacheId)
        {
            var url = GetCacheUrlFromId(geocacheId);

            m_logger.Debug($"ID: {geocacheId}, Url: {url}");
            webBrowserPreview.Navigate(url);
        }

        private string GetCacheUrlFromId(string geocacheId)
        {
            if (geocacheId.ToLower().StartsWith("gc"))
            {
                return "http://www.geocaching.com/seek/cache_details.aspx?wp=" + geocacheId;
            }
            else if (geocacheId.ToLower().StartsWith("oc"))
            {
                return "http://www.opencaching.de/viewcache.php?wp=" + geocacheId;
            }

            return string.Empty;
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            if (!EnsureGarminAvailable()) return;

            Download(true);
        }

        private void Download(bool promptForFilename)
        {
            m_logger.Debug($"({promptForFilename})");

            if (!EnsureGarminAvailable()) return;

            switch (GetProviderFromUrl(webBrowserPreview.Url.ToString()))
            {
                case Provider.ProviderGeocachingCom:
                    DownloadFromGeocaching(promptForFilename);
                    break;

                case Provider.ProviderOpencachingDe:
                    DownloadFromOpenCaching(promptForFilename);
                    break;
            }
        }

        private void DownloadFromGeocaching(bool promptForFilename)
        {
            var geocacheGpx = GeocachingDownload.ImportFromGeocaching(webBrowserPreview.Document);

            if (!geocacheGpx.IsValid)
            {
                string message = String.Format(GcDownload.Strings.ErrorNoGeocachePageSelected);
                MessageBox.Show(message, GcDownload.Strings.TitleDownload, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            var fullGpxFilePath = GetFullGpxFilePath(promptForFilename, geocacheGpx.GcId);

            if (string.IsNullOrEmpty(fullGpxFilePath)) return;

            try
            {
                m_logger.Debug("Write to file: " + fullGpxFilePath);
                geocacheGpx.WriteGpxFile(fullGpxFilePath);
            }
            catch (Exception ex)
            {
                m_logger.Error(ex, "Writing to file failed");
            }
        }

        private void DownloadFromOpenCaching(bool promptForFilename)
        {
            var title = webBrowserPreview.Document.Title;
            if (!title.StartsWith("OC")) return;

            int posBlank = title.IndexOf(" ");
            if (posBlank == -1) return;

            string ocCacheId = title.Substring(0, posBlank);
            string filename = GetFullGpxFilePath(promptForFilename, ocCacheId);

            DownloadViaOkapi(ocCacheId, filename);
        }

        public void DownloadViaOkapi(string cacheId, string filename)
        {
            //www.opencaching.de/okapi/services/caches/formatters/gpx?cache_codes=OC4FB6&ns_ground=true&attrs=gc:attrs|gc_ocde:attrs|desc:text&latest_logs=true&consumer_key=wgw9HbSeSXgH7xuWhM2P
            //string url = "http://www.opencaching.de/okapi/services/caches/formatters/gpx?ns_ground=true&latest_logs=true&consumer_key=wgw9HbSeSXgH7xuWhM2P";
            string url = "http://www.opencaching.de/okapi/services/caches/formatters/gpx?ns_ground=true&attrs=gc:attrs|desc:text&latest_logs=true&consumer_key=wgw9HbSeSXgH7xuWhM2P";
            url += "&cache_codes=";
            url += cacheId;

            using (var webClient = new System.Net.WebClient())
            {
                webClient.DownloadFile(url, filename);
            }
        }

        string GetFullGpxFilePath(bool promptForFilename, string gcid)
        {
            string fullGpxFilePath = string.Empty;

            if (promptForFilename)
            {
                var fileDialog = new SaveFileDialog
                {
                    InitialDirectory = m_settings.GpxPath,
                    AddExtension = true,
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    DefaultExt = "gpx",
                    FileName = gcid + ".gpx",
                    OverwritePrompt = true,
                    ValidateNames = true,
                    Filter = GcDownload.Strings.FilterGpxFiles,
                    FilterIndex = 1
                };

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    fullGpxFilePath = fileDialog.FileName;
                }
            }
            else
            {
                fullGpxFilePath = System.IO.Path.Combine(m_settings.GpxPath, gcid + ".gpx");
            }

            return fullGpxFilePath;
        }

        private void NavigateHome()
        {
            var url = comboBoxWebsite.SelectedItem.ToString();

            if (string.IsNullOrEmpty(url)) return;

            m_logger.Debug($"URL: {url}");
            webBrowserPreview.Navigate(url);
        }

        private void WebBrowserPreview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs eventArgs)
        {
            TriggerAutoDownload(eventArgs.Url.ToString());
        }

        private void ButtonHome_Click(object sender, EventArgs e)
        {
            NavigateHome();
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            webBrowserPreview.GoBack();
        }

        private Provider GetProviderFromUrl(string url)
        {
            if (url.ToLower().Contains("opencaching"))
            {
                return Provider.ProviderOpencachingDe;
            }
            else if (url.ToLower().Contains("geocaching.com"))
            {
                return Provider.ProviderGeocachingCom;
            }

            return Provider.ProviderUnknown;
        }

        private void ComboBoxWebsite_SelectedIndexChanged(object sender, EventArgs e)
        {
            NavigateHome();
        }

        private void ButtonFieldLog_Click(object sender, EventArgs e)
        {
            if (!EnsureGarminAvailable()) return;

            var fieldLog = new FieldLog();
            fieldLog.ReadFieldLogXml(m_settings.FieldLogXml);

            using (FieldLogForm fieldLogForm = new FieldLogForm
            {
                LogList = fieldLog.LogList
            })
            {
                int count = fieldLog.LogList.Count;

                if (fieldLogForm.ShowDialog() != DialogResult.OK) return;

                if (!string.IsNullOrEmpty(fieldLogForm.CacheIdToSearch))
                {
                    textBoxGeocacheId.Text = fieldLogForm.CacheIdToSearch;
                    Search(fieldLogForm.CacheIdToSearch);
                }

                if (fieldLog.LogList.Count == count) return;

                string prompt;

                if (fieldLog.LogList.Count > 0)
                {
                    prompt = string.Format(GcDownload.Strings.PromptDeleteFieldLogEntries, count - fieldLog.LogList.Count);
                }
                else
                {
                    prompt = GcDownload.Strings.PromptDeleteFieldLog;
                }

                if (MessageBox.Show(prompt, GcDownload.Strings.TitleDeleteFieldLog, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                fieldLog.WriteFieldLogXml(m_settings.FieldLogXml);
                fieldLog.WriteFieldLogCsv(m_settings.FieldLogCsv);

                if (fieldLogForm.CacheIdsToBeArchived.Count == 0) return;

                if (MessageBox.Show(GcDownload.Strings.PromptArchive, GcDownload.Strings.TitleArchive, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                if (!m_settings.IsArchivePathValid)
                {
                    if (!ShowSettings()) return;
                    if (!m_settings.IsArchivePathValid) return;
                }

                foreach (string GeocacheId in fieldLogForm.CacheIdsToBeArchived)
                {
                    string source = System.IO.Path.Combine(m_settings.GpxPath, GeocacheId + ".gpx");
                    string target = System.IO.Path.Combine(m_settings.ArchivePath, GeocacheId + ".gpx");

                    try
                    {
                        System.IO.File.Move(source, target);
                    }
                    catch (Exception ex)
                    {
                        m_logger.Debug("Archiving file failed: " + source + ", " + ex.Message);
                    }
                }

                string message = String.Format(GcDownload.Strings.MessageArchivedToDirectory, fieldLogForm.CacheIdsToBeArchived.Count, m_settings.ArchivePath);
                MessageBox.Show(message, GcDownload.Strings.TitleArchive, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ButtonBrowseCaches_Click(object sender, EventArgs e)
        {
            m_logger.Debug("buttonBrowseCaches_Click");

            if (!EnsureGarminAvailable()) return;

            try
            {
                string[] gcFiles = System.IO.Directory.GetFiles(m_settings.GpxPath, "gc*.gpx");
                string[] ocFiles = System.IO.Directory.GetFiles(m_settings.GpxPath, "oc*.gpx");

                string[] gpxFiles = new string[gcFiles.Length + ocFiles.Length];
                gcFiles.CopyTo(gpxFiles, 0);
                ocFiles.CopyTo(gpxFiles, gcFiles.Length);

                var geocacheList = new List<GeocacheGpx>();

                foreach (string filename in gpxFiles)
                {
                    var geocache = new GeocacheGpx(filename);

                    if (geocache.IsValid)
                    {
                        geocacheList.Add(geocache);
                    }
                    else
                    {
                        MessageBox.Show(string.Format(GcDownload.Strings.ErrorInvalidGpxFile, filename), GcDownload.Strings.TitleGeneric, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    };
                }

                var listCachesForm = new ListCachesForm
                {
                    geocacheList = geocacheList
                };

                if (listCachesForm.ShowDialog() != DialogResult.OK) return;

                m_chacheIdsToBeDownloaded = listCachesForm.updateCacheIds;

                if (listCachesForm.cacheIdToSearch.Length > 0)
                {
                    textBoxGeocacheId.Text = listCachesForm.cacheIdToSearch;
                    Search(listCachesForm.cacheIdToSearch);
                }

                if (listCachesForm.deletedCacheIds.Count > 0)
                {
                    if (MessageBox.Show(string.Format(GcDownload.Strings.PromptDeleteGeocacheFiles, listCachesForm.deletedCacheIds.Count.ToString()), GcDownload.Strings.TitleDeleteGeocache, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        foreach (var cacheId in listCachesForm.deletedCacheIds)
                        {
                            try
                            {
                                string filename = System.IO.Path.Combine(m_settings.GpxPath, cacheId + ".gpx");
                                System.IO.File.Delete(filename);
                            }
                            catch (Exception ex)
                            {
                                m_logger.Debug("Delete file failed: " + ex.Message);
                            }
                        }
                    }
                }

                TriggerAutoSearch();
            }
            catch (Exception ex)
            {
                m_logger.Debug("Browse caches failed: " + ex.Message);
            }

        }

        private void ButtonSettings_Click(object sender, EventArgs e)
        {
            ShowSettings();
        }

        private bool EnsureGarminAvailable()
        {
            if (m_settings.IsGarminConnected) return true;
            if (m_settings.AutoDetectGarmin()) return true;

            return ShowSettings();
        }

        private bool ShowSettings()
        {
            using (var settingsForm = new SettingsForm(ref m_settings))
            {
                return (settingsForm.ShowDialog() == DialogResult.OK);
            }
        }

        private void LinkLabelProjectHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            webBrowserPreview.Navigate("http://github.com/4vomAst/gcdownload");
        }

        private void TriggerAutoSearch()
        {
            if (m_chacheIdsToBeDownloaded.Count == 0) return;

            Search(m_chacheIdsToBeDownloaded[0]);
        }

        private void TriggerAutoDownload(string url)
        {
            if (m_chacheIdsToBeDownloaded.Count == 0) return;
            if (!url.ToLower().Contains(m_chacheIdsToBeDownloaded[0].ToLower())) return;

            m_chacheIdsToBeDownloaded.RemoveAt(0);
            Download(false);

            if (m_chacheIdsToBeDownloaded.Count == 0) return;

            System.Threading.Thread.Sleep(5000);
            TriggerAutoSearch();
        }

    }
}

