using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    public class UnsubscribeMessage
    {

        public int Index { get; set; }
        public string Type = "unsubscribe";
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp = DateTime.Now;

    }
}
