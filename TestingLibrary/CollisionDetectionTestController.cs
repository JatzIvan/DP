using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketLibrary.Models;
using WebSocketLibrary;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.CurvatureCalculations;
using CoreLibrary.RoadSectionHandling.RoadSimplificators;
using CoreLibrary;
using System.Reflection;
using Xunit;

namespace TestingLibrary
{
    [Collection("Sequential")]
    public class CollisionDetectionTestController: AbstractTestController
    {
        
        /**
         * Data setup
         */
        public static class VehicleWrapperData
        {
            private static readonly List<object[]> _data = new List<object[]>();

            static VehicleWrapperData()
            {
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

        /**
         * Setup for Test collision detector
         */
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

        // ------------------------------------------------------------ //

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



    }
}
