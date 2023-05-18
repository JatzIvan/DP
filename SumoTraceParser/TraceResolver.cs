using ConsoleApp1.Api;
using CoreLibrary;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
using NetTopologySuite.Operation.Distance;
using SumoTraceParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketLibrary;

namespace SumoTraceParser
{
    public class TraceResolver : AbstractTraceResolver<CustomWrapper>
    {

        Dictionary<string, RoadDataHandler> RoadHandlers = new Dictionary<string, RoadDataHandler>();

        public TraceResolver(string path) : base(path)
        {
            WrapperInstance = new CustomWrapper();
            // We need road to correctly split vehicles into dangerous and not dangerous meetings

            ApplicationConfigurationHandler.LoadConfiguration();

            ApiHelper.InitializeClient();


            Dictionary<string, List<RoadPointModel>> sections = RoadDataFetcher.GetInstance().GetRoadFromAPIGroupedByAttr();

            foreach (KeyValuePair<string, List<RoadPointModel>> section in sections)
            {

                RoadDataHandler roadHandler = new RoadDataHandler(section.Key, section.Key);
                roadHandler.GetParsedRoadData();
                RoadHandlers.Add(section.Key, roadHandler);

            }

        }

        public override List<Tuple<string, List<string>>> CreateExportPairs(CustomWrapper VehiclePairs)
        {
            List<Tuple<string, List<string>>> output = new List<Tuple<string, List<string>>>();

            foreach (Tuple<Tuple<string, string>, LocationPoint> pair in VehiclePairs.MeetInDangerousArea)
            {
                output.Add(new Tuple<string, List<string>>("danger_" + pair.Item2.Latitude + "-" + pair.Item2.Longitude + "_", new List<string>() { pair.Item1.Item1, pair.Item1.Item2 }));
            }

            foreach (Tuple<Tuple<string, string>, LocationPoint> pair in VehiclePairs.MeetInNonDangerousArea)
            {
                output.Add(new Tuple<string, List<string>>("no_danger" + pair.Item2.Latitude + "-" + pair.Item2.Longitude + "_", new List<string>() { pair.Item1.Item1, pair.Item1.Item2 }));
            }

            return output;
        }

        // Ofset is caused by gaps between road points
        // These gaps can be insignificant or quite large
        private double CalcOffsetBetweenCarAndMapPoint(Vehicle vehicle, AbstractRoadModel point)
        {
            AbstractRoadModel nextPoint = (AbstractCollisionDetector.DetermineDirection(vehicle.Angle, point) ? point.Next.Point : point.Previous.Point);
            double realDistance = MapParserUtils.CalculateDistanceBetweenPoints(new LocationPoint(vehicle.X, vehicle.Y), nextPoint.CurrentLocation);

            double offsetDistance = MapParserUtils.CalculateDistanceBetweenPoints(new LocationPoint(vehicle.X, vehicle.Y), point.CurrentLocation);

            return realDistance > AbstractCollisionDetector.GetDistanceBetweenMapPoints(point, nextPoint) ? offsetDistance : -offsetDistance;
        }

        private void ResolveVehicleMeetPoint(List<Timestep> timesteps)
        {

            List<string> allVehicles = new List<string>();

            foreach(Timestep step in timesteps)
            {

                allVehicles.AddRange(step.Vehicles.Select(x => x.Id).Where(x => !allVehicles.Contains(x)).ToList());

            }

            List<Tuple<string, string>> pairs = AbstractCollisionDetector.GetPermutations(allVehicles, 2).Select(x => new Tuple<string, string>(x.ElementAt(0), x.ElementAt(1))).ToList();
           Logger.GetLogger().WriteLine("Number of pairs - " + pairs.Count());

            Dictionary<Tuple<string, string>, List<Tuple<double, Timestep>>> allDistances = new Dictionary<Tuple<string, string>, List<Tuple<double, Timestep>>>();

            foreach(Tuple<string, string> pair in pairs)
            {
                allDistances.Add(pair, new List<Tuple<double, Timestep>>());
            }

            foreach (Timestep step in timesteps)
            {
                List<string> currentVehicles = step.Vehicles.Select(x => x.Id).ToList();

                Logger.GetLogger().WriteLine("---------------------------------------------");
                Logger.GetLogger().WriteLine("Processing timestep - " + step.Time);

                foreach (KeyValuePair<Tuple<string, string>, List<Tuple<double, Timestep>>> vehicles in allDistances)
                {

                    if (currentVehicles.Contains(vehicles.Key.Item1) && currentVehicles.Contains(vehicles.Key.Item2))
                    {

                        Vehicle veh1 = step.Vehicles.Where(x => x.Id.Equals(vehicles.Key.Item1)).First();
                        Vehicle veh2 = step.Vehicles.Where(x => x.Id.Equals(vehicles.Key.Item2)).First();
                        vehicles.Value.Add(new Tuple<double, Timestep>(MapParserUtils.CalculateDistanceBetweenPoints(new LocationPoint(veh1.X, veh1.Y), new LocationPoint(veh2.X, veh2.Y)), step));
                    }
                }

                Logger.GetLogger().WriteLine("---------------------------------------------");

            }

            foreach(KeyValuePair<Tuple<string, string>, List<Tuple<double, Timestep>>> vehiclesWithDistances in allDistances)
            {

                Tuple<double, Timestep> smallestDistance = vehiclesWithDistances.Value.DefaultIfEmpty(new Tuple<double, Timestep>(Double.MaxValue, null)).OrderBy(p => p.Item1).First();

                Logger.GetLogger().WriteLine("Smallest distance between vehicles " + vehiclesWithDistances.Key.Item1 + "/" + vehiclesWithDistances.Key.Item2 + " is " + smallestDistance.Item1);

                // There is not point if the smallest distance between vehicles is this high
                if(smallestDistance.Item1 > 50)
                {
                    continue;
                }

                Vehicle veh1 = smallestDistance.Item2.Vehicles.Where(x => x.Id.Equals(vehiclesWithDistances.Key.Item1)).First();
                Vehicle veh2 = smallestDistance.Item2.Vehicles.Where(x => x.Id.Equals(vehiclesWithDistances.Key.Item2)).First();

                (double, double, double) convertedVehicle1 = MapParserUtils.ConvertGPStoCartsian(
                        new LocationPoint(veh1.X, veh1.Y));
                (double, double, double) convertedVehicle2 = MapParserUtils.ConvertGPStoCartsian(
                    new LocationPoint(veh2.X, veh2.Y));

                KdTree<AbstractRoadModel> model = RoadHandlers["503"].GetParsedRoadData();

                AbstractRoadModel v1Point = model.NearestNeighbor(new NetTopologySuite.Geometries.CoordinateZ(convertedVehicle1.Item1, convertedVehicle1.Item2, convertedVehicle1.Item3)).Data;
                AbstractRoadModel v2Point = model.NearestNeighbor(new NetTopologySuite.Geometries.CoordinateZ(convertedVehicle2.Item1, convertedVehicle2.Item2, convertedVehicle2.Item3)).Data;
            
                // Skip when vehicles are not going against eachother
                if (AbstractCollisionDetector.DetermineDirection(veh1.Angle, v1Point) == AbstractCollisionDetector.DetermineDirection(veh2.Angle, v2Point))
                {
                    continue;
                }

                bool dangerZone = IsMeetInDangerZone(veh1, veh2, v1Point, v2Point);

                WrapperInstance.ResolvedIdPairs.Add(new Tuple<string, string>(veh1.Id, veh2.Id));

                if (dangerZone)
                {
                    WrapperInstance.MeetInDangerousArea.Add(new Tuple<Tuple<string, string>, LocationPoint>(
                        new Tuple<string, string>(veh1.Id, veh2.Id), v1Point.CurrentLocation));
                }
                else
                {
                    WrapperInstance.MeetInNonDangerousArea.Add(new Tuple<Tuple<string, string>, LocationPoint>(
                        new Tuple<string, string>(veh1.Id, veh2.Id), v1Point.CurrentLocation));
                }
            }

        }

