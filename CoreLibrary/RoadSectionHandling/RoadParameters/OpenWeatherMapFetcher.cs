using CoreLibrary.RoadSectionHandling.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace CoreLibrary.RoadSectionHandling.RoadParameters
{
    [RoadDataFetcher]
    internal class OpenWeatherMapFetcher : GenericRoadStateFetcher
    {

        private string APIKey { get; set; }

        public OpenWeatherMapFetcher() : base()
        {

            this.APIKey = ApplicationConfigurationHandler.LoadVariable("OpenWeatherAPIKey");

        }

        public override RoadParameters FetchRoadParameters(LocationPoint point)
        {
            string url = GenerateUrl(point);
            WeatherAppDataWrapper fetchedData = APIHandler.Get<WeatherAppDataWrapper>(url);

            RoadParameters output = new RoadParameters();
            output.Temperature = fetchedData.Main.Temp;
            output.Humidity = fetchedData.Main.Humidity;
            output.Pressure = fetchedData.Main.Pressure;

            return output;
            
        }

        public override string GenerateUrl(LocationPoint point)
        {
            return $"https://api.openweathermap.org/data/2.5/weather?lat={point.Latitude}&lon={point.Longitude}&units=metric&exclude=minutely,hourly,daily,alerts&appid={this.APIKey}";
        }


    }
    public class WeatherAppDataWrapper
    {
        public WeatherAppDataWrapper() { }
/*        [JsonProperty("lat")]
        public float Lat { get; set; }
        [JsonProperty("lon")]
        public float Lon { get; set; }*/
        [JsonProperty("main")]
        public CurrentWeatherAppDataWrapper Main { get; set; }
        [JsonProperty("visibility")]
        public float Visibility { get; set; }
    }

    public class CurrentWeatherAppDataWrapper
    {
        public CurrentWeatherAppDataWrapper() { }
        [JsonProperty("temp")]
        public float Temp { get; set; }
        [JsonProperty("feels_like")]
        public float FeelsLike { get; set; }
        [JsonProperty("pressure")]
        public float Pressure { get; set; }
        [JsonProperty("humidity")]
        public float Humidity { get; set; }


    }

}
