using ApiLibrary.Api;
using CoreLibrary.RoadSectionHandling.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketLibrary;

namespace CoreLibrary.RoadSectionHandling
{
    public class RoadDataFetcher
    {
        private ApiCallsHandler Handler { get; set; }

        private List<RoadPointModel> RawFetchedData { get; set; }

        private Dictionary<string, List<RoadPointModel>> RoadSegmentsByRef { get; set; }

        private static RoadDataFetcher INSTANCE;

        public RoadDataFetcher()
        {
            this.Handler = ApiCallsHandler.GetHandler();
        }

        public static RoadDataFetcher GetInstance()
        {

            if(INSTANCE == null)
            {
                INSTANCE = new RoadDataFetcher();
            }

            return INSTANCE;

        }

        public static void ClearInstance()
        {

            INSTANCE = null;

        }

        /**
         * TODO: Do more generic implementation
         */
        public virtual List<RoadPointModel> GetRoadFromAPI()
        {

            List<RoadPointModel> output = null;

            if (ApplicationConfigurationHandler.MapDataOrigin.Equals("local"))
            {
                Console.WriteLine("Loading road data from local file");
                string jsonString = File.ReadAllText(ApplicationConfigurationHandler.LoadVariable("LOCAL_DATA_PATH"));
                output = JsonConvert.DeserializeObject<List<RoadPointModel>>(jsonString)!;
            }
            else
            {
                Console.WriteLine("Loading road data from API");
                output = Handler.Get<List<RoadPointModel>>(
                ApplicationConfigurationHandler.DigitalMapConnection + "/roads/" + ApplicationConfigurationHandler.TestRoadQuery);
            }

            // When no road data could be fetched, repeat 3 times and then default with empty list
            // TODO: Implement repeat
            if(output == null)
            {
                return new List<RoadPointModel>();
            }

            output.Sort((RoadPointModel model1, RoadPointModel model2) => model1.OsmId.CompareTo(model2.OsmId));

            RawFetchedData = output;

            return output;
        }

        public Dictionary<string, List<RoadPointModel>> GetRoadFromAPIGroupedByAttr()
        {

            if(RawFetchedData == null)
            {
                RawFetchedData = GetRoadFromAPI();
            }

            if(RoadSegmentsByRef == null && RawFetchedData != null)
            {

                RoadSegmentsByRef = new Dictionary<string, List<RoadPointModel>>();

                foreach (RoadPointModel segment in RawFetchedData)
                {
                    if (segment[ApplicationConfigurationHandler.RoadGroupByAttribute] != null && !segment[ApplicationConfigurationHandler.RoadGroupByAttribute].Equals(""))
                    {

                        if (!RoadSegmentsByRef.ContainsKey(segment[ApplicationConfigurationHandler.RoadGroupByAttribute]))
                        {
                            RoadSegmentsByRef.Add(segment[ApplicationConfigurationHandler.RoadGroupByAttribute], new List<RoadPointModel>());
                        }

                        RoadSegmentsByRef[segment[ApplicationConfigurationHandler.RoadGroupByAttribute]].Add(segment);
                    }
                }

            }


            return RoadSegmentsByRef;

        }

        public LocationPoint ResolveAttrToLocation(string attr)
        {
            Dictionary<string, List<RoadPointModel>> roadPoints = GetRoadFromAPIGroupedByAttr();
            if (!roadPoints.ContainsKey(attr))
            {
                return null;
            }

            return roadPoints[attr].First().Way.Points.First();

        }

        public List<RoadPointModel> GetRoadDataForAttr(string attr)
        {

            Dictionary<string, List<RoadPointModel>> localData = GetRoadFromAPIGroupedByAttr();

            if (localData.ContainsKey(attr))
            {
                return localData[attr];
            }
            else
            {
                Console.WriteLine($"No data found for {ApplicationConfigurationHandler.RoadGroupByAttribute} " + attr);
                return new List<RoadPointModel>();
            }

        }

    }
}
