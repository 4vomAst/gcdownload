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

        public CacheExtension Cache {get; set;}
    }

    [XmlRoot(Namespace = "http://www.groundspeak.com/cache/1/0",
         ElementName = "cache",
         IsNullable = false)]
    public class CacheExtension
    {
        public CacheExtension()
        {
            ShortDescription = new CacheDescription() { IsHtmlContent = "False" };
            LongDescription = new CacheDescription() { IsHtmlContent = "False" };
            Logs = new List<Log>();
        }

        [XmlAttribute(AttributeName = "id")]
        public string IdNumber { get; set; }

        [XmlAttribute(AttributeName = "available")]
        public string Available { get; set; }

        [XmlAttribute(AttributeName = "archived")]
        public string Archived { get; set; }

        [XmlElement("time")]
        public string Time { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("placed_by")]
        public string PlacedBy { get; set; }

        [XmlElement("type")]
        public string GeocacheType { get; set; }

        [XmlElement("container")]
        public string Container { get; set; }

        [XmlElement("difficulty")]
        public string Difficulty { get; set; }

        [XmlElement("terrain")]
        public string Terrain { get; set; }

        [XmlElement("short_description")]
        public CacheDescription ShortDescription { get; set; }

        [XmlElement("long_description")]
        public CacheDescription LongDescription { get; set; }

        [XmlElement("encoded_hints")]
        public string EncodedHints { get; set; }

        [XmlElement("logs")]
        public List<Log> Logs;
    }

    public class CacheDescription
    {
        [XmlAttribute(AttributeName = "html")]
        public string IsHtmlContent;

        public string Content;
    }

    public class Log
    {
        public Log()
        {
            Finder = new LogFinder();
            Text = new LogText() { Encoded = "False" };
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
        public LogText Text { get; set; }
    }

    public class LogFinder
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id;
    }

    public class LogText
    {
        [XmlAttribute(AttributeName = "encoded")]
        public string Encoded;
    }
}
