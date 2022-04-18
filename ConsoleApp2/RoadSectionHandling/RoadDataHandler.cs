using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling
{
    public class RoadDataHandler
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
 
        }

        public List<RoadPointModel> GetRawRoadData()
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

        public Dictionary<LocationPoint, RoadCurvitureModel> GetParsedRoadData()
        {
            return this.GetParsedRoadData(ApplicationConfigurationHandler.DPTolerance);
        }

        public Dictionary<LocationPoint, RoadCurvitureModel> GetParsedRoadData(float tolerance)
        {
            if(this.RoadInfoTransformed == null || this.RoadInfoTransformed.Count == 0)
            {
                // First check database -- implement later

                // Then fetch from API

                List<RoadPointModel> fetchedModel = GetRawRoadData();
                RoadDataParser parser = new RoadDataParser(fetchedModel);

                // Get connected road points for easier manipulation
                Dictionary<LocationPoint, RoadCurvitureModel> connectedWays = parser.GetConnectedWays();

                RoadSectionSimplification simplificator = new RoadSectionSimplification(tolerance, connectedWays);
                Dictionary<LocationPoint, RoadCurvitureModel>  simplifiedModel = simplificator.GetSimplifiedModel();

                CurvesResolver curvesResolver = new CurvesResolver(simplifiedModel);
                this.RoadInfoTransformed = curvesResolver.CalculateCurvesForWays();
            }



            return this.RoadInfoTransformed;

        }

       

    }
}



