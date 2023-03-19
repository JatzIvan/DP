using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketLibrary.Models
{
    public class NotifyMessage: AbstractMessage
    {
        [JsonProperty("type")]
        public new string Type { get; set; } = "notify";
        
        [JsonProperty("vehicleId")]
        public int VehicleId { get; set; }
        
        [JsonProperty("level")]
        [JsonConverter(typeof(StringEnumConverter))]
        public NotificationLevel Level { get; set; }
        
        [JsonProperty("contentType")]
        public string ContentType { get; set; } = "head_collision";
        
        [JsonProperty("content")]
        public HeadCollisionContent Content { get; set; }
        
    }

    public enum NotificationLevel
    {
        info,
        warning,
        danger
    }

    public class HeadCollisionContent
    {
        [JsonProperty("timeToCollision")]
        public double TimeToCollision { get; set; }
        
        [JsonProperty("targetVehicleId")]
        public int TargetVehicleId { get; set; }

        public HeadCollisionContent(double timeToCollision, int targetVehicleId)
        {
            TimeToCollision = timeToCollision;
            TargetVehicleId = targetVehicleId;
        }
    }
}
