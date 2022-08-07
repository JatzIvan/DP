using ConsoleApp2.RoadSectionHandling.CircleCurvitureModel;
using ConsoleApp2.RoadSectionHandling.Data;
using ConsoleApp2.RoadSectionHandling.Model;
using ConsoleApp2.RoadSectionHandling.RoadSimplificators;
using System;
using System.Collections.Generic;
using System.Text;
using static ConsoleApp2.RoadSectionHandling.Data.RoadCurvitureCalculatorFactory;
using static ConsoleApp2.RoadSectionHandling.Data.SectionSimplificationFactory;

namespace ConsoleApp2.RoadSectionHandling
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

        public Dictionary<LocationPoint, AbstractRoadModel> GetParsedRoadData()
        {
            return this.GetParsedRoadData(new HandlerSetupConfig(
                new DouglasPeuckerConfig(ApplicationConfigurationHandler.DPTolerance),
                CurvCalcMethods.SIMPLE_CIRCLE));
        }

        public Dictionary<LocationPoint, AbstractRoadModel> GetParsedRoadData(HandlerSetupConfig config)
        {
            if(this.RoadInfoTransformed == null || this.RoadInfoTransformed.Count == 0)
            {
                // First check database -- implement later

                // Then fetch from API

                List<RoadPointModel> fetchedModel = GetRawRoadData();
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



