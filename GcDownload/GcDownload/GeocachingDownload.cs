using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;

namespace GcDownload
{
    internal class GeocachingDownload
    {
        private static Logger m_logger = LogManager.GetCurrentClassLogger();

        internal static GeocacheGpx ImportFromGeocaching(HtmlDocument document)
        {
            var geocacheGpx = new GeocacheGpx();

            m_logger.Debug("ImportFromGeocachingCom: " + document.Url);
            try
            {
                try
                {
                    geocacheGpx.Name = document.GetElementById("ctl00_ContentBody_CacheName").InnerText;
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract name failed: " + ex.Message);
                }

                try
                {
                    //GcId = document.GetElementById("ctl00_uxWaypointName").InnerText;
                    //GcId = document.GetElementById("ctl00_ContentBody_uxWaypointName").InnerText;
                    geocacheGpx.GcId = document.GetElementById("ctl00_ContentBody_CoordInfoLinkControl1_uxCoordInfoCode").InnerText;
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract GC ID: " + ex.Message);
                }

                try
                {
                    bool foundDifficulty = false;
                    bool foundTerrain = false;

                    geocacheGpx.Difficulty = "1";
                    geocacheGpx.Terrain = "1";

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
                                    geocacheGpx.Difficulty = alternate.Substring(0, alternate.Length - searchStringAlt.Length);
                                    foundDifficulty = true;
                                }
                                else if (!foundTerrain)
                                {
                                    geocacheGpx.Terrain = alternate.Substring(0, alternate.Length - searchStringAlt.Length);
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
                    geocacheGpx.Container = "Small";
                    foreach (HtmlElement element in document.GetElementsByTagName("img"))
                    {
                        string alternate = element.GetAttribute("alt");
                        string searchString = "Size: ";

                        if (alternate.StartsWith(searchString))
                        {
                            geocacheGpx.Container = alternate.Substring(searchString.Length);
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
                    geocacheGpx.Type = "Traditional Cache";

                    var cacheImage = document.GetElementById("uxCacheImage").InnerHtml;
                    var posTitle = cacheImage.IndexOf("<title>");
                    var posTitleEnd = cacheImage.IndexOf("</title>");

                    if ((posTitle != -1) && (posTitleEnd != -1))
                    {
                        geocacheGpx.Type = cacheImage.Substring(posTitle + 7, posTitleEnd - posTitle - 7);
                    }

                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract type failed: " + ex.Message);
                }

                try
                {
                    geocacheGpx.NumericCacheId = "";
                    foreach (HtmlElement element in document.Links)
                    {
                        string alternate = element.GetAttribute("href");
                        string searchString = "https://www.geocaching.com/seek/log.aspx?ID="; // "http://www.geocaching.com/seek/log.aspx?ID=";
                        string searchString2 = "http://www.geocaching.com/seek/log.aspx?ID="; // "http://www.geocaching.com/seek/log.aspx?ID=";

                        if (alternate.StartsWith(searchString))
                        {
                            geocacheGpx.NumericCacheId = alternate.Substring(searchString.Length);
                            break;
                        }
                        else if (alternate.StartsWith(searchString2))
                        {
                            geocacheGpx.NumericCacheId = alternate.Substring(searchString2.Length);
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(geocacheGpx.NumericCacheId) && geocacheGpx.NumericCacheId.Contains("&"))
                    {
                        geocacheGpx.NumericCacheId = geocacheGpx.NumericCacheId.Substring(0, geocacheGpx.NumericCacheId.IndexOf("&"));
                    }

                    if (string.IsNullOrEmpty(geocacheGpx.NumericCacheId)) geocacheGpx.NumericCacheId = geocacheGpx.Name;
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract numeric cache id failed: " + ex.Message);
                }

                try
                {
                    if (document.Body.InnerText.Contains("This cache is temporarily unavailable."))
                    {
                        geocacheGpx.Available = false;
                        geocacheGpx.Archived = true;
                    }
                    else if (document.Body.InnerText.Contains("This cache has been archived"))
                    {
                        geocacheGpx.Available = false;
                        geocacheGpx.Archived = false;
                    }
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract cache status failed: " + ex.Message);
                }

                try
                {
                    geocacheGpx.Author = "";
                    foreach (HtmlElement element in document.Links)
                    {
                        string url = element.GetAttribute("href");
                        string searchString = "http://www.geocaching.com/profile/?guid=";
                        string searchString2 = "https://www.geocaching.com/profile/?guid=";

                        if (url.StartsWith(searchString))
                        {
                            geocacheGpx.Author = element.InnerText;
                            break;
                        }
                        else if (url.StartsWith(searchString2))
                        {
                            geocacheGpx.Author = element.InnerText;
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
                    geocacheGpx.Timestamp = DateTime.Parse(Date, usDateTimeformat, System.Globalization.DateTimeStyles.AssumeUniversal);
                }
                catch (Exception ex)
                {
                    m_logger.Debug("Extract date hidden failed: " + ex.Message);
                }

                try
                {
                    var mapLinks = document.GetElementById("ctl00_ContentBody_MapLinks_MapLinks");
                    var mapLink = mapLinks.FirstChild.FirstChild;
                    geocacheGpx.LatLon = mapLink.InnerHtml;

                    //"<a href=\"https://www.geocaching.com/play/map?lat=49.03842&amp;lng=8.38852\" target=\"_blank\" rel=\"noopener noreferrer\">Geocaching.com-Karte</a>"

                    geocacheGpx.LatLon = geocacheGpx.LatLon.Replace("<a href=\"https://www.geocaching.com/play/map?lat=", "lat=\"");
                    geocacheGpx.LatLon = geocacheGpx.LatLon.Replace("&amp;lng=", "\" lon=\"");
                    geocacheGpx.LatLon = geocacheGpx.LatLon.Substring(0, geocacheGpx.LatLon.IndexOf(" target="));
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
                        if (weAreClose && (metaEntry.GetAttribute("name") == "description"))
                        {
                            geocacheGpx.ShortDescription = metaEntry.GetAttribute("content");
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
                    geocacheGpx.LongDescription = document.GetElementById("ctl00_ContentBody_LongDescription").InnerText;
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
                        geocacheGpx.Hint = Decrypt(EncryptedHint);
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
                                geocacheGpx.Log.Add(logEntry);
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

            return geocacheGpx;
        }

        private static string Decrypt(string encryptedString)
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

    }
}
