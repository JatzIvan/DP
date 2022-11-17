using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    public class VehicleData
    {

        public int Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public VehicleType Type { get; set; }

        public float Speed { get; set; }
        public float Acceleration { get; set; }
        public float Heading { get; set; }

        public float SteeringAngle { get; set; }
        public PositionWrapper Position { get; set; }
    }

    public class PositionWrapper
    {

        public PositionWrapper(float Lat, float Lon)
        {
            this.Lat = Lat;
            this.Lon = Lon;
        }

        public float Lat { get; set; }
        public float Lon { get; set; }
    }
}
