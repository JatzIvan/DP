using CoreLibrary;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketLibrary;
using WebSocketLibrary.Models;

namespace CollisionDetector
{
    class CustomCollisionDataHandler : AbstractCollisionDetector
    {

        private RoadDataHandler dataStorage;

        public CustomCollisionDataHandler(RoadDataHandler roadHandler) : base(roadHandler.GetParsedRoadData())
        {
            //roadHandler.GatherCurvaturesBetweenVehicles();
            this.dataStorage = roadHandler;
        }

        private bool PushMaxSpeedContent(VehicleData vehicle, NotifyMessage msg, double maxAllowedSpeed)
        {
            if (vehicle.Speed > maxAllowedSpeed)
            {
                //Console.WriteLine("-----------------------------------------");
                Console.WriteLine($"Vehicle {vehicle.Id} speed ({vehicle.Speed}) has exceeded the max possible speed ({maxAllowedSpeed}) to traverse curve");
                //Console.WriteLine("-----------------------------------------");
                msg.Level = NotificationLevel.danger;
                //msg.Content.NotificationMessages.Push($"Vehicle speed ({vehicle.Speed}) has exceeded the max possible speed ({maxAllowedSpeed}) to traverse curve");
                msg.Content.MaxSpeedExceededBy = vehicle.Speed - maxAllowedSpeed;

                return true;
            }

            return false;
        }

        public override void PerformActions(VehicleObserverWrapper data)
        {

            Stopwatch sw = Stopwatch.StartNew();
            List<VehicleData> vehicles = data.Data.Vehicles;
            if (vehicles.Count >= 2)
            {
                List<Tuple<VehicleData, AbstractRoadModel>> mappedVehicles = vehicles
                    .Select(veh => ( veh, MapParserUtils.ConvertGPStoCartsian(new LocationPoint(veh.Position.Lon, veh.Position.Lat)) ) )
                    .Select(convertedVehicle => new Tuple<VehicleData, AbstractRoadModel>(
                        convertedVehicle.veh, currectRoadModel.NearestNeighbor(new NetTopologySuite.Geometries.CoordinateZ(convertedVehicle.Item2.Item1, convertedVehicle.Item2.Item2, convertedVehicle.Item2.Item3)).Data
                    )).ToList();

                IEnumerable<IEnumerable<Tuple<VehicleData, AbstractRoadModel>>> pairsToCalc = CreateVehiclePairs(mappedVehicles);
                ParallelOptions options = new ParallelOptions
                                {
                                    MaxDegreeOfParallelism = ApplicationConfigurationHandler.MaxParallelism is null ? -1 : int.Parse(ApplicationConfigurationHandler.MaxParallelism)
                                };

                //foreach (var pair in pairsToCalc)

                Parallel.ForEach(pairsToCalc, options, pair =>
                {

                    ICollisionCalculatorImplementation calcMethod = ResolveCollisionCalculatorBasedOnCurvature(pair.ElementAt(0), pair.ElementAt(1));
                    if (calcMethod != null)
                    {
                        Console.WriteLine("--------------------Start of collision warning handling------------------------");
                        AbstractRoadModel collisionPoint = calcMethod.PerformCollisionCalculations(pair.ElementAt(0), pair.ElementAt(1), currectRoadModel);

                        if (calcMethod.CollisionOccured())
                        {

                            NotifyMessage msgVeh1 = calcMethod.CreateNotificationMessage(pair.ElementAt(0).Item1, pair.ElementAt(1).Item1, collisionPoint, dataStorage.SectionRef);
                            NotifyMessage msgVeh2 = calcMethod.CreateNotificationMessage(pair.ElementAt(1).Item1, pair.ElementAt(0).Item1, collisionPoint, dataStorage.SectionRef);

                            // If collision occured, check if one or both cars go above speed limit
/*                            if (collisionPoint != null)
                            {

                                double maxAllowedSpeed = collisionPoint.MaxSpeed;

                                if (PushMaxSpeedContent(pair.ElementAt(0), msgVeh1, maxAllowedSpeed))
                                {
                                    calcMethod.IsVehicleAbleToBrake(pair.ElementAt(0), dataStorage.SectionRef, msgVeh1);
                                }

                                if (PushMaxSpeedContent(pair.ElementAt(1), msgVeh2, maxAllowedSpeed))
                                {
                                    calcMethod.IsVehicleAbleToBrake(pair.ElementAt(1), dataStorage.SectionRef, msgVeh2);
                                }

                            }*/

                            AbstractSocket socket = WebSocketManagerFactory.GetInstance().GetConnection(data.SocketId);

                            socket.SendMessage(msgVeh1);
                            socket.SendMessage(msgVeh2);

                        }
                        Console.WriteLine("--------------------End of collision warning handling------------------------");
                    }
                });
            }

            if (sw.ElapsedMilliseconds > 100)
            {
                Console.WriteLine("Elapsed time " + sw.ElapsedMilliseconds + " for number of cars " + vehicles.Count);
            }
        }
    }
}
