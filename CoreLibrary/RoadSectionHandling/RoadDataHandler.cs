using CoreLibrary.RoadSectionHandling.CircleCurvitureModel;
using CoreLibrary.RoadSectionHandling.Data;
using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.RoadSimplificators;
using System;
using System.Collections.Generic;
using System.Text;
using static CoreLibrary.RoadSectionHandling.Data.RoadCurvitureCalculatorFactory;
using static CoreLibrary.RoadSectionHandling.Data.SectionSimplificationFactory;

namespace CoreLibrary.RoadSectionHandling
{
    public class RoadDataHandler
    {
        private bool Initialized { get; set; } = false;
        private string SectionName { get; set; }

        private string SectionRef { get; set; }

        private List<RoadPointModel> RoadInfoRaw { get; set; }

        private Dictionary<LocationPoint, AbstractRoadModel> RoadInfoTransformed { get; set; }

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

        /**
         * Return parsed data. Default to DouglasPeucker simplification and simple circle curvature resolver
         */
        public Dictionary<LocationPoint, AbstractRoadModel> GetParsedRoadData()
        {
            return this.GetParsedRoadData(new HandlerSetupConfig(
                new DouglasPeuckerConfig(ApplicationConfigurationHandler.DPTolerance),
                CurvCalcMethods.SIMPLE_CIRCLE));
        }

        /**
         * Return parsed data based on provided config
         */
        public Dictionary<LocationPoint, AbstractRoadModel> GetParsedRoadData(HandlerSetupConfig config)
        {
            if(this.RoadInfoTransformed == null || this.RoadInfoTransformed.Count == 0)
            {
                // First check database -- implement later

                // Then fetch from API

                List<RoadPointModel> fetchedModel = GetRawRoadData();
                
                // Big oops, this means that we did not manage to fetch any data
                if(fetchedModel == null || fetchedModel.Count == 0)
                {
                    Console.Write("Could not fetch any data, check if OSM road data link is valid");
                }
                RoadDataParser parser = new RoadDataParser(fetchedModel);

                // Get connected road points for easier manipulation
                Dictionary<LocationPoint, AbstractRoadModel> connectedWays = parser.GetConnectedWays();


                ISectionSimplificator simplificator = SectionSimplificationFactory.getInstance().GetSimplificatiorImplementation(connectedWays, config.SimplificatorConfig);
                Dictionary<LocationPoint, AbstractRoadModel>  simplifiedModel = simplificator.GetSimplifiedModel();
                ICurvesResolver curvesResolver = RoadCurvitureCalculatorFactory.getInstance().GetResolverImplementation(config.CurvitureResolver, simplifiedModel);
                RoadInfoTransformed = curvesResolver.CalculateCurvesForWays();
            }


            return this.RoadInfoTransformed;

        }

       

    }

    public class HandlerSetupConfig
    {
        public AbstractSimplificationModel SimplificatorConfig { get; set; }
        public CurvCalcMethods CurvitureResolver { get; set; }

        public HandlerSetupConfig(AbstractSimplificationModel simplificatorConfig, CurvCalcMethods curvitureResolver)
        {
            this.SimplificatorConfig = simplificatorConfig;
            this.CurvitureResolver = curvitureResolver;
        }
    }

}



