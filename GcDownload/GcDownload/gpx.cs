using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GcDownload
{
    [XmlRoot(Namespace = "http://www.topografix.com/GPX/1/1",
         ElementName = "gpx",
         IsNullable = false)]
    public class Gpx
    {
        public Gpx()
        {
            Wpt = new Waypoint();
        }

        [XmlAttribute("schemaLocation", Namespace = XmlSchema.InstanceNamespace)]
        public string xsiSchemaLocation = "http://www.garmin.com/xmlschemas/geocache_visits/v1 geocache_visits.xsd";

        [XmlElement("wpt")]
        public Waypoint Wpt { get; set; }

        [XmlIgnore]
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(Wpt.Extension.Cache.Name)
                    || string.IsNullOrEmpty(Wpt.GeocacheID)
                    || string.IsNullOrEmpty(Wpt.Extension.Cache.PlacedBy)
                    || string.IsNullOrEmpty(Wpt.Lat)
                    || string.IsNullOrEmpty(Wpt.Lon)
                    || (string.IsNullOrEmpty(Wpt.Extension.Cache.ShortDescription.Text) &&
                       string.IsNullOrEmpty(Wpt.Extension.Cache.LongDescription.Text))
                    )
                {
                    return false;
                }

                return true;
            }
        }
    }

    public class Waypoint
    {
        public Waypoint()
        {
            Extension = new WayPointExtension();
        }

        [XmlAttribute(AttributeName = "lat")]
        public string Lat { get; set; }

        [XmlAttribute(AttributeName = "lon")]
        public string Lon { get; set; }

        [XmlElement("name")]
        public string GeocacheID { get; set; }

        [XmlElement("desc")]
        public string WaypointDesc { get; set; }

        [XmlElement("sym")]
        public string WaypointSymbol { get; set; }

        [XmlElement("type")]
        public string WaypointType { get; set; }

        [XmlElement("extensions")]
        public WayPointExtension Extension { get; set; }
    }

    public class WayPointExtension
    {
        public WayPointExtension()
        {
            Cache = new CacheExtension();
        }

        [XmlElement("cache", Namespace = "http://www.groundspeak.com/cache/1/0")]
        public CacheExtension Cache { get; set; }
    }

    public class CacheExtension
    {
        public CacheExtension()
        {
            ShortDescription = new CacheDescription();
            LongDescription = new CacheDescription();
            Logs = new List<Log>();
        }

        [XmlAttribute(AttributeName = "id")]
        public string IdNumber { get; set; }

        [XmlAttribute(AttributeName = "available")]
        public string Available { get; set; }

        [XmlAttribute(AttributeName = "archived")]
        public string Archived { get; set; }

        [XmlElement("time", Order = 1)]
        public string Time { get; set; }

        [XmlElement("name", Order = 2)]
        public string Name { get; set; }

        [XmlElement("placed_by", Order = 3)]
        public string PlacedBy { get; set; }

        [XmlElement("type", Order = 4)]
        public string GeocacheType { get; set; }

        [XmlElement("container", Order = 5)]
        public string Container { get; set; }

        [XmlElement("difficulty", Order = 6)]
        public string Difficulty { get; set; }

        [XmlElement("terrain", Order = 7)]
        public string Terrain { get; set; }

        [XmlElement("short_description", Order = 8)]
        public CacheDescription ShortDescription { get; set; }

        [XmlElement("long_description", Order = 9)]
        public CacheDescription LongDescription { get; set; }

        [XmlElement("encoded_hints", Order = 10)]
        public string EncodedHints { get; set; }

        [XmlArrayItem(ElementName = "log", IsNullable = true, Type = typeof(Log))]
        [XmlArray(ElementName = "logs", Order = 11)]
        public List<Log> Logs;
    }

    public class CacheDescription
    {
        private string html;
        private string element_text;

        [XmlAttribute(AttributeName = "html")]
        public string Html
        {
            get { return html; }
            set { html = value; }
        }

        [XmlText()]
        public string Text
        {
            get { return element_text; }
            set { element_text = value; }
        }
    }

    public class Log
    {
        public Log()
        {
            Text = new LogDescription();
            Finder = new LogFinder();
        }

        [XmlAttribute(AttributeName = "id")]
        public string Id;

        [XmlElement("date")]
        public string LogDate { get; set; }

        [XmlElement("type")]
        public string LogType { get; set; }

        [XmlElement("finder")]
        public LogFinder Finder { get; set; }

        [XmlElement("text")]
        public LogDescription Text { get; set; }
    }

    public class LogDescription
    {
        private string encoded;
        private string element_text;

        [XmlAttribute("encoded")]
        public string Encoded
        {
            get { return encoded; }
            set { encoded = value; }
        }

        [XmlText()]
        public string Text
        {
            get { return element_text; }
            set { element_text = value; }
        }
    }

    public class LogFinder
    {
        private string id;
        private string element_text;

        [XmlAttribute(AttributeName = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [XmlText()]
        public string Name
        {
            get { return element_text; }
            set { element_text = value; }
        }
    }
}
