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
using System.Xml;
using NLog;

namespace GcDownload
{
    public class GeocacheGpx
    {
        private static Logger m_logger = LogManager.GetCurrentClassLogger();

        public string Name = "";
        public string GcId = "";
        public string NumericCacheId = "";
        public string Type = "Traditional Cache";
        public string Difficulty = "1";
        public string Terrain = "1";
        public string Container = "Small";
        public string Author = "";
        public string LatLon = "";
        public string ShortDescription = "";
        public string LongDescription = "";
        public string Hint = "";
        public bool Available = true;
        public bool Archived = false;
        public DateTime Timestamp = DateTime.Now;
        public DateTime FileTimestamp = DateTime.Now;
        List<LogEntry> Log = new List<LogEntry>();

        string BoolToString(bool val)
        {
            if (val) return "True"; else return "False";
        }

        bool StringToBool(string val)
        {
            return val.ToLower() != "false";
        }

        public string Decrypt(string encryptedString)
        {
            m_logger.Debug("Decrypt");
            m_logger.Debug("Input: " + encryptedString);
            string decryptedString = "";

            for (int i = 0; i < encryptedString.Length; i++)
            {
                char c = encryptedString[i];

                if ((encryptedString[i] >= 'A') && (encryptedString[i] <= 'M'))
                {
                    c = (char)(encryptedString[i] - 'A' + 'N');
                }
                else if ((encryptedString[i] >= 'a') && (encryptedString[i] <= 'm'))
                {
                    c = (char)(encryptedString[i] - 'a' + 'n');
                }
                else if ((encryptedString[i] >= 'N') && (encryptedString[i] <= 'Z'))
                {
                    c = (char)(encryptedString[i] - 'N' + 'A');
                }
                else if ((encryptedString[i] >= 'n') && (encryptedString[i] <= 'z'))
                {
                    c = (char)(encryptedString[i] - 'n' + 'a');
                }

                decryptedString += c;
            }

            m_logger.Debug("Decrypted: " + decryptedString);
            return decryptedString;
        }

        public string Quote(string originalString)
        {
            string quotedString = "";

            for (int i = 0; i < originalString.Length; i++)
            {
                string c = originalString[i].ToString();

                switch (originalString[i])
                {
                    case '&':
                        {
                            string remainingString = originalString.Substring(i).ToLower();

                            if (remainingString.StartsWith("&amp;")
                                || remainingString.StartsWith("&apos;")
                                || remainingString.StartsWith("&lt;")
                                || remainingString.StartsWith("&gt;")
                                || remainingString.StartsWith("&quot;")
                                )
                            {
                                //nothing to do
                            }
                            else
                            {
                                c = "&amp;";
                            }
                        }
                        break;

                    case '\'':
                        c = "&apos;";
                        break;

                    case '<':
                        c = "&lt;";
                        break;

                    case '>':
                        c = "&gt;";
                        break;

                    case '"':
                        c = "&quot;";
                        break;
                }

                quotedString += c;
            }

            return quotedString;
        }

