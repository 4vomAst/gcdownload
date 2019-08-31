using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog;

namespace GcDownload
{
    internal class FieldLog
    {
        private static Logger m_logger = LogManager.GetCurrentClassLogger();

        private FieldLogEntries m_fieldLogEntries = new FieldLogEntries();

        internal List<FieldLogEntry> LogList
        {
            get
            {
                return m_fieldLogEntries.LogList;
            }
        }

        internal bool ReadFieldLogXml(string fieldLogXmlFile)
        {
            do
            {
                if (!File.Exists(fieldLogXmlFile))
                {
                    m_logger.Info("No field log xml file found: {0}", fieldLogXmlFile);
                    break;
                }

                try
                {
                    m_logger.Debug("Open field log file: {0}", fieldLogXmlFile);

                    var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(FieldLogEntries));

                    using (var reader = File.OpenRead(fieldLogXmlFile))
                    {
                        m_fieldLogEntries = (FieldLogEntries)xmlSerializer.Deserialize(reader);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    m_logger.Error(ex, "Could not read xml file {0}", fieldLogXmlFile);
                    m_fieldLogEntries = new FieldLogEntries();
                }

            } while (false);

            return false;
        }

        internal bool WriteFieldLogXml(string fieldLogXmlFile)
        {
            if (m_fieldLogEntries?.LogList == null) return true;

            try
            {
                m_logger.Debug("Write field log XML file: {0}", fieldLogXmlFile);

                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(FieldLogEntries));

                using (var writer = File.OpenWrite(fieldLogXmlFile))
                {
                    xmlSerializer.Serialize(writer, m_fieldLogEntries);
                }
            }
            catch (Exception ex)
            {
                m_logger.Error(ex, "Could not write xml file {0}", fieldLogXmlFile);
            }

            return false;
        }

        internal bool WriteFieldLogCsv(string fieldLogCsvFile)
        {
            if (m_fieldLogEntries?.LogList == null) return true;

            try
            {
                m_logger.Debug("Write field log CSV file: {0}", fieldLogCsvFile);

                using (var writer = new StreamWriter(fieldLogCsvFile, false, Encoding.Unicode))
                {
                    foreach (var LogEntry in m_fieldLogEntries.LogList)
                    {
                        writer.WriteLine(LogEntry.code + "," + LogEntry.time + "," + LogEntry.result + ",\"" + LogEntry.comment + "\"");
                    }

                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                m_logger.Error(ex, "Could not write CSV file {0}", fieldLogCsvFile);
            }

            return false;
        }
    }
}
