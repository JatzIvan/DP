using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    class AcknowledgeMessage
    {

        public int Index { get; set; }
        public string Type { get; set; }
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp = DateTime.Now;
        public int AcknowledgingIndex { get; set; }

    }
}
