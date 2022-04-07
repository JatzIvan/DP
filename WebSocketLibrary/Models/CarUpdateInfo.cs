using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    public class CarUpdateInfo
    {
        /*        public string Lon { get; set; }
                public string Lat { get; set; }
                public long Vel { get; set; }
                public long Orientation { get; set; }*/

        public int Index { get; set; }

        public string Type { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp = DateTime.Now;

        public List<VehicleData> Vehicles { get; set; }


    }
}
