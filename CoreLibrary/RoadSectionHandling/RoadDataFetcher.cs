using ApiLibrary.Api;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
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

        /**
         * TODO: Do more generic implementation
         */
        public virtual List<RoadPointModel> GetRoadFromAPI()
        {

            List<RoadPointModel> output = Handler.Get<List<RoadPointModel>>("roads/" + ApplicationConfigurationHandler.TestRoadQuery);

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

        public Dictionary<string, List<RoadPointModel>> GetRoadFromAPIGroupedByRef()
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
                    if (segment.Ref != null && !segment.Ref.Equals(""))
                    {

                        if (!RoadSegmentsByRef.ContainsKey(segment.Ref))
                        {
                            RoadSegmentsByRef.Add(segment.Ref, new List<RoadPointModel>());
                        }

                        RoadSegmentsByRef[segment.Ref].Add(segment);
                    }
                }

            }


            return RoadSegmentsByRef;

        }

        public List<RoadPointModel> GetRoadDataForRef(string roadRef)
        {

            Dictionary<string, List<RoadPointModel>> localData = GetRoadFromAPIGroupedByRef();

            if (localData.ContainsKey(roadRef))
            {
                return localData[roadRef];
            }
            else
            {
                Console.WriteLine("No data found for ref " + roadRef);
                return new List<RoadPointModel>();
            }

        }

    }
}
