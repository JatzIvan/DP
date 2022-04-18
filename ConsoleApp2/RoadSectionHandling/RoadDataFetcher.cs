using ApiLibrary.Api;
using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling
{
    public class RoadDataFetcher
    {
        private ApiCallsHandler Handler { get; set; }

        public RoadDataFetcher()
        {
            this.Handler = ApiCallsHandler.GetHandler();
        }

        /**
         * TODO Do more generic implementation
         */
        public List<RoadPointModel> GetRoadFromAPI()
        {

            List<RoadPointModel> output = Handler.Get<List<RoadPointModel>>("roads/" + ApplicationConfigurationHandler.TestRoadQuery);

            // When no road data could be fetched, repeat 3 times and then default with empty list
            // TODO: Implement repeat
            if(output == null)
            {
                return new List<RoadPointModel>();
            }

            output.Sort((RoadPointModel model1, RoadPointModel model2) => model1.OsmId.CompareTo(model2.OsmId));

            return output;
        }

    }
}
