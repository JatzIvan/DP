using CoreLibrary.RoadSectionHandling.CurvatureCalculations;
using CoreLibrary.RoadSectionHandling.Data;
using CoreLibrary.RoadSectionHandling.MaxSpeedCalculators;
using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.RoadSimplificators;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CoreLibrary.RoadSectionHandling.Data.RoadCurvitureCalculatorFactory;
using static CoreLibrary.RoadSectionHandling.Data.SectionSimplificationFactory;

namespace CoreLibrary.RoadSectionHandling
{
    public class RoadDataHandler
    {
        private bool Initialized { get; set; } = false;
        private string SectionName { get; set; }

        public string SectionRef { get; set; }

        private List<RoadPointModel> RoadInfoRaw { get; set; }

        private List<AbstractRoadModel> RoadInfoTransformed { get; set; }

        private List<ValueTuple<Dictionary<LocationPoint, AbstractRoadModel>, double>> MaxSpeedForCurvatureSegments { get; set; } 

        private KdTree<AbstractRoadModel> RoadInfoInTreeForm { get; set; }

        public RoadDataFetcher roadDataFether { get; set; }


        // Implement with config Object
        // Do not instantiate with constructor. Use RoadDataManager.GetInstance().AddDataHandler()
        public RoadDataHandler(string sectionName, string sectionRef)
        {
            if (String.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentNullException("Handler is missing a name");
            }

/*            if (String.IsNullOrEmpty(sectionRef))
            {
                throw new ArgumentNullException("Missing road refference");
            }*/

            this.SectionName = sectionName;
            this.SectionRef = sectionRef;
            this.Initialized = false;
            this.roadDataFether = RoadDataFetcher.GetInstance();

        }

        public List<RoadPointModel> GetRawRoadData()
        {
            if (this.RoadInfoRaw == null || this.RoadInfoRaw.Count == 0)
            {

                // Get Data from API
                List<RoadPointModel> fetchedModel = roadDataFether.GetRoadDataForAttr(SectionRef);
                this.RoadInfoRaw = fetchedModel;
            }

            return RoadInfoRaw;
        }

        /**
         * Return parsed data. Default to DouglasPeucker simplification and simple circle curvature resolver
         */
        public KdTree<AbstractRoadModel> GetParsedRoadData()
        {

            return this.GetParsedRoadData(ApplicationConfigurationHandler.GenerateHandlerSetupConfig());
        }

        /**
         * Return parsed data based on provided config
         */
        public KdTree<AbstractRoadModel> GetParsedRoadData(HandlerSetupConfig config)
        {
            if (this.RoadInfoTransformed == null || this.RoadInfoTransformed.Count == 0)
            {

                // fetch from API

                List<RoadPointModel> fetchedModel = GetRawRoadData();

                Logger.GetLogger().WriteLine("Data fetched");

                // Big oops, this means that we did not manage to fetch any data
                if (fetchedModel == null || fetchedModel.Count == 0)
                {
                    Logger.GetLogger().WriteLine("Could not fetch any data, check if OSM road data link is valid");
                }
                RoadDataParser parser = new RoadDataParser(fetchedModel);

                // Get connected road points for easier manipulation
                //Dictionary<LocationPoint, AbstractRoadModel> connectedWays = parser.GetConnectedWays();
                List<AbstractRoadModel> connectedWays = parser.GetConnectedWays();


                ISectionSimplificator simplificator = SectionSimplificationFactory.GetInstance().GetResolverImplementation(config.Simplificator);
                List<AbstractRoadModel> simplifiedModel = simplificator.GetSimplifiedModel(connectedWays);
                ICurvesResolver curvesResolver = RoadCurvitureCalculatorFactory.GetInstance().GetResolverImplementation(config.CurvitureResolver);
                RoadInfoTransformed = curvesResolver.CalculateCurvesForWays(simplifiedModel);
                MaxSpeedCalculatorFactory.GetInstance().GetImplementation().CalcMaxSpeedsForRoadSegment(RoadInfoTransformed, SectionRef);
                //GatherCurvaturesBetweenVehicles();
                RoadInfoInTreeForm = MapParserUtils.CreateKdTreeWithCoordinates(RoadInfoTransformed);
            }

            return this.RoadInfoInTreeForm;

        }

        public void RecalculateRoadModelBasedOnCurrentRoadState()
        {
            List<AbstractRoadModel> modelCopy = RoadInfoTransformed.ConvertAll(roadPoint => roadPoint.Clone());
            
            MaxSpeedCalculations.ISpeedCalculator calculator = MaxSpeedCalculatorFactory.GetInstance().GetImplementation();
            
            foreach(AbstractRoadModel model in modelCopy)
            {
                model.MaxSpeed = calculator.GetMaxSpeed(model, SectionRef);
            }

            KdTree<AbstractRoadModel> roadTreeFormCopy = MapParserUtils.CreateKdTreeWithCoordinates(RoadInfoTransformed);

            lock (RoadInfoTransformed) lock(RoadInfoInTreeForm)
            {
                RoadInfoTransformed = modelCopy;
                RoadInfoInTreeForm= roadTreeFormCopy;
            }

            Logger.GetLogger().WriteLine("Recalculated road model for " + this.SectionRef);

        }

        public List<AbstractRoadModel> GetParsedRoadDataList()
        {
            return this.GetParsedRoadDataList(ApplicationConfigurationHandler.GenerateHandlerSetupConfig());
        }

        public List<AbstractRoadModel> GetParsedRoadDataList(HandlerSetupConfig config)
        {
            if (this.RoadInfoTransformed == null || this.RoadInfoTransformed.Count == 0)
            {
                GetParsedRoadData(config);
            }

            return this.RoadInfoTransformed;

        }
    }

    public class HandlerSetupConfig
    {
        public string Simplificator { get; set; } = typeof(DouglasPeuckerRoadSectionSimplification).Name;
        public string CurvitureResolver { get; set; } = typeof(CircumcircleRoadCircleCurvesResolver).Name;

        public HandlerSetupConfig(string simplificator, string curvitureResolver)
        {

            if(simplificator != null)
            {
                this.Simplificator = simplificator;
            }

            if(curvitureResolver  != null)
            {
                this.CurvitureResolver = curvitureResolver;
            }

        }
    }

}



