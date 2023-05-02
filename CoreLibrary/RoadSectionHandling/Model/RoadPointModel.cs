using Newtonsoft.Json;
using Npgsql;
using RoadSectionHandler;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.Model
{
    public class RoadPointModel : IDatabaseModel<RoadPointModel>
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

        public RoadPointModel MapReaderToObject(NpgsqlDataReader reader)
        {
            int? id = reader["id"] as int?;
            int? osmId = reader["osm_id"] as int?;
            string type = reader["type"] as string;
            string name = reader["name"] as string;
            int? tunnel = reader["tunnel"] as int?;
            int? bridge = reader["bridge"] as int?;
            int? oneway = reader["oneway"] as int?;
            string _ref = reader["ref"] as string;
            int? zorder = reader["zorder"] as int?;
            string access = reader["access"] as string;
            string service = reader["service"] as string;
            string classField = reader["class_field"] as string;
            return new RoadPointModel
            {
                Id = id.Value,
                OsmId = osmId.Value,
                Type = type,
                Name = name,
                Tunnel = tunnel.HasValue && tunnel.Value == 1,
                Bridge = bridge.HasValue && bridge.Value == 1,
                Oneway = oneway.HasValue && oneway.Value == 1,
                Ref = _ref,
                ZOrder = zorder.Value,
                Access = access,
                Service = service,
                ClassField = classField
            };
        }
    }
}
