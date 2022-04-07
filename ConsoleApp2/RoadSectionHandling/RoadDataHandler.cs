using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling
{
    class RoadDataHandler
    {
        private bool Initialized { get; set; } = false;
        private string SectionName { get; set; }

        private string SectionRef { get; set; }

        private List<RoadPointModel> RoadInfoRaw { get; set; }

        private Dictionary<LocationPoint, RoadCurvitureModel> RoadInfoTransformed { get; set; }

        // Implement with config Object
        public RoadDataHandler(string sectionName, string sectionRef)
        {
            if(String.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentNullException("Handler is missing a name");
            }

            if (String.IsNullOrEmpty(sectionRef))
            {
                throw new ArgumentNullException("Missing road refference");
            }

            this.SectionName = sectionName;
            this.SectionRef = sectionRef;
            this.Initialized = false;
            getParsedRoadData();
 
        }

        public List<RoadPointModel> getRawRoadData()
        {
            if (this.RoadInfoRaw == null || this.RoadInfoRaw.Count == 0)
            {

                // Before API, check database
                // TODO implement later

                // Get Data from API
                List<RoadPointModel> fetchedModel = new RoadDataFetcher().GetRoadFromAPI();
                this.RoadInfoRaw = fetchedModel;
            }

            return RoadInfoRaw;
        }

        public Dictionary<LocationPoint, RoadCurvitureModel> getParsedRoadData()
        {
            if(this.RoadInfoTransformed == null || this.RoadInfoTransformed.Count == 0)
            {
                // First check database -- implement later

                // Then fetch from API

                List<RoadPointModel> fetchedModel = getRawRoadData();
                RoadDataParser parser = new RoadDataParser(fetchedModel);

                // Get connected road points for easier manipulation
                Dictionary<LocationPoint, RoadCurvitureModel> connectedWays = parser.GetConnectedWays();
                CurvesResolver curvesResolver = new CurvesResolver(connectedWays);
                this.RoadInfoTransformed = curvesResolver.CalculateCurvesForWays();
            }



            return this.RoadInfoTransformed;

        }

    }
}
