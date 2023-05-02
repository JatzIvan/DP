using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    public class AreaMessage: AbstractMessage
    {
        [JsonProperty("type")]
        public new string Type { get; set; } = "area";
        
        [JsonProperty("topLeft")]
        public PositionWrapper TopLeft { get; set; }
        
        [JsonProperty("bottomRight")]
        public PositionWrapper BottomRight { get; set; }

    }
}
