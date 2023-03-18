using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WebSocketLibrary;
using WebSocketLibrary.Models;

namespace CollisionDetector
{
    class CustomCollisionDataHandler : AbstractCollisionDetector
    {

        private RoadDataHandler dataStorage;

        public CustomCollisionDataHandler(RoadDataHandler roadHandler) : base(roadHandler.GetParsedRoadData())
        {
            roadHandler.GatherCurvaturesBetweenVehicles();
            this.dataStorage = roadHandler;
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
                    if(calcMethod != null)
                    {
                        AbstractRoadModel collisionPoint = calcMethod.PerformCollisionCalculations(pair.ElementAt(0), pair.ElementAt(1), currectRoadModel);
                        
                        if (calcMethod.CollisionOccured())
                        {

                            // TODO: find a better spot for braking distance calcs
                            // --------------------------------------------------------------------------------

                            double breakingDistance1 = BrakingDistanceCalculatorUtils.CalculateBrakingDistance(pair.ElementAt(0), dataStorage.SectionRef);
                            double breakingDistance2 = BrakingDistanceCalculatorUtils.CalculateBrakingDistance(pair.ElementAt(1), dataStorage.SectionRef);

                            if (breakingDistance1 > pair.ElementAt(0).Speed * calcMethod.CalculateTTC())
                            {
                                Console.WriteLine("Vehicle 1 breaking distance was higher than distance to collision");
                                Console.WriteLine("Distance to coll " + pair.ElementAt(0).Speed * calcMethod.CalculateTTC() + " - breaking distance " + breakingDistance1);
                            }

                            if (breakingDistance2 > pair.ElementAt(1).Speed * calcMethod.CalculateTTC())
                            {
                                Console.WriteLine("Vehicle 2 breaking distance was higher than distance to collision");
                                Console.WriteLine("Distance to coll " + pair.ElementAt(1).Speed * calcMethod.CalculateTTC() + " - breaking distance " + breakingDistance2);
                            }

                            // --------------------------------------------------------------------------------

                            WarningMessage msgVeh1 = calcMethod.CreateWarningMessage(pair.ElementAt(0));
                            WarningMessage msgVeh2 = calcMethod.CreateWarningMessage(pair.ElementAt(1));
                            
                            // If collision occured, check if one or both cars go above speed limit
                            if(collisionPoint != null)
                            {
                                //double maxAllowedSpeed = dataStorage.GatherCurvaturesBetweenVehicles().Where(pair => pair.Item1.ContainsKey(collisionPoint.CurrentLocation))
                                //                                    .FirstOrDefault().Item2;

                                double maxAllowedSpeed = collisionPoint.MaxSpeed;

                                Console.WriteLine("Max speed " + maxAllowedSpeed);

                                if(pair.ElementAt(0).Speed > maxAllowedSpeed)
                                {
                                    msgVeh1.CollisionSeverity = ICollisionCalculatorImplementation.CollisionSeverity.SEVERE.ToString();
                                }

                                if (pair.ElementAt(1).Speed > maxAllowedSpeed)
                                {
                                    msgVeh2.CollisionSeverity = ICollisionCalculatorImplementation.CollisionSeverity.SEVERE.ToString();
                                }
                            }

                            AbstractSocket socket = WebSocketManagerFactory.GetInstance().GetConnection(data.SocketId);



                            socket.SendMessage(msgVeh1);
                            socket.SendMessage(msgVeh2);
                        
                            // Validate this
                            //socket.AddToMessageQueue(msgVeh1.Index ,msgVeh1);
                            //socket.AddToMessageQueue(msgVeh2.Index, msgVeh2);
                        }
                    }
                }
            }

            Console.WriteLine("Elapsed time (ms) " + sw.ElapsedMilliseconds + " for number of cars " + vehicles.Count);
            Console.WriteLine("Elapsed time (ms) " + (sw.ElapsedTicks / 10000) + " for number of cars " + vehicles.Count);
            Console.WriteLine("Elapsed time (mikro) " + (sw.ElapsedTicks / 10) + " for number of cars " + vehicles.Count);

            if (sw.ElapsedMilliseconds > 100)
            {
                Console.WriteLine("Elapsed time " + sw.ElapsedMilliseconds + " for number of cars " + vehicles.Count);
            }
        }
    }
}
