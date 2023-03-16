using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    public class WarningMessage: AbstractMessage
    {
        [JsonProperty("type")]
        public new string Type { get; set; } = "warning";
        [JsonProperty("vehicleId")]
        public int VehicleId{ get; set; }
        [JsonProperty("timeToCollision")]
        public double TimeToCollision { get; set; }
        [JsonProperty("collisionType")]
        public string CollisionType { get; set; }
        [JsonProperty("collisionSeverity")]
        public string CollisionSeverity { get; set; }

    }
}
