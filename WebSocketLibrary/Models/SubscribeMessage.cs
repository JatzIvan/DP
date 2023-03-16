using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebSocketLibrary.Models
{
    public class SubscribeMessage: AbstractMessage
    {
        [JsonProperty("type")]
        public new string Type = "subscribe";

        [JsonProperty("content")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SubscribeContent Content { get; set; }
        
        [JsonProperty("interval")]
        public float Interval { get; set; }
        
        [JsonProperty("road")]
        public string Road { get; set; }

    }
    public enum SubscribeContent
    {
        vehicles
    }
}
