
using Newtonsoft.Json;

namespace WebSocketLibrary.Models
{
    public class AcknowledgeMessage: AbstractMessage
    {
        [JsonProperty("type")]
        public new string Type { get; set; } = "acknowledge";
        
        [JsonProperty("acknowledgingIndex")]
        public int AcknowledgingIndex { get; set; }

    }
}
