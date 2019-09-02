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
    internal class GeocacheGpx
    {
        private static Logger m_logger = LogManager.GetCurrentClassLogger();

        internal Gpx m_gpx = new Gpx();

        internal DateTime FileTimestamp = DateTime.Now;

        internal string GcId => m_gpx.Wpt.GeocacheID;
        internal string Name => m_gpx.Wpt.Extension.Cache.Name;
        internal string Author => m_gpx.Wpt.Extension.Cache.PlacedBy;
        internal string ShortDescription => m_gpx.Wpt.Extension.Cache.ShortDescription.Text;

        internal bool IsValid => m_gpx.IsValid;

        internal GeocacheGpx()
        {

        }
        internal GeocacheGpx(string gpxFile)
        {
            ReadGpxFile(gpxFile);
        }


        internal bool ReadGpxFile(string gpxFile)
        {
            do
            {
                if (!File.Exists(gpxFile))
                {
                    m_logger.Info("No gpx file found: {0}", gpxFile);
                    break;
                }

                try
                {
                    m_logger.Debug("Open gpx file: {0}", gpxFile);

                    var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Gpx));

                    using (var reader = File.OpenRead(gpxFile))
                    {
                        m_gpx = (Gpx)xmlSerializer.Deserialize(reader);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    m_logger.Error(ex, "Could not read xml file {0}", gpxFile);
                    m_gpx = new Gpx();
                }

            } while (false);

            return false;
        }

        internal bool WriteGpxFile(string gpxFile)
        {
            if (m_gpx == null) return true;

            try
            {
                m_logger.Debug("Write gpx file: {0}", gpxFile);

                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Gpx));

                using (var writer = File.OpenWrite(gpxFile))
                {
                    xmlSerializer.Serialize(writer, m_gpx);
                }
            }
            catch (Exception ex)
            {
                m_logger.Error(ex, "Could not write gpx file {0}", gpxFile);
            }

            return false;
        }

    }
}

