using CoreLibrary;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using CoreLibrary.RoadSectionHandling.CurvatureCalculations;
using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.RoadSimplificators;
using NetTopologySuite.Index.KdTree;
using System.Diagnostics;
using WebSocketLibrary.Models;
using WebSocketLibrary;
using Xunit;
using Newtonsoft.Json;

namespace TestingLibrary
{
    public class TestController : AbstractTestController
    {

        public TestController() : base()
        {

        }

        [Fact]
        public void TestRoadHandlerWithDefaultConfiguration()
        {
            RoadDataHandler handler = new RoadDataHandler("503", "503");
            handler.roadDataFether = roadDataFetcher.Object;

            List<AbstractRoadModel> parsedList = handler.GetParsedRoadDataList();
            Xunit.Assert.Equal(889, parsedList.Count);

            KdTree<AbstractRoadModel> kdTree = handler.GetParsedRoadData();
            Xunit.Assert.Equal(889, kdTree.Count);
        }

        [Theory]
        [InlineData(0, 889)]
        [InlineData(0.5, 605)]
        [InlineData(1, 415)]
        public void TestRoadSimplificationWithDouglasPeucker(float simplificationTolerance, float expectedNumberOfPoints)
        {

            ApplicationConfigurationHandler.SimplificationMethod = typeof(DouglasPeuckerRoadSectionSimplification).Name;
            ApplicationConfigurationHandler.DPTolerance = simplificationTolerance;

            RoadDataHandler handler = new RoadDataHandler("503", "503");
            handler.roadDataFether = roadDataFetcher.Object;

            List<AbstractRoadModel> parsedList = handler.GetParsedRoadDataList();
            Xunit.Assert.Equal(expectedNumberOfPoints, parsedList.Count);

            KdTree<AbstractRoadModel> kdTree = handler.GetParsedRoadData();
            Xunit.Assert.Equal(expectedNumberOfPoints, kdTree.Count);
        }

        [Theory]
        [InlineData(0, 5, 889)]
        [InlineData(0.5, 5, 553)]
        [InlineData(1, 5, 367)]
        public void TestRoadSimplificationWithLang(float simplificationTolerance, int regionSize, float expectedNumberOfPoints)
        {

            ApplicationConfigurationHandler.SimplificationMethod = typeof(LangRoadSectionSimplification).Name;
            ApplicationConfigurationHandler.DPTolerance = simplificationTolerance;
            ApplicationConfigurationHandler.LangRegionSize = regionSize;

            RoadDataHandler handler = new RoadDataHandler("503", "503");
            handler.roadDataFether = roadDataFetcher.Object;

            List<AbstractRoadModel> parsedList = handler.GetParsedRoadDataList();
            Xunit.Assert.Equal(expectedNumberOfPoints, parsedList.Count);

            KdTree<AbstractRoadModel> kdTree = handler.GetParsedRoadData();
            Xunit.Assert.Equal(expectedNumberOfPoints, kdTree.Count);
        }


        [Theory]
        // 888 is one less than number of points, because we resolve based on curvature between points
        [InlineData(0, 888)]
        [InlineData(0.006, 436)]
        [InlineData(0.01, 344)]
        public void TestCircleCurvatureResolver(float curveTolerance, int numberOfDangerousRegions)
        {

            ApplicationConfigurationHandler.CurvetureCalcMethod = typeof(CircumcircleRoadCircleCurvesResolver).Name;

            RoadDataHandler handler = new RoadDataHandler("503", "503");
            handler.roadDataFether = roadDataFetcher.Object;

            List<AbstractRoadModel> parsedList = handler.GetParsedRoadDataList();

            int count = 0;
            AbstractRoadModel first = parsedList[0];
            while (true)
            {
                if (first.Next == null)
                {
                    break;
                }

                if (first.Next.RadiusOfCurvature > curveTolerance)
                {
                    count++;
                }

                first = first.Next.Point;

            }

            Xunit.Assert.Equal(numberOfDangerousRegions, count);

        }

        [Theory]
        [MemberData(nameof(VehicleWrapperData.TestData), MemberType = typeof(VehicleWrapperData))]
        public void TestCollisionSituations(VehicleObserverWrapper wrapper, bool collision)
        {

            ApplicationConfigurationHandler.CurvetureCalcMethod = typeof(CircumcircleRoadCircleCurvesResolver).Name;
            ApplicationConfigurationHandler.SimplificationMethod = typeof(DouglasPeuckerRoadSectionSimplification).Name;
            ApplicationConfigurationHandler.DPTolerance = 0.5f;
            ApplicationConfigurationHandler.CurvatureTreshold = 0.006f;
            ApplicationConfigurationHandler.CarDistanceSkipTreshold = 500;

            RoadDataHandler handler = new RoadDataHandler("503", "503");
            handler.roadDataFether = roadDataFetcher.Object;

            new TestCollisionDetector(handler, collision).PerformActions(wrapper);
        }

        class TestCollisionDetector : AbstractCollisionDetector
        {

            private RoadDataHandler dataStorage;

            private bool collisionOccured;

            public TestCollisionDetector(RoadDataHandler roadHandler, bool collisionOccured) : base(roadHandler.GetParsedRoadData())
            {
                roadHandler.GatherCurvaturesBetweenVehicles();
                this.dataStorage = roadHandler;
                this.collisionOccured = collisionOccured;
            }

            public override void PerformActions(VehicleObserverWrapper data)
            {
                List<VehicleData> vehicles = data.Data.Vehicles;
                //Console.WriteLine("Num of vehicles recieved " + vehicles.Count);
                if (vehicles.Count >= 2)
                {
                    IEnumerable<IEnumerable<VehicleData>> pairsToCalc = CreateVehiclePairs(vehicles);

                    // Try threading or something, right now I need to ensure that this concept can work
                    foreach (var pair in pairsToCalc)
                    {
                        ICollisionCalculatorImplementation calcMethod = ResolveCollisionCalculatorBasedOnCurvature(pair.ElementAt(0), pair.ElementAt(1));
                        if (calcMethod != null)
                        {
                            AbstractRoadModel collisionPoint = calcMethod.PerformCollisionCalculations(pair.ElementAt(0), pair.ElementAt(1), currectRoadModel);

                            Xunit.Assert.Equal(collisionOccured, calcMethod.CollisionOccured());
                        }
                    }
                }
            }

        }


        public static class VehicleWrapperData
        {
            private static readonly List<object[]> _data = new List<object[]>();

            static VehicleWrapperData(){
                foreach (string file in Directory.EnumerateFiles(@"Resources/VehicleScenarios", "*.json"))
                {
                    string jsonString = File.ReadAllText(file);
                    CarUpdateInfo roadRaw = JsonConvert.DeserializeObject<CarUpdateInfo>(jsonString)!;
                    _data.Add(new object[] { new VehicleObserverWrapper(roadRaw, 0), Convert.ToBoolean(file.Split("_")[1].Replace(".json", "")) });

                }
            }

            public static IEnumerable<object[]> TestData
            {
                get { return _data; }
            }
        }
    }
}