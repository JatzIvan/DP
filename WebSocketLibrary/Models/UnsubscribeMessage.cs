using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    public class UnsubscribeMessage: AbstractMessage
    {

        public new string Type = "unsubscribe";

    }
}
