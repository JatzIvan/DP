using Newtonsoft.Json;

namespace CoreLibrary.RoadSectionHandling.Model
{
    public class LocationPoint
    {

        [JsonProperty("long")]
        public double Longitude { get; set; }
        
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        public LocationPoint(double longitude, double latitude)
        {
            this.Longitude = longitude;
            this.Latitude = latitude;
        }

        public override int GetHashCode()
        {
            int hashCode = Longitude.GetHashCode() ^ Latitude.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                LocationPoint p = (LocationPoint)obj;
                return (Longitude.Equals(p.Longitude)) && (Latitude.Equals(p.Latitude));
            }
        }

    }
}