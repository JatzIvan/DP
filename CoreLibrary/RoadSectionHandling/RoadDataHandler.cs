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

        private string SectionRef { get; set; }

        private List<RoadPointModel> RoadInfoRaw { get; set; }

        private List<AbstractRoadModel> RoadInfoTransformed { get; set; }

        private List<ValueTuple<Dictionary<LocationPoint, AbstractRoadModel>, double>> MaxSpeedForCurvatureSegments { get; set; } 

        private KdTree<AbstractRoadModel> RoadInfoInTreeForm { get; set; }

        public RoadDataFetcher roadDataFether { get; set; }


        // Implement with config Object
        public RoadDataHandler(string sectionName, string sectionRef)
        {
            if (String.IsNullOrEmpty(sectionName))
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
            this.roadDataFether = RoadDataFetcher.GetInstance();

        }

        public List<RoadPointModel> GetRawRoadData()
        {
            if (this.RoadInfoRaw == null || this.RoadInfoRaw.Count == 0)
            {

                // Before API, check database
                // TODO implement later

                // Get Data from API
                List<RoadPointModel> fetchedModel = roadDataFether.GetRoadDataForRef(SectionRef);
                this.RoadInfoRaw = fetchedModel;
            }

            return RoadInfoRaw;
        }

        /**
         * Return parsed data. Default to DouglasPeucker simplification and simple circle curvature resolver
         */
        public KdTree<AbstractRoadModel> GetParsedRoadData()
        {
            return this.GetParsedRoadData(new HandlerSetupConfig(
                typeof(DouglasPeuckerRoadSectionSimplification).Name,
                typeof(CircumcircleRoadCircleCurvesResolver).Name));
        }

        /**
         * Return parsed data based on provided config
         */
        public KdTree<AbstractRoadModel> GetParsedRoadData(HandlerSetupConfig config)
        {
            if (this.RoadInfoTransformed == null || this.RoadInfoTransformed.Count == 0)
            {
                // First check database -- implement later

                // Then fetch from API

                List<RoadPointModel> fetchedModel = GetRawRoadData();

                Console.WriteLine("Data fetched");

                // Big oops, this means that we did not manage to fetch any data
                if (fetchedModel == null || fetchedModel.Count == 0)
                {
                    Console.Write("Could not fetch any data, check if OSM road data link is valid");
                }
                RoadDataParser parser = new RoadDataParser(fetchedModel);

                // Get connected road points for easier manipulation
                //Dictionary<LocationPoint, AbstractRoadModel> connectedWays = parser.GetConnectedWays();
                List<AbstractRoadModel> connectedWays = parser.GetConnectedWays();


                ISectionSimplificator simplificator = SectionSimplificationFactory.getInstance().GetSimplificatiorImplementation(connectedWays, config.Simplificator);
                List<AbstractRoadModel> simplifiedModel = simplificator.GetSimplifiedModel();
                ICurvesResolver curvesResolver = RoadCurvitureCalculatorFactory.getInstance().GetResolverImplementation(config.CurvitureResolver, simplifiedModel);
                RoadInfoTransformed = curvesResolver.CalculateCurvesForWays();
                MaxSpeedCalculatorFactory.GetInstance().GetImplementation().CalcMaxSpeedsForRoadSegment(RoadInfoTransformed, SectionRef);
                GatherCurvaturesBetweenVehicles();
                RoadInfoInTreeForm = MapParserUtils.CreateKdTreeWithCoordinates(RoadInfoTransformed);
            }

            return this.RoadInfoInTreeForm;

        }

        private AbstractRoadModel GetModel(bool first)
        {

            AbstractRoadModel currentModel = RoadInfoTransformed.First();

            while (true)
            {

                if ((first && currentModel.Previous != null) || (!first && currentModel.Next != null))
                {
                    currentModel = first ? currentModel.Previous.Point : currentModel.Next.Point;
                }
                else
                {
                    break;
                }

            }

            return currentModel;

        }

        public List<ValueTuple<Dictionary<LocationPoint, AbstractRoadModel>, double>> GatherCurvaturesBetweenVehicles()
        {
            
            
            if(MaxSpeedForCurvatureSegments == null || MaxSpeedForCurvatureSegments.Count == 0)
            {
                MaxSpeedForCurvatureSegments = new List<ValueTuple<Dictionary<LocationPoint, AbstractRoadModel>, double>>();
                
                AbstractRoadModel firstPointOfRoad = GetModel(true);
                bool isInCurve = false;

                Dictionary<LocationPoint, AbstractRoadModel> curve = new Dictionary<LocationPoint, AbstractRoadModel>();

                while (true)
                {

                    if(firstPointOfRoad.Next == null)
                    {
                        break;
                    }

                    // Start of curve
                    if (firstPointOfRoad.Next.RadiusOfCurvature > ApplicationConfigurationHandler.CurvatureTreshold)
                    {
                        if (!isInCurve)
                        {
                            curve = new Dictionary<LocationPoint, AbstractRoadModel>();
                        }
                        curve.Add(firstPointOfRoad.CurrentLocation, firstPointOfRoad);
                        isInCurve = true;
                    }
                    else
                    {
                        if (isInCurve)
                        {
                            // Calc max allowed speed
                            // Currently just take the smallest max speed and call it a day

                            double min = Double.MaxValue;

                            foreach(KeyValuePair<LocationPoint, AbstractRoadModel> entry in curve)
                            {
                                if(entry.Value.MaxSpeed < min)
                                {
                                    min = entry.Value.MaxSpeed;
                                }
                            }

                            MaxSpeedForCurvatureSegments.Add((curve, min));

                        }
                        isInCurve = false;
                    }

                    firstPointOfRoad = firstPointOfRoad.Next.Point;
                }
            }


            return MaxSpeedForCurvatureSegments;
        }

        public List<AbstractRoadModel> GetParsedRoadDataList()
        {
            return this.GetParsedRoadDataList(new HandlerSetupConfig(
                typeof(DouglasPeuckerRoadSectionSimplification).Name,
                typeof(CircumcircleRoadCircleCurvesResolver).Name));
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
        public string Simplificator { get; set; }
        public string CurvitureResolver { get; set; }

        public HandlerSetupConfig(string simplificator, string curvitureResolver)
        {
            this.Simplificator = simplificator;
            this.CurvitureResolver = curvitureResolver;
        }
    }

}



