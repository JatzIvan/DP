using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    public class SubscribeMessage
    {

        public int Index { get; set; }

        public string Type = "subscribe";

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp = DateTime.Now;
        
        [JsonConverter(typeof(StringEnumConverter))]
        public SubscribeContent Content { get; set; }

        public float Interval { get; set; }


    }
    public enum SubscribeContent
    {
        vehicles
    }
}
