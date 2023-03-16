using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    class RequestAreaMessage: AbstractMessage
    {
        [JsonProperty("type")]
        public new string Type { get; set; } = "request_area";

    }
}
