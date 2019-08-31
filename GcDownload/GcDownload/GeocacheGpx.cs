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
        public List<LogEntry> Log = new List<LogEntry>();

        string BoolToString(bool val)
        {
            if (val) return "True"; else return "False";
        }

        bool StringToBool(string val)
        {
            return val.ToLower() != "false";
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

