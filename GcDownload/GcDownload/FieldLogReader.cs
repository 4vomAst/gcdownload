using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog;

namespace GcDownload
{
    public class FieldLogReader
    {
        private static Logger m_logger = LogManager.GetCurrentClassLogger();
        public static List<FieldLogEntry> ReadFieldLog(string fieldLogPath)
        {
            var fieldLogEntries = new List<FieldLogEntry>();

            try
            {
                m_logger.Debug("Open log file: " + fieldLogPath);

                var reader = new StreamReader(fieldLogPath, Encoding.Unicode);

                var logLine = reader.ReadLine();

                while (logLine != null)
                {
                    if (logLine.Length == 0) continue;

                    //the split method does not care about quoted strings
                    //replace delimeter characters within quoted strings by \n
                    MasqueradeUnwantedDelimeters(true, ',', ref logLine);

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
                        var LogEntry = new FieldLogEntry
                        {
                            CacheId = values[0]
                        };

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

                        fieldLogEntries.Add(LogEntry);
                    }

                    logLine = reader.ReadLine();
                }

                reader.Close();

            }
            catch(Exception)
            {

            }

            return fieldLogEntries;
        }

        private static void MasqueradeUnwantedDelimeters(bool masquerade, char delimeter, ref string s)
        {
            try
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
            catch (Exception)
            {
            }
        }
    }
}
