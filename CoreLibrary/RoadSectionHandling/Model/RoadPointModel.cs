using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.Model
{
    public class RoadPointModel
    {

        public string this[string propertyName]
        {
            get { return GetType().GetProperty(propertyName).GetValue(this, null)?.ToString() ?? ""; }
        }

        public int Id { get; set; }

        [Column(TypeName = "jsonb")]
        public WayModel Way { get; set; }
        [JsonProperty("osm_id")]
        public int OsmId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public bool Tunnel { get; set; }
        public bool Bridge { get; set; }
        public bool Oneway { get; set; }
        public string Ref { get; set; }
        public int ZOrder { get; set; }
        public string Access { get; set; }
        public string Service { get; set; }
        [JsonProperty("class_field")]
        public string ClassField { get; set; }

    }
}
