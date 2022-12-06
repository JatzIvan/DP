using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    public class AreaMessage: AbstractMessage
    {

        public new string Type { get; set; } = "area";
        public PositionWrapper TopLeft { get; set; }
        public PositionWrapper BottomRight { get; set; }

    }
}
