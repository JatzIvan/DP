
using Newtonsoft.Json;

namespace WebSocketLibrary.Models
{
    public class KeepAliveMessage: AbstractMessage
    {
        [JsonProperty("type")]
        public new string Type { get; set; } = "keepalive";

    }
}
