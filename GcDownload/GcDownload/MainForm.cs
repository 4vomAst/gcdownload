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
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Xml;


namespace GcDownload
{

    public partial class MainForm : Form
    {
        private static Mutex traceMutex = new Mutex(false);
        private CSettings settings = new CSettings();
        private Provider provider = Provider.ProviderGeocachingCom;
        private List<string> updateCacheIds = new List<string>();

        enum Provider
        {
            ProviderGeocachingCom,
            ProviderOpencachingDe
        }

        static string LogfileName()
        {
            return Path.GetTempPath() + "GcDownload.log";
        }

        static void WriteToLogfile(string logline, bool append)
        {
            try
            {
                DateTime timestamp = DateTime.Now;
                traceMutex.WaitOne();
                string logFileName = LogfileName();
                StreamWriter writer = new StreamWriter(logFileName, append);
                writer.WriteLine(timestamp.ToShortDateString() + " " + timestamp.ToLongTimeString() + "." + timestamp.Millisecond + " " + logline);
                writer.Close();
                traceMutex.ReleaseMutex();
            }
            catch
            {
            }
        }

        public class LogEntry
        {
            public string Id = "";
            public DateTime Timestamp = DateTime.Now;
            public string Type = "Write note";
            public string FinderId = "";
            public string FinderName = "";
            public string Text = "";
            public string TextEncoded = "False";
        }

        public class FieldLogEntry
        {
            public string CacheId = "";
            public DateTime Timestamp = DateTime.Now;
            public string Type = "Unattempted";
            public string Text = "";
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


        public class GeocacheGpx
        {
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
                WriteToLogfile("Decrypt", true);
                WriteToLogfile("Input: " + encryptedString, true);
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

                WriteToLogfile("Decrypted: " + decryptedString, true);
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
                WriteToLogfile("ImportFromGeocachingCom: " + document.Url, true);
                try
                {
                    try
                    {
                        Name = document.GetElementById("ctl00_ContentBody_CacheName").InnerText;
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract name failed: " + ex.Message, true);
                    }

                    try
                    {
                        GcId = document.GetElementById("ctl00_uxWaypointName").InnerText;
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract GC ID: " + ex.Message, true);
                    }

                    //try
                    //{
                    //    Difficulty = "1";
                    //    foreach (HtmlElement element in document.GetElementById("ctl00_ContentBody_Difficulty").GetElementsByTagName("img"))
                    //    {
                    //        //1 out of 5    1.5 out of 5
                    //        string alternate = element.GetAttribute("alt");
                    //        string searchString = " out of 5";
                    //        if (alternate.EndsWith(searchString))
                    //        {
                    //            Difficulty = alternate.Substring(0, alternate.Length - searchString.Length);
                    //            break;
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    WriteToLogfile("Extract difficulty: " + ex.Message, true);
                    //}

                    //try
                    //{
                    //    Terrain = "1";
                    //    foreach (HtmlElement element in document.GetElementById("ctl00_ContentBody_Terrain").GetElementsByTagName("img"))
                    //    {
                    //        //1 out of 5    1.5 out of 5
                    //        string alternate = element.GetAttribute("alt");
                    //        string searchString = " out of 5";
                    //        if (alternate.EndsWith(searchString))
                    //        {
                    //            Terrain = alternate.Substring(0, alternate.Length - searchString.Length);
                    //            break;
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    WriteToLogfile("Extract terrain: " + ex.Message, true);
                    //}


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
                            string searchStringAlt = " out of 5";

                            if (src.Contains(searchStringUrl))
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
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract difficulty and terrain: " + ex.Message, true);
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
                        WriteToLogfile("Extract container failed: " + ex.Message, true);
                    }

