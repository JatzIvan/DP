using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SumoTraceParser.Models
{
    [XmlRoot("timestep")]
    public class Timestep
    {
        [XmlAttribute("time")]
        public double Time { get; set; }
        [XmlElement("vehicle")]
        public List<Vehicle> Vehicles { get; set; }

    }
}
