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
using System.Threading;
using System.IO;
using System.Xml;
using NLog;

namespace GcDownload
{

    public partial class MainForm : Form
    {
        private static Logger m_logger = LogManager.GetCurrentClassLogger();
        private CSettings settings = new CSettings();
        private Provider provider = Provider.ProviderGeocachingCom;
        private List<string> updateCacheIds = new List<string>();

        enum Provider
        {
            ProviderGeocachingCom,
            ProviderOpencachingDe
        }

        static void MasqueradeUnwantedDelimeters(bool masquerade, char delimeter, ref string s)
        {
            if (masquerade)
            {
                //preserve double quotation marks, replace them with formfeeds
                s = s.Replace("\"\"", "\f");

                bool withinQuotedString = false;
                for (int i = 0; i < s.Length; i++)
                {
                    if (!withinQuotedString)
                    {
                        if (s[i] == '"')
                        {
                            withinQuotedString = true;
                        }
                    }
                    else
                    {
                        if (s[i] == '"')
                        {
                            withinQuotedString = false;
                        }
                        else if (s[i] == delimeter)
                        {
                            char[] sz = s.ToCharArray();
                            sz[i] = '\n';
                            s = new string(sz);
                        }
                    }
                }
                //restore double quotation marks
                s = s.Replace("\f", "\"\"");

            }
            else
            {
                s = s.Replace('\n', delimeter);
            }
        }

        public MainForm()
        {
            m_logger.Debug("Starting...", false);
            InitializeComponent();

            webBrowserPreview.ScriptErrorsSuppressed = true;
            SelectProvider(Provider.ProviderOpencachingDe);
            NavigateHome();

            this.Text += "    V" + GetType().Assembly.GetName().Version.ToString(3);

            m_logger.Debug(this.Text);
            m_logger.Debug(System.Environment.OSVersion.VersionString);
            m_logger.Debug(System.Environment.Version.ToString());

            settings.readSettings();
            settings.autoDetectGarmin();
        }

        private void Search(string geocacheId)
        {
            string url = "";

            if (geocacheId.ToLower().StartsWith("gc"))
            {
                SelectProvider(Provider.ProviderGeocachingCom);
            }
            else if (geocacheId.ToLower().StartsWith("oc"))
            {
                SelectProvider(Provider.ProviderOpencachingDe);
            }

            switch (provider)
            {
                case Provider.ProviderGeocachingCom:
                    url = "http://www.geocaching.com/seek/cache_details.aspx?wp=" + geocacheId;
                    break;

                case Provider.ProviderOpencachingDe:
                    url = "http://www.opencaching.de/viewcache.php?wp=" + geocacheId;
                    break;
            }

            m_logger.Debug("Url: " + url);
            webBrowserPreview.Navigate(url);
        }

        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            m_logger.Debug("buttonSearch_Click");

            if (textBoxGeocacheId.Text != "")
            {
                Search(textBoxGeocacheId.Text);
            }
            else
            {
                MessageBox.Show(GcDownload.Strings.ErrorEnterGeocacheId, GcDownload.Strings.TitleSearch, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            m_logger.Debug("buttonDownload_Click");

            if (!EnsureGarminAvailable()) return;

            Download(true);

        }

        string GetFullGpxFilePath(bool promptForFilename, string gcid)
        {
            string fullGpxFilePath = "";
            if (promptForFilename)
            {
                System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
                fileDialog.InitialDirectory = settings.GpxPath;
                fileDialog.AddExtension = true;
                fileDialog.AutoUpgradeEnabled = true;
                fileDialog.CheckPathExists = true;
                fileDialog.DefaultExt = "gpx";
                fileDialog.FileName = gcid + ".gpx";
                fileDialog.OverwritePrompt = true;
                fileDialog.ValidateNames = true;
                fileDialog.Filter = GcDownload.Strings.FilterGpxFiles;
                fileDialog.FilterIndex = 1;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    fullGpxFilePath = fileDialog.FileName;
                }
            }
            else
            {
                fullGpxFilePath = System.IO.Path.Combine(settings.GpxPath, gcid + ".gpx");
            }

            return fullGpxFilePath;
        }

