using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebSocketLibrary.Models
{
    public class SubscribeMessage: AbstractMessage
    {

        public new string Type = "subscribe";
        
        [JsonConverter(typeof(StringEnumConverter))]
        public SubscribeContent Content { get; set; }

        public float Interval { get; set; }

        public string Road { get; set; }

    }
    public enum SubscribeContent
    {
        vehicles
    }
}
