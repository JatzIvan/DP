using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace WebSocketLibrary.Models
{
    public class AbstractMessage
    {
        [JsonProperty("index")]
        public int Index { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp = DateTime.Now;

    }
}
