using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    public class ConnectMessage : AbstractMessage
    {

        public new string Type { get; set; } = "connect";

    }
}
