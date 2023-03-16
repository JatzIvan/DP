using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebSocketLibrary.Models
{
    public class CarUpdateInfo: AbstractMessage
    {
        [JsonProperty("type")]
        public new string Type { get; set; } = "update_vehicles";
        
        [JsonProperty("vehicles")]
        public List<VehicleData> Vehicles { get; set; }


    }
}
