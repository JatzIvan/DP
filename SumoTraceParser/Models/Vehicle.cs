using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SumoTraceParser.Models
{
    [XmlRoot("vehicle")]
    public class Vehicle
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("x")]
        public float X { get; set; }

        [XmlAttribute("y")]
        public float Y { get; set; }

        [XmlAttribute("angle")]
        public float Angle { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("speed")]
        public float Speed { get; set; }

        [XmlAttribute("pos")]
        public float Pos { get; set; }

        [XmlAttribute("lane")]
        public string Lane { get; set; }

    }
}
