using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GcDownload
{
    [XmlRoot(Namespace = "http://www.garmin.com/xmlschemas/geocache_visits/v1",
         ElementName = "logs",
         IsNullable = false)]
    public class FieldLogEntries
    {
        public FieldLogEntries()
        {
            LogList = new List<FieldLogEntry>();
        }

        [XmlAttribute("schemaLocation", Namespace = XmlSchema.InstanceNamespace)]
        public string xsiSchemaLocation = "http://www.garmin.com/xmlschemas/geocache_visits/v1 geocache_visits.xsd";

        [XmlElement("log")]
        public List<FieldLogEntry> LogList { get; set; }
    }
}
