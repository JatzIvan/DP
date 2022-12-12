using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace WebSocketLibrary.Models
{
    public class AbstractMessage
    {

        public int Index { get; set; }

        public string Type { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp = DateTime.Now;

    }
}
