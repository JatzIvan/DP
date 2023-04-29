using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    public class VehicleData
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public VehicleType Type { get; set; }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp = DateTime.Now;

        [JsonProperty("speed")]
        public float Speed { get; set; }
        
        [JsonProperty("acceleration")]
        public float Acceleration { get; set; }
        
        [JsonProperty("heading")]
        public float Heading { get; set; }

        [JsonProperty("steeringAngle")]
        public float SteeringAngle { get; set; }

        [JsonProperty("position")]
        public PositionWrapper Position { get; set; }
    }

    public class PositionWrapper
    {

        public PositionWrapper(float Lat, float Lon)
        {
            this.Lat = Lat;
            this.Lon = Lon;
        }

        [JsonProperty("lat")]
        public float Lat { get; set; }

        [JsonProperty("lon")]
        public float Lon { get; set; }
    }
}
