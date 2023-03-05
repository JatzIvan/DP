using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SumoTraceParser.Models
{
    [XmlRoot("fcd-export")]
    public class FcdExport
    {
        [XmlElement("timestep")]
        public List<Timestep> Timestemps { get; set; }

    }
}