        private bool IsMeetInDangerZone(Vehicle veh1, Vehicle veh2, AbstractRoadModel v1Point, AbstractRoadModel v2Point)
        {
            double v1Offset = CalcOffsetBetweenCarAndMapPoint(veh1, v1Point);
            double v2Offset = CalcOffsetBetweenCarAndMapPoint(veh2, v2Point);
            double distance = AbstractCollisionDetector.GetDistanceBetweenMapPoints(v1Point, v2Point) + v1Offset + v2Offset;

            double TTC = distance / (veh1.Speed + veh1.Speed);

            if (TTC is double.NaN || double.IsInfinity(TTC)) 
            {
                return false;
            }

            double distanceToCollision = TTC * veh1.Speed;

            bool v1Direction = AbstractCollisionDetector.DetermineDirection(veh1.Angle, v1Point);

            double smallestCumDistance = Double.PositiveInfinity;
            AbstractRoadModel pointWithSmallestCumDistance = null;
            AbstractRoadModel v1NextPoint = v1Point;
            //AbstractRoadModel v1NextPoint = v1Direction ? v1Point.Next.Point : v1Point.Previous.Point;

            while (true)
            {

                double currentCumDistance = AbstractCollisionDetector.GetDistanceBetweenMapPoints(v1NextPoint, v1Point) + v1Offset;

                if (smallestCumDistance < Math.Abs(currentCumDistance - distanceToCollision))
                {
                    //Logger.GetLogger().WriteLine("cum distance -- " + smallestCumDistance + " and distance to col - " + distanceToCollision);
                    break;
                }
                else
                {
                    pointWithSmallestCumDistance = v1NextPoint;
                    smallestCumDistance = Math.Abs(currentCumDistance - distanceToCollision);
                }

                v1NextPoint = v1Direction ? v1NextPoint.Next.Point : v1NextPoint.Previous.Point;

            }

            bool CollisionWillHappen;

            if (distanceToCollision < (AbstractCollisionDetector.GetDistanceBetweenMapPoints(pointWithSmallestCumDistance, v1Point) + v1Offset))
            {
                CollisionWillHappen = (v1Direction ? pointWithSmallestCumDistance.Previous.RadiusOfCurvature : pointWithSmallestCumDistance.Next.RadiusOfCurvature) > ApplicationConfigurationHandler.CurvatureTreshold;
            }
            else
            {
                CollisionWillHappen = (v1Direction ? pointWithSmallestCumDistance.Next.RadiusOfCurvature : pointWithSmallestCumDistance.Previous.RadiusOfCurvature) > ApplicationConfigurationHandler.CurvatureTreshold;
            }

            Logger.GetLogger().WriteLine("Coll will happen " + CollisionWillHappen);
            return CollisionWillHappen;

        }

        public override CustomWrapper CreateVehiclePairs()
        {

            ResolveVehicleMeetPoint(SumoExport.Timestemps);

            return WrapperInstance;
        }
    }

    public class CustomWrapper : PairWrapper
    {

        public List<Tuple<Tuple<string, string>, LocationPoint>> MeetInDangerousArea { get; set; } = new List<Tuple<Tuple<string, string>, LocationPoint>>();

        public List<Tuple<Tuple<string, string>, LocationPoint>> MeetInNonDangerousArea { get; set; } = new List<Tuple<Tuple<string, string>, LocationPoint>>();

    }
}