                    try
                    {
                        Type = "Traditional Cache";
                        foreach (HtmlElement element in document.GetElementsByTagName("img"))
                        {
                            string alternate = element.GetAttribute("alt");
                            string src = element.GetAttribute("src");
                            string searchString = "/images/WptTypes/";

                            if (src.Contains(searchString))
                            {
                                Type = alternate;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract type failed: " + ex.Message, true);
                    }

                    try
                    {
                        NumericCacheId = "";
                        foreach (HtmlElement element in document.Links)
                        {
                            string alternate = element.GetAttribute("href");
                            string searchString = "http://www.geocaching.com/seek/log.aspx?ID=";

                            if (alternate.StartsWith(searchString))
                            {
                                NumericCacheId = alternate.Substring(searchString.Length);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract numeric cache id failed: " + ex.Message, true);
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
                        WriteToLogfile("Extract cache status failed: " + ex.Message, true);
                    }


                    //try
                    //{
                    //    Author = document.GetElementById("ctl00_ContentBody_CacheOwner").InnerText;
                    //    if (Author.StartsWith("by "))
                    //    {
                    //        Author = Author.Substring(3);
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    WriteToLogfile("Extract author failed: " + ex.Message, true);
                    //}

                    try
                    {
                        Author = "";
                        foreach (HtmlElement element in document.Links)
                        {
                            string url = element.GetAttribute("href");
                            string searchString = "http://www.geocaching.com/profile/?guid=";

                            if (url.StartsWith(searchString))
                            {
                                Author = element.InnerText;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract name cache id failed: " + ex.Message, true);
                    }




                    try
                    {
                        string searchString = "Hidden : ";
                        int posDate = document.Body.InnerText.IndexOf(searchString) + searchString.Length;
                        int lenDate = document.Body.InnerText.IndexOf(" ", posDate) - posDate;
                        string Date = document.Body.InnerText.Substring(posDate, lenDate);
                        System.Globalization.DateTimeFormatInfo usDateTimeformat = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
                        Timestamp = DateTime.Parse(Date, usDateTimeformat, System.Globalization.DateTimeStyles.AssumeUniversal);
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract date hidden failed: " + ex.Message, true);
                    }

                    try
                    {
                        LatLon = document.GetElementById("ctl00_ContentBody_lnkConversions").GetAttribute("href");
                        LatLon = LatLon.Replace("http://www.geocaching.com/wpt/?lat=", "lat=\"");
                        LatLon = LatLon.Replace("&lon=", "\" lon=\"");
                        LatLon = LatLon.Replace("&detail=1", "\"");
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract location failed: " + ex.Message, true);
                    }

                    try
                    {
                        ShortDescription = document.GetElementById("ctl00_ContentBody_ShortDescription").InnerText;
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract short description failed: " + ex.Message, true);
                    }

                    try
                    {
                        LongDescription = document.GetElementById("ctl00_ContentBody_LongDescription").InnerText;
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract long description failed: " + ex.Message, true);
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
                        WriteToLogfile("Extract hint failed: " + ex.Message, true);
                    }

                    try
                    {
                        // HtmlElement cacheLogTable = document.GetElementById("ctl00_ContentBody_CacheLogs");
                        HtmlElement cacheLogTable = null;
                        HtmlElementCollection tables = document.GetElementsByTagName("table");

                        foreach (HtmlElement table in tables)
                        {
                            if (table.OuterHtml.Contains("<TABLE class=\"LogsTable Table\">"))
                            {
                                cacheLogTable = table;
                                break;
                            }
                        }

                        if (cacheLogTable != null)
                        {
                            int iLogId = 4711000;
                            int iFinderId = 4242000;

                            foreach (HtmlElement cacheLogTableRow in cacheLogTable.GetElementsByTagName("td"))
                            {
                                if (cacheLogTableRow.InnerText == null)
                                {
                                    continue;
                                }
                                LogEntry logEntry = new LogEntry(); //" May 16 by jan-lennart (104 found)\r\n\r\nheute noch mal geocoins gedroppt\r\n\r\nView This Log "
                                string[] stringSeparators = new string[] { "\r\n" };
                                string[] logEntryLines = cacheLogTableRow.InnerText.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                                if (logEntryLines.Length > 1)
                                {
                                    logEntryLines[0] = logEntryLines[0].Trim();
                                    string[] headerStringSeparators = new string[] { " by ", " (" };
                                    string[] headerElements = logEntryLines[0].Split(headerStringSeparators, StringSplitOptions.RemoveEmptyEntries);
                                    if (headerElements.Length > 1)
                                    {
                                        try
                                        {
                                            System.Globalization.DateTimeFormatInfo usDateTimeformat = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
                                            logEntry.Timestamp = DateTime.Parse(headerElements[0], usDateTimeformat, System.Globalization.DateTimeStyles.AssumeUniversal);
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteToLogfile("Extract log timestamp failed: " + ex.Message, true);
                                        }

                                        try
                                        {
                                            logEntry.FinderName = headerElements[1];
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteToLogfile("Extract log finder name failed: " + ex.Message, true);
                                        }

                                        try
                                        {
                                            logEntry.Type = "Write note";
                                            foreach (HtmlElement imgElement in cacheLogTableRow.GetElementsByTagName("img"))
                                            {
                                                string src = imgElement.GetAttribute("src");

                                                if (src.Contains("icon_smile.gif"))
                                                {
                                                    logEntry.Type = "Found it";
                                                    break;
                                                }
                                                else if (src.Contains("icon_sad.gif"))
                                                {
                                                    logEntry.Type = "Didn't find it";
                                                    break;
                                                }
                                                else if (src.Contains("icon_note.gif"))
                                                {
                                                    logEntry.Type = "Write note";
                                                    break;
                                                }
                                                else if (src.Contains("icon_needsmaint.gif"))
                                                {
                                                    logEntry.Type = "Needs Maintenance";
                                                    break;
                                                }
                                                else if (src.Contains("icon_maint.gif"))
                                                {
                                                    logEntry.Type = "Owner Maintenance";
                                                    break;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteToLogfile("Extract log type failed: " + ex.Message, true);
                                        }

                                        try
                                        {
                                            logEntry.TextEncoded = "False";
                                            foreach (HtmlElement linkElement in cacheLogTableRow.GetElementsByTagName("a"))
                                            {
                                                string title = linkElement.GetAttribute("title");

                                                if (title == "Decrypt")
                                                {
                                                    logEntry.TextEncoded = "True";
                                                    break;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteToLogfile("Extract log text failed: " + ex.Message, true);
                                        }

                                        for (int iLine = 1; iLine < logEntryLines.Length - 1; iLine++)
                                        {
                                            if (logEntry.Text.Length > 0)
                                            {
                                                logEntry.Text += "\r\n";
                                            }
                                            logEntry.Text += logEntryLines[iLine];
                                        }

                                        if (logEntry.TextEncoded == "True")
                                        {
                                            if (logEntry.Text.EndsWith("(decrypt)"))
                                            {
                                                logEntry.Text = logEntry.Text.Substring(0, logEntry.Text.Length - 9);
                                            }
                                            logEntry.Text = Decrypt(logEntry.Text);
                                        }

                                        logEntry.Id = iLogId.ToString();
                                        logEntry.FinderId = iFinderId.ToString();
                                        Log.Add(logEntry);
                                        iLogId++;
                                        iFinderId++;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract cache logs failed: " + ex.Message, true);
                    }
                }
                catch (Exception ex)
                {
                    WriteToLogfile("Extraction failed: " + ex.Message, true);
                }
            }

            public void ImportFromOpencachingDe(HtmlDocument document)
            {
                WriteToLogfile("ImportFromOpencachingDe: " + document.Url, true);
                try
                {
                    try
                    {
                        string seachString = " - Geocaching in Deutschland";
                        Name = document.Title;
                        if (Name.Contains(seachString))
                        {
                            Name = Name.Substring(0, Name.IndexOf(seachString));
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract name failed: " + ex.Message, true);
                    }

                    try
                    {
                        string baseInfoText = document.GetElementById("viewcache-baseinfo").InnerText;

                        string[] stringSeparators = new string[] { "\r\n" };
                        string[] baseInfoLines = baseInfoText.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                        WriteToLogfile("viewcache-baseinfo", true);
                        foreach (string line in baseInfoLines)
                        {
                            WriteToLogfile(line, true);
                        }

                        for (int i = 0; i < baseInfoLines.Length; i++)
                        {
                            if (baseInfoLines[i].Contains("(WGS84)"))
                            {
                                string coordinates = baseInfoLines[i].Trim();
                                coordinates = coordinates.Remove(coordinates.IndexOf("(WGS84)"));
                                coordinates = coordinates.Trim();

                                string[] LatLonComponents = coordinates.Split(new Char[] { '°', '\'' }, StringSplitOptions.RemoveEmptyEntries);

                                if (LatLonComponents.Length == 4)
                                {
                                    LatLonComponents[0] = LatLonComponents[0].Trim();
                                    LatLonComponents[0] = LatLonComponents[0].Replace("N ", "");
                                    LatLonComponents[0] = LatLonComponents[0].Replace("S ", "-");

                                    while (LatLonComponents[0].StartsWith("0"))
                                    {
                                        LatLonComponents[0] = LatLonComponents[0].Remove(0, 1);
                                    }
                                    while (LatLonComponents[0].StartsWith("-0"))
                                    {
                                        LatLonComponents[0] = LatLonComponents[0].Remove(1, 1);
                                    }

                                    LatLonComponents[1] = LatLonComponents[1].Trim();
                                    if (System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.Length > 0)
                                    {
                                        string decimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                                        if (LatLonComponents[1].IndexOf(decimalSeparator) == -1)
                                        {
                                            if (decimalSeparator == ".")
                                            {
                                                LatLonComponents[1] = LatLonComponents[1].Replace(',', '.');
                                            }
                                            else if (decimalSeparator == ",")
                                            {
                                                LatLonComponents[1] = LatLonComponents[1].Replace('.', ',');
                                            }
                                        }
                                    }
                                    double minutesLat = System.Convert.ToDouble(LatLonComponents[1]);
                                    double decLat = System.Math.Round(minutesLat * 100000.0 / 60.0, 0);

                                    LatLonComponents[2] = LatLonComponents[2].Trim();
                                    LatLonComponents[2] = LatLonComponents[2].Replace("E ", "");
                                    LatLonComponents[2] = LatLonComponents[2].Replace("W ", "-");
                                    while (LatLonComponents[2].StartsWith("0"))
                                    {
                                        LatLonComponents[2] = LatLonComponents[2].Remove(0, 1);
                                    }
                                    while (LatLonComponents[2].StartsWith("-0"))
                                    {
                                        LatLonComponents[2] = LatLonComponents[2].Remove(1, 1);
                                    }

                                    LatLonComponents[3] = LatLonComponents[3].Trim();
                                    if (System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.Length > 0)
                                    {
                                        string decimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                                        if (LatLonComponents[3].IndexOf(decimalSeparator) == -1)
                                        {
                                            if (decimalSeparator == ".")
                                            {
                                                LatLonComponents[3] = LatLonComponents[3].Replace(',', '.');
                                            }
                                            else if (decimalSeparator == ",")
                                            {
                                                LatLonComponents[3] = LatLonComponents[3].Replace('.', ',');
                                            }
                                        }
                                    }
                                    double minutesLon = System.Convert.ToDouble(LatLonComponents[3]);
                                    double decLon = System.Math.Round(minutesLon * 100000.0 / 60.0, 0);


                                    LatLon = String.Format("lat=\"{0}.{1}\" lon=\"{2}.{3}\"", LatLonComponents[0], decLat.ToString(), LatLonComponents[2], decLon.ToString());
                                }
                            }
                            else if ((baseInfoLines[i].Trim().StartsWith("Größe")) || (baseInfoLines[i].Trim().StartsWith("Size")))
                            {
                                Container = baseInfoLines[i].Substring(baseInfoLines[i].IndexOf(":") + 2).Trim();

                                switch (Container.ToLower())
                                {
                                    case "mikro":
                                        Container = "Micro";
                                        break;
                                    case "klein":
                                        Container = "Small";
                                        break;
                                    case "normal":
                                        Container = "Regular";
                                        break;
                                    case "groß":
                                        Container = "Large";
                                        break;
                                    case "extrem groß":
                                        Container = "Large";
                                        break;
                                    case "andere größe":
                                        Container = "Other";
                                        break;
                                    case "kein behälter":
                                        Container = "Virtual";
                                        break;
                                }
                            }
                            else if ((baseInfoLines[i].Trim().StartsWith("Versteckt am")) || (baseInfoLines[i].Trim().StartsWith("Hidden on")))
                            {
                                try
                                {
                                    string Date = baseInfoLines[i].Substring(baseInfoLines[i].IndexOf(":") + 2).Trim();
                                    System.Globalization.DateTimeFormatInfo usDateTimeformat = new System.Globalization.CultureInfo("de-DE", false).DateTimeFormat;
                                    Timestamp = DateTime.Parse(Date, usDateTimeformat, System.Globalization.DateTimeStyles.AssumeUniversal);
                                }
                                catch (Exception ex)
                                {
                                    WriteToLogfile("Extract date hidden failed: " + ex.Message, true);
                                }
                            }
                            else if ((baseInfoLines[i].Trim().StartsWith("Wegpunkt")) || (baseInfoLines[i].Trim().StartsWith("Waypoint")))
                            {
                                GcId = baseInfoLines[i].Substring(baseInfoLines[i].IndexOf("OC")).Trim();

                                try
                                {
                                    ulong ulNumericCacheId = System.Convert.ToUInt64(GcId.Substring(2), 16);
                                    NumericCacheId = ulNumericCacheId.ToString();
                                }
                                catch (Exception ex)
                                {
                                    WriteToLogfile("Calculation of numerical cache id failed, use random: " + ex.Message, true);
                                    System.Random rnd = new System.Random(System.Environment.TickCount);
                                    NumericCacheId = rnd.Next(1, 65000).ToString();
                                }
                            }
                            else if (baseInfoLines[i].Trim().Contains("Status:"))
                            {
                                if ( (baseInfoLines[i].Trim().Contains("Kann gesucht weden")) || (baseInfoLines[i].Trim().Contains("Available")) )
                                {
                                    Available = true;
                                    Archived = false;
                                }
                                else if ( (baseInfoLines[i].Trim().Contains("Archiviert")) || (baseInfoLines[i].Trim().Contains("Archived")) )
                                {
                                    Available = false;
                                    Archived = true;
                                }
                                else if ((baseInfoLines[i].Trim().Contains("Momentan nicht verfügbar")) || (baseInfoLines[i].Trim().Contains("Temporarily not available")))
                                {
                                    Available = false;
                                    Archived = false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract Location / Container / GC ID: " + ex.Message, true);
                    }

                    try
                    {
                        Difficulty = "1";
                        foreach (HtmlElement element in document.GetElementsByTagName("img"))
                        {
                            string alternate = element.GetAttribute("alt");
                            string src = element.GetAttribute("src");
                            string searchString = "images/difficulty/diff";

                            if (src.Contains(searchString))
                            {
                                Difficulty = alternate.Substring(alternate.IndexOf(':') + 2, 3);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract difficulty: " + ex.Message, true);
                    }

                    try
                    {
                        Terrain = "1";
                        foreach (HtmlElement element in document.GetElementsByTagName("img"))
                        {
                            string alternate = element.GetAttribute("alt");
                            string src = element.GetAttribute("src");
                            string searchString = "images/difficulty/terr";

                            if (src.Contains(searchString))
                            {
                                Terrain = alternate.Substring(alternate.IndexOf(':') + 2, 3);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract terrain: " + ex.Message, true);
                    }

                    try
                    {
                        Type = "Traditional Cache";
                        foreach (HtmlElement element in document.GetElementsByTagName("img"))
                        {
                            string alternate = element.GetAttribute("alt");
                            string src = element.GetAttribute("src");
                            string searchString = "images/cacheicon";

                            if (src.Contains(searchString))
                            {
                                Type = alternate;

                                switch (Type.ToLower())
                                {
                                    case "normaler geocache":
                                        Type = "Traditional Cache";
                                        break;
                                    case "normaler cache":
                                        Type = "Traditional Cache";
                                        break;
                                    case "multicache":
                                        Type = "Multi-Cache";
                                        break;
                                    case "virtueller cache":
                                        Type = "Virtual Cache";
                                        break;
                                    case "virtueller geocache":
                                        Type = "Virtual Cache";
                                        break;
                                    case "event geocache":
                                        Type = "Event Cache";
                                        break;
                                    case "beweglicher geocache":
                                        Type = "Locationless (Reverse) Cache";
                                        break;
                                    case "webcam geocache":
                                        Type = "Webcam Cache";
                                        break;
                                    default:
                                        Type = "Traditional Cache";
                                        break;
                                }
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract type failed: " + ex.Message, true);
                    }


                    try
                    {
                        foreach (HtmlElement element in document.Links)
                        {
                            string alternate = element.GetAttribute("alt");
                            string href = element.GetAttribute("href");
                            string searchString = "viewprofile.php?userid=";

                            if (href.Contains(searchString))
                            {
                                Author = element.InnerText;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract author failed: " + ex.Message, true);
                    }

                    try
                    {
                        foreach (HtmlElement element in document.GetElementsByTagName("img"))
                        {
                            string src = element.GetAttribute("src");
                            string searchString = "images/description/22x22-description.png";

                            if (src.Contains(searchString))
                            {
                                LongDescription = element.Parent.Parent.NextSibling.InnerText;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract long description failed: " + ex.Message, true);
                    }

                    try
                    {
                        HtmlElement element = document.GetElementById("decrypt-hints");

                        if (element != null)
                        {
                            string EncryptedHint = element.InnerText;
                            Hint = Decrypt(EncryptedHint);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract hint failed: " + ex.Message, true);
                    }

                    try
                    {
                        int iLogId = 4711000;
                        int iFinderId = 4242000;

                        try
                        {
                            foreach (HtmlElement element in document.GetElementsByTagName("img"))
                            {
                                try
                                {
                                    string src = element.GetAttribute("src");
                                    string border = element.GetAttribute("border");
                                    string searchString = "ocstyle/images/log/";

                                    if (src.Contains(searchString) && (border.Length == 0))
                                    {
                                        LogEntry logEntry = new LogEntry();

                                        //" 16. Mai 2010 Herzblatt1 hat den Geocache gefunden "
                                        string logTitle = element.Parent.InnerText.Trim();
                                        string[] logTitleComponents = logTitle.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                        if (logTitleComponents.Length > 4)
                                        {
                                            logEntry.FinderName = logTitleComponents[3];

                                            string Date = logTitleComponents[0] + " " + logTitleComponents[1] + " " + logTitleComponents[2];
                                            System.Globalization.DateTimeFormatInfo usDateTimeformat = new System.Globalization.CultureInfo("de-DE", false).DateTimeFormat;
                                            logEntry.Timestamp = DateTime.Parse(Date, usDateTimeformat, System.Globalization.DateTimeStyles.AssumeUniversal);

                                            logEntry.TextEncoded = "False";
                                            logEntry.Text = element.Parent.NextSibling.InnerText;

                                            logEntry.Type = "Write note";

                                            if (src.Contains("16x16-found.png"))
                                            {
                                                logEntry.Type = "Found it";
                                            }
                                            else if (src.Contains("16x16-dnf.png"))
                                            {
                                                logEntry.Type = "Didn't find it";
                                            }

                                            logEntry.Id = iLogId.ToString();
                                            logEntry.FinderId = iFinderId.ToString();
                                            Log.Add(logEntry);
                                            iLogId++;
                                            iFinderId++;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteToLogfile("Extract log entry failed: " + ex.Message, true);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteToLogfile("Extract log failed: " + ex.Message, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogfile("Extract cache logs failed: " + ex.Message, true);
                    }
                }
                catch (Exception ex)
                {
                    WriteToLogfile("Extraction failed: " + ex.Message, true);
                }
            }

            public string ExportToGpx()
            {
                WriteToLogfile("ExportToGpx", true);
                StringBuilder gpxTrack = new StringBuilder(5000);

                try
                {
                    WriteToLogfile("Build Gpx", true);

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
                    gpxTrack.AppendLine(String.Format("<type>Geocache|{0}</type>", Quote(Type)));
                    gpxTrack.AppendLine("<extensions>");
                    gpxTrack.AppendLine(String.Format("<cache id=\"{0}\" available=\"{1}\" archived=\"{2}\" xmlns=\"http://www.groundspeak.com/cache/1/0\">", Quote(NumericCacheId), BoolToString(Available), BoolToString(Archived)));
                    gpxTrack.AppendLine(String.Format("<time>{0}</time>", Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.0Z")));
                    gpxTrack.AppendLine(String.Format("<name>{0}</name>", Quote(namePrefix + Name)));
                    gpxTrack.AppendLine(String.Format("<placed_by>{0}</placed_by>", Quote(Author)));
                    gpxTrack.AppendLine(String.Format("<type>{0}</type>", Quote(Type)));
                    gpxTrack.AppendLine(String.Format("<container>{0}</container>", Quote(Container)));
                    gpxTrack.AppendLine(String.Format("<difficulty>{0}</difficulty>", Quote(Difficulty)));
                    gpxTrack.AppendLine(String.Format("<terrain>{0}</terrain>", Quote(Terrain)));
                    gpxTrack.AppendLine(String.Format("<short_description html=\"False\">{0}</short_description>", Quote(ShortDescription)));
                    gpxTrack.AppendLine(String.Format("<long_description html=\"False\">{0}</long_description>", Quote(LongDescription)));
                    gpxTrack.AppendLine(String.Format("<encoded_hints>{0}</encoded_hints>", Quote(Hint)));
                    if (Log.Count > 0)
                    {
                        gpxTrack.AppendLine("<logs>");
                        foreach (LogEntry logEntry in Log)
                        {
                            gpxTrack.AppendLine(String.Format("<log id=\"{0}\">", Quote(logEntry.Id)));
                            gpxTrack.AppendLine(String.Format("<date>{0}</date>", logEntry.Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.0Z"))); //2010-05-09T19:01:15.8760502Z
                            gpxTrack.AppendLine(String.Format("<type>{0}</type>", Quote(logEntry.Type)));
                            gpxTrack.AppendLine(String.Format("<finder id=\"{0}\">{1}</finder>", Quote(logEntry.FinderId), Quote(logEntry.FinderName)));
                            gpxTrack.AppendLine(String.Format("<text encoded=\"{0}\">{1}</text>", Quote(logEntry.TextEncoded), Quote(logEntry.Text)));
                            gpxTrack.AppendLine("</log>");
                        }
                        gpxTrack.AppendLine("</logs>");
                    }
                    gpxTrack.AppendLine("</cache>");
                    gpxTrack.AppendLine("</extensions>");
                    gpxTrack.AppendLine("</wpt>");
                    gpxTrack.AppendLine("</gpx>");
                    gpxTrack.AppendLine();
                }
                catch (Exception ex)
                {
                    WriteToLogfile("GPX export failed: " + ex.Message, true);
                }

                return gpxTrack.ToString();
            }

            public void ImportFromGpxFile(string filename)
            {
                WriteToLogfile("Import GPX: " + filename, true);

                try
                {
                    XmlDocument document = new XmlDocument();
                    XmlNodeList nodeListWaypoint;

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

                                case "extensions":
                                    foreach (XmlNode extensionsChildnode in wptChildnode.ChildNodes)
                                    {
                                        switch (extensionsChildnode.LocalName.ToLower())
                                        {
                                            case "cache":
                                                NumericCacheId = extensionsChildnode.Attributes.GetNamedItem("id").Value;
                                                Available = StringToBool(extensionsChildnode.Attributes.GetNamedItem("available").Value);
                                                Archived = StringToBool(extensionsChildnode.Attributes.GetNamedItem("archived").Value);
                                                foreach (XmlNode cacheChildnode in extensionsChildnode.ChildNodes)
                                                {
                                                    switch (cacheChildnode.LocalName.ToLower())
                                                    {
                                                        case "time":
                                                            try
                                                            {
                                                                System.Globalization.DateTimeFormatInfo usDateTimeformat = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
                                                                Timestamp = DateTime.Parse(cacheChildnode.InnerText, usDateTimeformat, System.Globalization.DateTimeStyles.AssumeUniversal);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                WriteToLogfile("Parsing timestamp failed: " + cacheChildnode.InnerText + ", " + ex.Message, true);

                                                                try
                                                                {
                                                                    System.Globalization.DateTimeFormatInfo usDateTimeformat = new System.Globalization.CultureInfo("de-DE", false).DateTimeFormat;
                                                                    Timestamp = DateTime.Parse(cacheChildnode.InnerText, usDateTimeformat, System.Globalization.DateTimeStyles.AssumeUniversal);
                                                                }
                                                                catch
                                                                {
                                                                    WriteToLogfile("Parsing timestamp failed", true);
                                                                }
                                                            }
                                                            break;

                                                        case "name":
                                                            Name = cacheChildnode.InnerText;
                                                            break;

                                                        case "placed_by":
                                                            Author = cacheChildnode.InnerText;
                                                            break;

                                                        case "type":
                                                            Type = cacheChildnode.InnerText;
                                                            break;

                                                        case "container":
                                                            Container = cacheChildnode.InnerText;
                                                            break;

                                                        case "difficulty":
                                                            Difficulty = cacheChildnode.InnerText;
                                                            break;

                                                        case "terrain":
                                                            Terrain = cacheChildnode.InnerText;
                                                            break;

                                                        case "short_description":
                                                            ShortDescription = cacheChildnode.InnerText;
                                                            break;

                                                        case "long_description":
                                                            LongDescription = cacheChildnode.InnerText;
                                                            break;

                                                        case "encoded_hints":
                                                            Hint = cacheChildnode.InnerText;
                                                            break;

                                                        case "logs":
                                                            foreach (XmlNode logsChildnode in cacheChildnode.ChildNodes)
                                                            {
                                                                switch (logsChildnode.LocalName.ToLower())
                                                                {
                                                                    case "log":
                                                                        try
                                                                        {
                                                                            LogEntry logEntry = new LogEntry();

                                                                            logEntry.Id = logsChildnode.Attributes.GetNamedItem("id").Value;

                                                                            foreach (XmlNode logChildnode in logsChildnode.ChildNodes)
                                                                            {
                                                                                switch (logChildnode.LocalName.ToLower())
                                                                                {
                                                                                    case "date":
                                                                                        try
                                                                                        {
                                                                                            System.Globalization.DateTimeFormatInfo usDateTimeformat = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
                                                                                            logEntry.Timestamp = DateTime.Parse(logChildnode.InnerText, usDateTimeformat, System.Globalization.DateTimeStyles.AssumeUniversal);
                                                                                        }
                                                                                        catch (Exception ex)
                                                                                        {
                                                                                            WriteToLogfile("Parsing timestamp failed: " + logChildnode.InnerText + ", " + ex.Message, true);

                                                                                            try
                                                                                            {
                                                                                                System.Globalization.DateTimeFormatInfo usDateTimeformat = new System.Globalization.CultureInfo("de-DE", false).DateTimeFormat;
                                                                                                logEntry.Timestamp = DateTime.Parse(logChildnode.InnerText, usDateTimeformat, System.Globalization.DateTimeStyles.AssumeUniversal);
                                                                                            }
                                                                                            catch
                                                                                            {
                                                                                                WriteToLogfile("Parsing timestamp failed", true);
                                                                                            }
                                                                                        }
                                                                                        break;

                                                                                    case "type":
                                                                                        logEntry.Type = logChildnode.InnerText;
                                                                                        break;

                                                                                    case "finder":
                                                                                        logEntry.FinderName = logChildnode.InnerText;
                                                                                        logEntry.FinderId = logChildnode.Attributes.GetNamedItem("id").Value;
                                                                                        break;

                                                                                    case "text":
                                                                                        logEntry.Text = logChildnode.InnerText;
                                                                                        logEntry.TextEncoded = logChildnode.Attributes.GetNamedItem("encoded").Value;
                                                                                        break;
                                                                                }
                                                                            }

                                                                            Log.Add(logEntry);
                                                                        }
                                                                        catch
                                                                        {
                                                                            WriteToLogfile("Parsing log entry failed", true);
                                                                        }
                                                                        break;
                                                                }
                                                            }
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
                }
                catch (Exception ex)
                {
                    WriteToLogfile("Parsing xml file failed: " + ex.Message, true);
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



        public MainForm()
        {
            WriteToLogfile("Starting...", false);
            InitializeComponent();
            webBrowserPreview.ScriptErrorsSuppressed = true;

            selectProvider(Provider.ProviderOpencachingDe);
            navigateHome();

            this.Text += "    V" + GetType().Assembly.GetName().Version.ToString(3);

            WriteToLogfile(this.Text, true);
            //WriteToLogfile(System.Environment.MachineName, true);
            //WriteToLogfile(System.Environment.UserName, true);
            WriteToLogfile(System.Environment.OSVersion.VersionString, true);
            WriteToLogfile(System.Environment.Version.ToString(), true);

            settings.readSettings();
            settings.autoDetectGarmin();
        }

        private void search(string geocacheId)
        {
            string url = "";

            if (geocacheId.ToLower().StartsWith("gc"))
            {
                selectProvider(Provider.ProviderGeocachingCom);
            }
            else if (geocacheId.ToLower().StartsWith("oc"))
            {
                selectProvider(Provider.ProviderOpencachingDe);
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

            WriteToLogfile("Url: " + url, true);
            webBrowserPreview.Navigate(url);
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            WriteToLogfile("buttonSearch_Click", true);

            if (textBoxGeocacheId.Text != "")
            {
                search(textBoxGeocacheId.Text);
            }
            else
            {
                MessageBox.Show(GcDownload.Strings.ErrorEnterGeocacheId, GcDownload.Strings.TitleSearch, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            WriteToLogfile("buttonDownload_Click", true);

            if (!ensureGarminAvailable()) return;

            download(true);

        }

        private void download(bool promptForFilename)
        {
            WriteToLogfile("doGeocacheDownload", true);

            if (!ensureGarminAvailable()) return;

            GeocacheGpx geocacheGpx = new GeocacheGpx();

            switch (provider)
            {
                case Provider.ProviderGeocachingCom:
                    geocacheGpx.ImportFromGeocachingCom(webBrowserPreview.Document);
                    break;

                case Provider.ProviderOpencachingDe:
                    geocacheGpx.ImportFromOpencachingDe(webBrowserPreview.Document);
                    break;
            }

            if (geocacheGpx.IsValid())
            {
                string fileContent = geocacheGpx.ExportToGpx();

                try
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
                        fileDialog.FileName = geocacheGpx.GcId + ".gpx";
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
                        fullGpxFilePath = System.IO.Path.Combine(settings.GpxPath, geocacheGpx.GcId + ".gpx");                        
                    }

                    if (!string.IsNullOrEmpty(fullGpxFilePath))
                    {
                        try
                        {
                            WriteToLogfile("Write to file: " + fullGpxFilePath, true);
                            StreamWriter writer = new StreamWriter(fullGpxFilePath, false, Encoding.UTF8);
                            writer.Write(fileContent);
                            writer.Close();
                        }
                        catch (Exception ex)
                        {
                            WriteToLogfile("Writing to file failed: " + ex.Message, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteToLogfile("Save gpx to file failed: " + ex.Message, true);
                }
            }
            else
            {
                string message = String.Format(GcDownload.Strings.ErrorNoGeocachePageSelected, LogfileName());
                MessageBox.Show(message, GcDownload.Strings.TitleDownload, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void navigateHome()
        {
            WriteToLogfile("navigateHome", true);
            switch (provider)
            {
                case Provider.ProviderGeocachingCom:
                    WriteToLogfile("http://www.geocaching.com/login/", true);
                    webBrowserPreview.Navigate("http://www.geocaching.com/login/");
                    break;

                case Provider.ProviderOpencachingDe:
                    WriteToLogfile("http://www.opencaching.de/", true);
                    webBrowserPreview.Navigate("http://www.opencaching.de/");
                    break;
            }
        }

        private void webBrowserPreview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString().ToLower().Contains("opencaching"))
            {
                selectProvider(Provider.ProviderOpencachingDe);
            }
            else if (e.Url.ToString().ToLower().Contains("geocaching.com"))
            {
                selectProvider(Provider.ProviderGeocachingCom);
            }

            triggerAutoDownload(e.Url.ToString());
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            WriteToLogfile("buttonHome_Click", true);
            navigateHome();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            WriteToLogfile("buttonBack_Click", true);
            webBrowserPreview.GoBack();
        }

        private void selectProvider(Provider newprovider)
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

        private void comboBoxWebsite_SelectedIndexChanged(object sender, EventArgs e)
        {
            WriteToLogfile("comboBoxWebsite_SelectedIndexChanged", true);
            if (comboBoxWebsite.Text == "www.geocaching.com")
            {
                provider = Provider.ProviderGeocachingCom;
            }
            else if (comboBoxWebsite.Text == "www.opencaching.de")
            {
                provider = Provider.ProviderOpencachingDe;
            }
            navigateHome();
        }

        private void buttonFieldLog_Click(object sender, EventArgs e)
        {
            WriteToLogfile("buttonFieldLog_Click", true);

            if (!ensureGarminAvailable()) return;

            List<FieldLogEntry> FieldLog = new List<FieldLogEntry>();

            try
            {
                WriteToLogfile("Open log file: " + settings.FieldLogPath, true);
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
                                WriteToLogfile("Parsing log timestamp failed: " + ex.Message, true);
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
                                search(fieldLogForm.CacheIdToSearch);
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
                                                showSettings();
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
                                                        WriteToLogfile("Archiving file failed: " + source + ", " + ex.Message, true);
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
                WriteToLogfile("Reading log file failed: " + ex.Message, true);
            }
        }

        private void buttonBrowseCaches_Click(object sender, EventArgs e)
        {
            WriteToLogfile("buttonBrowseCaches_Click", true);

            if (!ensureGarminAvailable()) return;

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
                                search(listCachesForm.cacheIdToSearch);
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
                                            WriteToLogfile("Delete file failed: " + ex.Message, true);
                                        }
                                    }
                                }
                            }

                            triggerAutoSearch();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteToLogfile("Browse caches failed: " + ex.Message, true);
            }

        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            showSettings();
        }

        private bool ensureGarminAvailable()
        {
            if (settings.isGarminConnected()) return true;
            if (settings.autoDetectGarmin()) return true;
            if (showSettings()) return true;
            return false;
        }

        private bool showSettings()
        {
            SettingsForm settingsForm = new SettingsForm(ref settings);

            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                return true;
            }

            return false;
        }

        private void linkLabelProjectHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            webBrowserPreview.Navigate("http://code.google.com/p/gcdownload/");
        }

        private void triggerAutoSearch()
        {
            if (updateCacheIds.Count > 0)
            {
                search(updateCacheIds[0]);
            };
        }

        private void triggerAutoDownload(string url)
        {
            if (updateCacheIds.Count > 0)
            {
                if (url.ToLower().Contains(updateCacheIds[0].ToLower()))
                {
                    updateCacheIds.RemoveAt(0);
                    download(false);

                    if (updateCacheIds.Count > 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        triggerAutoSearch();
                    }
                }
            };
        }
    }
}