        public void ImportFromGeocachingCom(HtmlDocument document)
        {
            m_logger.Debug("ImportFromGeocachingCom: " + document.Url);
            try
            {
                try
                {
                    Name = document.GetElementById("ctl00_ContentBody_CacheName").InnerText;
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract name failed: " + ex.Message);
                }

                try
                {
                    //GcId = document.GetElementById("ctl00_uxWaypointName").InnerText;
                    //GcId = document.GetElementById("ctl00_ContentBody_uxWaypointName").InnerText;
                    GcId = document.GetElementById("ctl00_ContentBody_CoordInfoLinkControl1_uxCoordInfoCode").InnerText;
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract GC ID: " + ex.Message);
                }

                try
                {
                    bool foundDifficulty = false;
                    bool foundTerrain = false;

                    Difficulty = "1";
                    Terrain = "1";

                    foreach (HtmlElement element in document.GetElementsByTagName("img"))
                    {
                        string alternate = element.GetAttribute("alt");
                        string src = element.GetAttribute("src");
                        string searchStringUrl = "http://www.geocaching.com/images/stars/";
                        string searchStringUrl2 = "https://www.geocaching.com/images/stars/";
                        string searchStringAlt = " out of 5";

                        if (src.Contains(searchStringUrl) || src.Contains(searchStringUrl2))
                        {
                            if (alternate.EndsWith(searchStringAlt))
                            {
                                if (!foundDifficulty)
                                {
                                    Difficulty = alternate.Substring(0, alternate.Length - searchStringAlt.Length);
                                    foundDifficulty = true;
                                }
                                else if (!foundTerrain)
                                {
                                    Terrain = alternate.Substring(0, alternate.Length - searchStringAlt.Length);
                                    foundTerrain = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        if (foundDifficulty && foundTerrain) break;
                    }
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract difficulty and terrain: " + ex.Message);
                }

                try
                {
                    Container = "Small";
                    foreach (HtmlElement element in document.GetElementsByTagName("img"))
                    {
                        string alternate = element.GetAttribute("alt");
                        string searchString = "Size: ";

                        if (alternate.StartsWith(searchString))
                        {
                            Container = alternate.Substring(searchString.Length);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract container failed: " + ex.Message);
                }

                try
                {
                    Type = "Traditional Cache";

                    var cacheImage = document.GetElementById("uxCacheImage").InnerHtml;
                    var posTitle = cacheImage.IndexOf("<title>");
                    var posTitleEnd = cacheImage.IndexOf("</title>");

                    if ( (posTitle != -1) && (posTitleEnd != -1) )
                    {
                        Type = cacheImage.Substring(posTitle + 7, posTitleEnd - posTitle - 7);
                    }

                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract type failed: " + ex.Message);
                }

                try
                {
                    NumericCacheId = "";
                    foreach (HtmlElement element in document.Links)
                    {
                        string alternate = element.GetAttribute("href");
                        string searchString = "https://www.geocaching.com/seek/log.aspx?ID="; // "http://www.geocaching.com/seek/log.aspx?ID=";
                        string searchString2 = "http://www.geocaching.com/seek/log.aspx?ID="; // "http://www.geocaching.com/seek/log.aspx?ID=";

                        if (alternate.StartsWith(searchString))
                        {
                            NumericCacheId = alternate.Substring(searchString.Length);
                            break;
                        }
                        else if (alternate.StartsWith(searchString2))
                        {
                            NumericCacheId = alternate.Substring(searchString2.Length);
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(NumericCacheId) && NumericCacheId.Contains("&"))
                    {
                        NumericCacheId = NumericCacheId.Substring(0, NumericCacheId.IndexOf("&"));
                    }

                    if (string.IsNullOrEmpty(NumericCacheId)) NumericCacheId = this.Name;
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract numeric cache id failed: " + ex.Message);
                }

                try
                {
                    if (document.Body.InnerText.Contains("This cache is temporarily unavailable."))
                    {
                        Available = false;
                        Archived = true;
                    }
                    else if (document.Body.InnerText.Contains("This cache has been archived"))
                    {
                        Available = false;
                        Archived = false;
                    }                        
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract cache status failed: " + ex.Message);
                }

                try
                {
                    Author = "";
                    foreach (HtmlElement element in document.Links)
                    {
                        string url = element.GetAttribute("href");
                        string searchString = "http://www.geocaching.com/profile/?guid=";
                        string searchString2 = "https://www.geocaching.com/profile/?guid=";

                        if (url.StartsWith(searchString))
                        {
                            Author = element.InnerText;
                            break;
                        }
                        else if (url.StartsWith(searchString2))
                        {
                            Author = element.InnerText;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract name cache id failed: " + ex.Message);
                }




                try
                {
                    string searchString = "Hidden : ";
                    if (document.Body.InnerText.IndexOf(searchString) == -1) searchString = "Versteckt : ";
                    int posDate = document.Body.InnerText.IndexOf(searchString) + searchString.Length;
                    int lenDate = document.Body.InnerText.IndexOf(" ", posDate) - posDate;
                    string Date = document.Body.InnerText.Substring(posDate, lenDate);
                    System.Globalization.DateTimeFormatInfo usDateTimeformat = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
                    Timestamp = DateTime.Parse(Date, usDateTimeformat, System.Globalization.DateTimeStyles.AssumeUniversal);
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract date hidden failed: " + ex.Message);
                }

                try
                {
                    var mapLinks = document.GetElementById("ctl00_ContentBody_MapLinks_MapLinks");
                    var mapLink = mapLinks.FirstChild.FirstChild;
                    LatLon = mapLink.InnerHtml;

                    //"<a href=\"https://www.geocaching.com/play/map?lat=49.03842&amp;lng=8.38852\" target=\"_blank\" rel=\"noopener noreferrer\">Geocaching.com-Karte</a>"

                    LatLon = LatLon.Replace("<a href=\"https://www.geocaching.com/play/map?lat=", "lat=\"");
                    LatLon = LatLon.Replace("&amp;lng=", "\" lon=\"");
                    LatLon = LatLon.Substring(0, LatLon.IndexOf(" target="));
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract location failed: " + ex.Message);
                }

                try
                {
                    HtmlElementCollection metaEntries = document.GetElementsByTagName("meta");
                    bool weAreClose = false;

                    foreach (HtmlElement metaEntry in metaEntries)
                    {
                        if (metaEntry.GetAttribute("name") == "og:description")
                        {
                            weAreClose = true;
                        }
                        if (weAreClose && (metaEntry.GetAttribute("name") == "description") )
                        {
                            ShortDescription = metaEntry.GetAttribute("content");
                            break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract short description failed: " + ex.Message);
                }

                try
                {
                    LongDescription = document.GetElementById("ctl00_ContentBody_LongDescription").InnerText;
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract long description failed: " + ex.Message);
                }

                try
                {
                    HtmlElement element = document.GetElementById("div_hint");
                    if ((element != null) && (element.InnerText != null))
                    {
                        string EncryptedHint = element.InnerText;
                        Hint = Decrypt(EncryptedHint);
                    }
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract hint failed: " + ex.Message);
                }

                try
                {
                    // HtmlElement cacheLogTable = document.GetElementById("ctl00_ContentBody_CacheLogs");
                    int logsStartPos = document.Body.InnerText.IndexOf("initialLogs = {");

                    if (logsStartPos != -1)
                    {
                        string logsString = document.Body.InnerText.Substring(logsStartPos);
                        int iLogId = 4711000;
                        int iFinderId = 4242000;

                        const string logTypeMagic = "LogType\":\"";
                        const string logTextMagic = "LogText\":\"";
                        const string logTimestampMagic = "Created\":\"";
                        const string logUserNameMagic = "UserName\":\"";
                        const string logIsEncodedMagic = "IsEncoded\":";

                        int pos = logsString.IndexOf(logTypeMagic);

                        while (pos != -1)
                        {
                            try
                            {
                                LogEntry logEntry = new LogEntry();
                                int startValPos = pos + logTypeMagic.Length;
                                int endValPos = logsString.IndexOf("\",", startValPos);
                                logEntry.Type = logsString.Substring(startValPos, endValPos - startValPos);

                                pos = logsString.IndexOf(logTextMagic, pos);
                                startValPos = pos + logTextMagic.Length;
                                endValPos = logsString.IndexOf("\",", startValPos);
                                logEntry.Text = logsString.Substring(startValPos, endValPos - startValPos);

                                pos = logsString.IndexOf(logTimestampMagic, pos);
                                startValPos = pos + logTimestampMagic.Length;
                                endValPos = logsString.IndexOf("\",", startValPos);

                                try
                                {
                                    System.Globalization.DateTimeFormatInfo usDateTimeformat = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
                                    logEntry.Timestamp = DateTime.Parse(logsString.Substring(startValPos, endValPos - startValPos), usDateTimeformat, System.Globalization.DateTimeStyles.AssumeUniversal);
                                }
                                catch (Exception ex)
                                {
                                    m_logger.Debug("Extract log timestamp failed: " + ex.Message);
                                };

                                pos = logsString.IndexOf(logUserNameMagic, pos);
                                startValPos = pos + logUserNameMagic.Length;
                                endValPos = logsString.IndexOf("\",", startValPos);
                                logEntry.FinderName = logsString.Substring(startValPos, endValPos - startValPos);

                                pos = logsString.IndexOf(logIsEncodedMagic, pos);
                                startValPos = pos + logIsEncodedMagic.Length;
                                endValPos = logsString.IndexOf(",", startValPos);
                                logEntry.TextEncoded = logsString.Substring(startValPos, endValPos - startValPos);

                                if (logEntry.TextEncoded == "True")
                                {
                                    if (logEntry.Text.EndsWith("(decrypt)"))
                                    {
                                        logEntry.Text = logEntry.Text.Substring(0, logEntry.Text.Length - 9);
                                    }
                                    logEntry.Text = Decrypt(logEntry.Text);
                                };

                                logEntry.Id = iLogId.ToString();
                                logEntry.FinderId = iFinderId.ToString();
                                Log.Add(logEntry);
                                iLogId++;
                                iFinderId++;
                            }
                            catch (Exception)
                            {

                            }

                            pos = logsString.IndexOf(logTypeMagic, pos);
                        };
                    };
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract cache logs failed: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                m_logger.Debug("Extraction failed: " + ex.Message);
            }
        }

        public string ExportToGpx()
        {
            m_logger.Debug("ExportToGpx");
            StringBuilder gpxTrack = new StringBuilder(5000);

            try
            {
                m_logger.Debug("Build Gpx");

                System.Globalization.NumberFormatInfo numberFormat = new System.Globalization.NumberFormatInfo();
                numberFormat.NumberDecimalSeparator = ".";
                numberFormat.NegativeSign = "-";
                numberFormat.NumberGroupSeparator = "";

                if ( (!Available))
                {
                    LongDescription = GcDownload.Strings.UnAvailableGeocacheDescriptionPrefix + LongDescription;
                }
                if (Archived)
                {
                    LongDescription = GcDownload.Strings.ArchivedGeocacheDescriptionPrefix + LongDescription;
                }

                string namePrefix = "";
                if ((!Available) || Archived)
                {
                    namePrefix = "(x) ";
                }

                gpxTrack.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                gpxTrack.AppendLine("<gpx xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" version=\"1.1\" creator=\"http://www.groundspeak.com\" xmlns=\"http://www.topografix.com/GPX/1/1\">");
                gpxTrack.AppendLine(String.Format("<wpt {0}>", LatLon));
                gpxTrack.AppendLine(String.Format("<name>{0}</name>", Quote(GcId)));
                gpxTrack.AppendLine(String.Format("<desc>{0} by {1}, {2}</desc>", Quote(Name), Quote(Author), Quote(Type)));
                gpxTrack.AppendLine("<sym>Geocache</sym>");
                if (!string.IsNullOrEmpty(Type)) gpxTrack.AppendLine(String.Format("<type>Geocache|{0}</type>", Quote(Type)));
                gpxTrack.AppendLine("<extensions>");
                try
                {
                    if (!string.IsNullOrEmpty(NumericCacheId))
                    {
                        gpxTrack.AppendLine(String.Format("<cache id=\"{0}\" available=\"{1}\" archived=\"{2}\" xmlns=\"http://www.groundspeak.com/cache/1/0\">", Quote(NumericCacheId), BoolToString(Available), BoolToString(Archived)));
                        try
                        {
                            gpxTrack.AppendLine(String.Format("<time>{0}</time>", Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.0Z")));
                            if (!string.IsNullOrEmpty(Name)) gpxTrack.AppendLine(String.Format("<name>{0}</name>", Quote(namePrefix + Name)));
                            if (!string.IsNullOrEmpty(Author)) gpxTrack.AppendLine(String.Format("<placed_by>{0}</placed_by>", Quote(Author)));
                            if (!string.IsNullOrEmpty(Type)) gpxTrack.AppendLine(String.Format("<type>{0}</type>", Quote(Type)));
                            if (!string.IsNullOrEmpty(Container)) gpxTrack.AppendLine(String.Format("<container>{0}</container>", Quote(Container)));
                            if (!string.IsNullOrEmpty(Difficulty)) gpxTrack.AppendLine(String.Format("<difficulty>{0}</difficulty>", Quote(Difficulty)));
                            if (!string.IsNullOrEmpty(Terrain)) gpxTrack.AppendLine(String.Format("<terrain>{0}</terrain>", Quote(Terrain)));
                            if (!string.IsNullOrEmpty(ShortDescription)) gpxTrack.AppendLine(String.Format("<short_description html=\"False\">{0}</short_description>", Quote(ShortDescription)));
                            if (!string.IsNullOrEmpty(LongDescription)) gpxTrack.AppendLine(String.Format("<long_description html=\"False\">{0}</long_description>", Quote(LongDescription)));
                            if (!string.IsNullOrEmpty(Hint)) gpxTrack.AppendLine(String.Format("<encoded_hints>{0}</encoded_hints>", Quote(Hint)));
                            if (Log.Count > 0)
                            {
                                gpxTrack.AppendLine("<logs>");
                                try
                                {
                                    foreach (LogEntry logEntry in Log)
                                    {
                                        if (string.IsNullOrEmpty(logEntry.Id)) continue;

                                        gpxTrack.AppendLine(String.Format("<log id=\"{0}\">", Quote(logEntry.Id)));

                                        try
                                        {
                                            gpxTrack.AppendLine(String.Format("<date>{0}</date>", logEntry.Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.0Z"))); //2010-05-09T19:01:15.8760502Z
                                            if (!string.IsNullOrEmpty(logEntry.Type)) gpxTrack.AppendLine(String.Format("<type>{0}</type>", Quote(logEntry.Type)));
                                            if ((!string.IsNullOrEmpty(logEntry.FinderId)) && (!string.IsNullOrEmpty(logEntry.FinderName)))
                                            {
                                                gpxTrack.AppendLine(String.Format("<finder id=\"{0}\">{1}</finder>", Quote(logEntry.FinderId), Quote(logEntry.FinderName)));
                                            }
                                            if ((!string.IsNullOrEmpty(logEntry.TextEncoded)) && (!string.IsNullOrEmpty(logEntry.Text)))
                                            {
                                                gpxTrack.AppendLine(String.Format("<text encoded=\"{0}\">{1}</text>", Quote(logEntry.TextEncoded), Quote(logEntry.Text)));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            m_logger.Debug("<log> export failed: " + ex.Message);
                                        }

                                        gpxTrack.AppendLine("</log>");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    m_logger.Debug("<logs> export failed: " + ex.Message);
                                }
                                gpxTrack.AppendLine("</logs>");
                            }
                        }
                        catch (Exception ex)
                        {
                            m_logger.Debug("<cache> export failed: " + ex.Message);
                        }
                        gpxTrack.AppendLine("</cache>");
                    }
                }
                catch (Exception ex)
                {
                    m_logger.Debug("<extensions> export failed: " + ex.Message);
                }
                gpxTrack.AppendLine("</extensions>");
                gpxTrack.AppendLine("</wpt>");
                gpxTrack.AppendLine("</gpx>");
                gpxTrack.AppendLine();
            }
            catch (Exception ex)
            {
                m_logger.Debug("GPX export failed: " + ex.Message);
            }

            return gpxTrack.ToString();
        }

        public void ImportFromGpxFile(string filename)
        {
            m_logger.Debug("Import GPX: " + filename);

            try
            {
                XmlDocument document = new XmlDocument();
                XmlNodeList nodeListWaypoint;

                System.IO.FileInfo fileInfo = new FileInfo(filename);
                FileTimestamp = fileInfo.LastWriteTime;

                document.Load(filename);


                nodeListWaypoint = document.GetElementsByTagName("wpt");

                if (nodeListWaypoint.Count == 1)
                {
                    string lat = nodeListWaypoint[0].Attributes.GetNamedItem("lat").Value;
                    string lon = nodeListWaypoint[0].Attributes.GetNamedItem("lon").Value;
                    LatLon = string.Format("lat=\"{0}\" lon=\"{1}\"", lat, lon);

                    foreach (XmlNode wptChildnode in nodeListWaypoint[0].ChildNodes)
                    {
                        switch (wptChildnode.LocalName.ToLower())
                        {
                            case "name":
                                GcId = wptChildnode.InnerText;
                                break;

                            case "urlname":
                                Name = wptChildnode.InnerText;
                                break;

                            case "desc":
                                ShortDescription = wptChildnode.InnerText;
                                break;

                            case "extensions":
                                foreach (XmlNode extensionsChildnode in wptChildnode.ChildNodes)
                                {
                                    switch (extensionsChildnode.LocalName.ToLower())
                                    {
                                        case "cache":
                                            Available = StringToBool(extensionsChildnode.Attributes.GetNamedItem("available").Value);
                                            Archived = StringToBool(extensionsChildnode.Attributes.GetNamedItem("archived").Value);
                                            foreach (XmlNode cacheChildnode in extensionsChildnode.ChildNodes)
                                            {
                                                switch (cacheChildnode.LocalName.ToLower())
                                                {
                                                    case "name":
                                                        Name = cacheChildnode.InnerText;
                                                        break;

                                                    case "placed_by":
                                                        Author = cacheChildnode.InnerText;
                                                        break;

                                                    case "short_description":
                                                        ShortDescription = cacheChildnode.InnerText;
                                                        break;

                                                    case "long_description":
                                                        LongDescription = cacheChildnode.InnerText;
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(Name))
                {
                    XmlNodeList nodeList = document.GetElementsByTagName("groundspeak:name");

                    if (nodeList.Count > 0)
                    {
                        Name = nodeList[0].InnerText;
                    }
                }

                if (string.IsNullOrEmpty(Author))
                {
                    XmlNodeList nodeList = document.GetElementsByTagName("groundspeak:placed_by");

                    if (nodeList.Count > 0)
                    {
                        Author = nodeList[0].InnerText;
                    }
                }

            }
            catch (Exception ex)
            {
                m_logger.Debug("Parsing xml file failed: " + ex.Message);
            }



        }

        public bool IsValid()
        {
            bool valid = true;

            if ((Name == "")
                || (GcId == "")
                || (Author == "")
                || (LatLon == "")
                || ((ShortDescription == "") && (LongDescription == ""))
                )
            {
                valid = false;
            }

            return valid;
        }
    }
}

