using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    public class ConnectMessage : AbstractMessage
    {
        [JsonProperty("type")]
        public new string Type { get; set; } = "connect";

    }
}
