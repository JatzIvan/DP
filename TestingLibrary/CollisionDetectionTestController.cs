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
using System.Diagnostics;

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
                    _data.Add(new object[] { new VehicleObserverWrapper(roadRaw, 0), Convert.ToBoolean(file.Split("_")[1].Replace(".json", "")), Convert.ToBoolean(file.Split("_")[2].Replace(".json", "")) });

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

            private bool speedExceeded;

            private List<NotifyMessage> messages = new List<NotifyMessage>();

            public TestCollisionDetector(RoadDataHandler roadHandler, bool collisionOccured, bool speedExceeded) : base(roadHandler.GetParsedRoadData())
            {
                roadHandler.GatherCurvaturesBetweenVehicles();
                this.dataStorage = roadHandler;
                this.collisionOccured = collisionOccured;
                this.speedExceeded = speedExceeded;
            }

            public override void PerformActions(VehicleObserverWrapper data)
            {
                Stopwatch sw = Stopwatch.StartNew();
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

                            if (calcMethod.CollisionOccured())
                            {
                                messages.Add(calcMethod.CreateNotificationMessage(pair.ElementAt(0), pair.ElementAt(1), collisionPoint, dataStorage.SectionRef));
                                messages.Add(calcMethod.CreateNotificationMessage(pair.ElementAt(1), pair.ElementAt(0), collisionPoint, dataStorage.SectionRef));
                                
                                Xunit.Assert.Equal(collisionOccured, calcMethod.CollisionOccured());
                                Xunit.Assert.Equal(pair.ElementAt(0).Speed > collisionPoint.MaxSpeed || pair.ElementAt(1).Speed > collisionPoint.MaxSpeed, speedExceeded);

                            }

                        }
                    }

                    if (collisionOccured)
                    {
                        Xunit.Assert.Equal(2 ,messages.Count);
                    }

                    Xunit.Assert.Equal(speedExceeded, messages.Where(msg => msg.Level.Equals(NotificationLevel.danger)).ToList().Count > 0);

                    Console.WriteLine("Elapsed time (ms) " + (sw.ElapsedTicks / 10000) + " for number of cars " + vehicles.Count);
                    Console.WriteLine("Elapsed time (mikro) " + (sw.ElapsedTicks / 10) + " for number of cars " + vehicles.Count);

                }
            }

        }

        // ------------------------------------------------------------ //

        [Theory]
        [MemberData(nameof(VehicleWrapperData.TestData), MemberType = typeof(VehicleWrapperData))]
        public void TestCollisionSituations(VehicleObserverWrapper wrapper, bool collision, bool speedExceeded)
        {
            ApplicationConfigurationHandler.InitConstructors();
            ApplicationConfigurationHandler.CurvetureCalcMethod = typeof(CircumcircleRoadCircleCurvesResolver).Name;
            ApplicationConfigurationHandler.SimplificationMethod = typeof(DouglasPeuckerRoadSectionSimplification).Name;
            ApplicationConfigurationHandler.DPTolerance = 0.3f;
            ApplicationConfigurationHandler.CurvatureTreshold = 0.006f;
            ApplicationConfigurationHandler.CarDistanceSkipTreshold = 500;

            RoadDataHandler handler = new RoadDataHandler("503", "503");
            handler.roadDataFether = roadDataFetcher.Object;

            new TestCollisionDetector(handler, collision, speedExceeded).PerformActions(wrapper);
        }



    }
}
