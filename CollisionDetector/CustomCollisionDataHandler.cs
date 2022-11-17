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

        public override void PerformCalculations(ObserverWrapper data)
        {

            Stopwatch sw = Stopwatch.StartNew();

            if(data.Data.Count >= 2)
            {
                IEnumerable<IEnumerable<VehicleData>> pairsToCalc = CreateVehiclePairs(data.Data);

                // Try threading or something, right now I need to ensure that this concept can work
                foreach (var pair in pairsToCalc)
                {
                    ICollisionCalculatorImplementation calcMethod = ResolveCollisionCalculatorBasedOnCurvature(pair.ElementAt(0), pair.ElementAt(1));
                    if(calcMethod != null)
                    {
                        AbstractRoadModel collisionPoint = calcMethod.PerformCollisionCalculations(pair.ElementAt(0), pair.ElementAt(1), currectRoadModel);
                        
                        if (calcMethod.CollisionOccured())
                        {
                            
                            WarningMessage msgVeh1 = calcMethod.CreateWarningMessage(pair.ElementAt(0));
                            WarningMessage msgVeh2 = calcMethod.CreateWarningMessage(pair.ElementAt(1));
                            
                            // If collision occured, check if one or both cars go above speed limit
                            if(collisionPoint != null)
                            {
                                double maxAllowedSpeed = dataStorage.GatherCurvaturesBetweenVehicles().Where(pair => pair.Item1.ContainsKey(collisionPoint.CurrentLocation))
                                                                    .First().Item2;
                                
                                if(pair.ElementAt(0).Speed > maxAllowedSpeed)
                                {
                                    msgVeh1.CollisionSeverity = ICollisionCalculatorImplementation.CollisionSeverity.SEVERE.ToString();
                                }

                                if (pair.ElementAt(1).Speed > maxAllowedSpeed)
                                {
                                    msgVeh2.CollisionSeverity = ICollisionCalculatorImplementation.CollisionSeverity.SEVERE.ToString();
                                }
                            }

                            UdpSocketClientImplementation socket = WebSocketManagerFactory.GetInstance().GetConnection(data.SocketId);



                            socket.SendMessage(AbstractSocket.ConvertMesssageToBytes(msgVeh1));
                            socket.SendMessage(AbstractSocket.ConvertMesssageToBytes(msgVeh2));
                        
                            // Validate this
                            //socket.AddToMessageQueue(msgVeh1.Index ,msgVeh1);
                            //socket.AddToMessageQueue(msgVeh2.Index, msgVeh2);
                        }
                    }
                }
            }

            Console.WriteLine("Elapsed time " + sw.ElapsedMilliseconds);
        }
    }
}