        private void Download(bool promptForFilename)
        {
            m_logger.Debug("doGeocacheDownload");

            if (!EnsureGarminAvailable()) return;


            switch (provider)
            {
                case Provider.ProviderGeocachingCom:
                    {
                        GeocacheGpx geocacheGpx = new GeocacheGpx();
                        geocacheGpx.ImportFromGeocachingCom(webBrowserPreview.Document);

                        if (geocacheGpx.IsValid())
                        {
                            string fileContent = geocacheGpx.ExportToGpx();

                            try
                            {
                                string fullGpxFilePath = GetFullGpxFilePath(promptForFilename, geocacheGpx.GcId);

                                if (!string.IsNullOrEmpty(fullGpxFilePath))
                                {
                                    try
                                    {
                                        m_logger.Debug("Write to file: " + fullGpxFilePath);
                                        StreamWriter writer = new StreamWriter(fullGpxFilePath, false, Encoding.UTF8);
                                        writer.Write(fileContent);
                                        writer.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        m_logger.Debug("Writing to file failed: " + ex.Message);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                m_logger.Debug("Save gpx to file failed: " + ex.Message);
                            }
                        }
                        else
                        {
                            string message = String.Format(GcDownload.Strings.ErrorNoGeocachePageSelected);
                            MessageBox.Show(message, GcDownload.Strings.TitleDownload, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case Provider.ProviderOpencachingDe:
                    {
                        string title = webBrowserPreview.Document.Title;
                        if (!title.StartsWith("OC")) return;
                        int posBlank = title.IndexOf(" ");
                        if (posBlank == -1) return;

                        string ocCacheId = title.Substring(0, posBlank);

                        DownloadViaOkapi(ocCacheId, promptForFilename);
                    }
                    break;
            }
        }

        public void DownloadViaOkapi(string cacheId, bool promptForFilename)
        {
            string filename = GetFullGpxFilePath(promptForFilename, cacheId);

            DownloadViaOkapi(cacheId, filename);

        }

        public void DownloadViaOkapi(string cacheId, string filename)
        {
            //www.opencaching.de/okapi/services/caches/formatters/gpx?cache_codes=OC4FB6&ns_ground=true&attrs=gc:attrs|gc_ocde:attrs|desc:text&latest_logs=true&consumer_key=wgw9HbSeSXgH7xuWhM2P
            //string url = "http://www.opencaching.de/okapi/services/caches/formatters/gpx?ns_ground=true&latest_logs=true&consumer_key=wgw9HbSeSXgH7xuWhM2P";
            string url = "http://www.opencaching.de/okapi/services/caches/formatters/gpx?ns_ground=true&attrs=gc:attrs|desc:text&latest_logs=true&consumer_key=wgw9HbSeSXgH7xuWhM2P";
            url += "&cache_codes=";
            url += cacheId;

            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.DownloadFile(url, filename);
        }



        private void NavigateHome()
        {
            m_logger.Debug("navigateHome");
            switch (provider)
            {
                case Provider.ProviderGeocachingCom:
                    m_logger.Debug("http://www.geocaching.com/login/");
                    webBrowserPreview.Navigate("http://www.geocaching.com/login/");
                    break;

                case Provider.ProviderOpencachingDe:
                    m_logger.Debug("http://www.opencaching.de/");
                    webBrowserPreview.Navigate("http://www.opencaching.de/");
                    break;
            }
        }

        private void WebBrowserPreview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString().ToLower().Contains("opencaching"))
            {
                SelectProvider(Provider.ProviderOpencachingDe);
            }
            else if (e.Url.ToString().ToLower().Contains("geocaching.com"))
            {
                SelectProvider(Provider.ProviderGeocachingCom);
            }

            TriggerAutoDownload(e.Url.ToString());
        }

        private void ButtonHome_Click(object sender, EventArgs e)
        {
            m_logger.Debug("buttonHome_Click");
            NavigateHome();
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            m_logger.Debug("buttonBack_Click");
            webBrowserPreview.GoBack();
        }

        private void SelectProvider(Provider newprovider)
        {
            switch (newprovider)
            {
                case Provider.ProviderGeocachingCom:
                    comboBoxWebsite.Text = "www.geocaching.com";
                    break;

                case Provider.ProviderOpencachingDe:
                    comboBoxWebsite.Text = "www.opencaching.de";
                    break;
            }

            provider = newprovider;
        }

        private void ComboBoxWebsite_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_logger.Debug("comboBoxWebsite_SelectedIndexChanged");

            if (comboBoxWebsite.Text == "www.geocaching.com")
            {
                provider = Provider.ProviderGeocachingCom;
            }
            else if (comboBoxWebsite.Text == "www.opencaching.de")
            {
                provider = Provider.ProviderOpencachingDe;
            }

            NavigateHome();
        }

        private void ButtonFieldLog_Click(object sender, EventArgs e)
        {
            m_logger.Debug("buttonFieldLog_Click");

            if (!EnsureGarminAvailable()) return;

            List<FieldLogEntry> FieldLog = new List<FieldLogEntry>();

            try
            {
                m_logger.Debug("Open log file: " + settings.FieldLogPath);
                StreamReader reader = new StreamReader(settings.FieldLogPath, Encoding.Unicode);

                string logLine = reader.ReadLine();

                while (logLine != null)
                {
                    if (logLine.Length > 0)
                    {
                        //danger: the split method does not care about quoted strings
                        //replace delimeter characters within quoted strings by \n
                        try
                        {
                            MasqueradeUnwantedDelimeters(true, ',', ref logLine);
                        }
                        catch
                        {
                        }

                        string[] values = logLine.Split(new Char[] { ',' });

                        try
                        {
                            //re-concatenate quoted strings that got splitted because they contained delimeter chars
                            for (int i = 0; i < values.GetLength(0); i++)
                            {
                                values[i] = values[i].Trim();

                                if ((values[i].Length > 1)
                                    && (values[i].StartsWith("\""))
                                    && (values[i].EndsWith("\"")))
                                {
                                    //string enclosed with "
                                    values[i] = values[i].Substring(1, values[i].Length - 2);
                                    values[i] = values[i].Replace("\"\"", "\"");
                                    values[i] = values[i].Trim();
                                }
                                //replace \n again with the delimeter character
                                MasqueradeUnwantedDelimeters(false, ',', ref values[i]);
                            }
                        }
                        catch
                        {
                        }

                        if (values.Length >= 4)
                        {
                            FieldLogEntry LogEntry = new FieldLogEntry();

                            LogEntry.CacheId = values[0];

                            try
                            {
                                System.Globalization.DateTimeFormatInfo usDateTimeformat = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
                                LogEntry.Timestamp = DateTime.Parse(values[1], usDateTimeformat, System.Globalization.DateTimeStyles.AssumeUniversal);
                            }
                            catch (Exception ex)
                            {
                                m_logger.Debug("Parsing log timestamp failed: " + ex.Message);
                            }

                            LogEntry.Type = values[2];
                            LogEntry.Text = values[3];

                            FieldLog.Add(LogEntry);
                        }
                    }

                    logLine = reader.ReadLine();
                }

                reader.Close();

                FieldLogForm fieldLogForm = new FieldLogForm();
                fieldLogForm.FieldLog = FieldLog;

                int count = FieldLog.Count;

                switch (fieldLogForm.ShowDialog())
                {
                    case DialogResult.OK:
                        {
                            if (fieldLogForm.CacheIdToSearch.Length > 0)
                            {
                                textBoxGeocacheId.Text = fieldLogForm.CacheIdToSearch;
                                Search(fieldLogForm.CacheIdToSearch);
                            }

                            if (FieldLog.Count != count)
                            {
                                string prompt = GcDownload.Strings.PromptDeleteFieldLog;

                                if (FieldLog.Count > 0)
                                {
                                    prompt = string.Format(GcDownload.Strings.PromptDeleteFieldLogEntries, count - FieldLog.Count);
                                }

                                if (MessageBox.Show(prompt, GcDownload.Strings.TitleDeleteFieldLog, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    StreamWriter writer = new StreamWriter(settings.FieldLogPath, false, Encoding.Unicode);

                                    foreach (FieldLogEntry LogEntry in FieldLog)
                                    {
                                        writer.WriteLine(LogEntry.CacheId + "," + LogEntry.Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mmZ") + "," + LogEntry.Type + ",\"" + LogEntry.Text + "\"");
                                    }

                                    writer.Close();


                                    if (fieldLogForm.FoundCacheIds.Count > 0)
                                    {
                                        if (MessageBox.Show(GcDownload.Strings.PromptArchive, GcDownload.Strings.TitleArchive, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            if (!settings.isArchivePathValid())
                                            {
                                                ShowSettings();
                                            }
                                            if (settings.isArchivePathValid())
                                            {
                                                foreach (string GeocacheId in fieldLogForm.FoundCacheIds)
                                                {
                                                    string source = System.IO.Path.Combine(settings.GpxPath, GeocacheId + ".gpx");
                                                    string target = System.IO.Path.Combine(settings.ArchivePath, GeocacheId + ".gpx");

                                                    try
                                                    {
                                                        System.IO.File.Move(source, target);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        m_logger.Debug("Archiving file failed: " + source + ", " + ex.Message);
                                                    }
                                                }

                                                string message = String.Format(GcDownload.Strings.MessageArchivedToDirectory, fieldLogForm.FoundCacheIds.Count, settings.ArchivePath);
                                                MessageBox.Show(message, GcDownload.Strings.TitleArchive, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            };
                                        }
                                    }
                                }
                            }

                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                m_logger.Debug("Reading log file failed: " + ex.Message);
            }
        }

        private void ButtonBrowseCaches_Click(object sender, EventArgs e)
        {
            m_logger.Debug("buttonBrowseCaches_Click");

            if (!EnsureGarminAvailable()) return;

            try
            {
                string[] gcFiles = System.IO.Directory.GetFiles(settings.GpxPath, "gc*.gpx");
                string[] ocFiles = System.IO.Directory.GetFiles(settings.GpxPath, "oc*.gpx");

                List<GeocacheGpx> geocacheList = new List<GeocacheGpx>();

                foreach (string filename in gcFiles)
                {
                    GeocacheGpx geocache = new GeocacheGpx();
                    geocache.ImportFromGpxFile(filename);
                    if (geocache.IsValid())
                    {
                        geocacheList.Add(geocache);
                    }
                    else
                    {
                        MessageBox.Show(string.Format(GcDownload.Strings.ErrorInvalidGpxFile, filename), GcDownload.Strings.TitleGeneric, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    };
                }
                foreach (string filename in ocFiles)
                {
                    GeocacheGpx geocache = new GeocacheGpx();
                    geocache.ImportFromGpxFile(filename);
                    if (geocache.IsValid())
                    {
                        geocacheList.Add(geocache);
                    }
                    else
                    {
                        MessageBox.Show(string.Format(GcDownload.Strings.ErrorInvalidGpxFile, filename), GcDownload.Strings.TitleGeneric, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    };
                }

                ListCachesForm listCachesForm = new ListCachesForm();
                listCachesForm.geocacheList = geocacheList;
                switch (listCachesForm.ShowDialog())
                {
                    case DialogResult.OK:
                        {
                            updateCacheIds = listCachesForm.updateCacheIds;

                            if (listCachesForm.cacheIdToSearch.Length > 0)
                            {
                                textBoxGeocacheId.Text = listCachesForm.cacheIdToSearch;
                                Search(listCachesForm.cacheIdToSearch);
                            }

                            if (listCachesForm.deletedCacheIds.Count > 0)
                            {
                                if (MessageBox.Show(string.Format(GcDownload.Strings.PromptDeleteGeocacheFiles, listCachesForm.deletedCacheIds.Count.ToString()), GcDownload.Strings.TitleDeleteGeocache, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    foreach (string cacheId in listCachesForm.deletedCacheIds)
                                    {
                                        try
                                        {
                                            string filename = System.IO.Path.Combine(settings.GpxPath, cacheId + ".gpx");
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
                        break;
                }
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
            if (settings.isGarminConnected()) return true;
            if (settings.autoDetectGarmin()) return true;
            if (ShowSettings()) return true;
            return false;
        }

        private bool ShowSettings()
        {
            SettingsForm settingsForm = new SettingsForm(ref settings);

            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                return true;
            }

            return false;
        }

        private void LinkLabelProjectHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            webBrowserPreview.Navigate("http://github.com/4vomAst/gcdownload");
        }

        private void TriggerAutoSearch()
        {
            if (updateCacheIds.Count > 0)
            {
                Search(updateCacheIds[0]);
            };
        }

        private void TriggerAutoDownload(string url)
        {
            if (updateCacheIds.Count > 0)
            {
                if (url.ToLower().Contains(updateCacheIds[0].ToLower()))
                {
                    updateCacheIds.RemoveAt(0);
                    Download(false);

                    if (updateCacheIds.Count > 0)
                    {
                        System.Threading.Thread.Sleep(5000);
                        TriggerAutoSearch();
                    }
                }
            };
        }
    }
}

