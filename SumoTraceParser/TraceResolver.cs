using ConsoleApp1.Api;
using CoreLibrary;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
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

            ApiHelper.InitializeClient(ApplicationConfigurationHandler.DigitalMapConnection);


            Dictionary<string, List<RoadPointModel>> sections = RoadDataFetcher.GetInstance().GetRoadFromAPIGroupedByRef();

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

            foreach(Tuple<Tuple<string, string>, LocationPoint> pair in VehiclePairs.MeetInDangerousArea)
            {
                output.Add(new Tuple<string, List<string>>("danger_" + pair.Item2.Latitude + "-" + pair.Item2.Longitude + "_", new List<string>() { pair.Item1.Item1, pair.Item1.Item2 }));
            }
            
            foreach (Tuple<Tuple<string, string>, LocationPoint> pair in VehiclePairs.MeetInNonDangerousArea)
            {
                output.Add(new Tuple<string, List<string>>("no_danger" + pair.Item2.Latitude + "-" + pair.Item2.Longitude + "_", new List<string>() { pair.Item1.Item1, pair.Item1.Item2 }));
            }

            return output;
        }

        // We just check if one vehicle has lane with - sign
        private bool GoingAgainstEachOther(Vehicle veh1, Vehicle veh2)
        {
            return ((veh1.Lane.Contains("-") && !veh2.Lane.Contains("-")) || (!veh1.Lane.Contains("-") && veh2.Lane.Contains("-"))
                && veh1.Lane.Replace("-", "").Equals(veh2.Lane.Replace("-", "")));
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

        // Simple method that determines if vehicles are too far apart, not going against eachother or passed eachother
        private bool SkipPair(Vehicle veh1, AbstractRoadModel veh1MappedPoint, Vehicle veh2, AbstractRoadModel veh2MappedPoint)
        {
            if (AbstractCollisionDetector.DetermineDirection(veh1.Angle, veh1MappedPoint) == AbstractCollisionDetector.DetermineDirection(veh2.Angle, veh2MappedPoint))
            {
                return true;
            }

            // Point on road wont be perfectly on the car position, start with the offset distance
            // Calc ofset of both vehicles for better accuracy
            // We need to take into account, if car is closer to second vehicle than point or vice versa
            double distance = CalcOffsetBetweenCarAndMapPoint(veh1, veh1MappedPoint) + CalcOffsetBetweenCarAndMapPoint(veh2, veh2MappedPoint);
            distance += AbstractCollisionDetector.GetDistanceBetweenMapPoints(veh1MappedPoint, veh2MappedPoint);

            // If distance is smaller than treshold, fetch point and determine where they meet
            if (distance > 100)
            {
                return true;
            }

            double checkDistance = CalcOffsetBetweenCarAndMapPoint(veh1, veh1MappedPoint) + CalcOffsetBetweenCarAndMapPoint(veh2, veh2MappedPoint);

            // Loop until vehicle 1 does not meet position of vehicle 2
            // This is to check if cars did not pass eachother
            while (true)
            {
                // We can stop prematurely when distance treshold was exceeded 
                if (veh1MappedPoint.CurrentLocation.Equals(veh2MappedPoint.CurrentLocation))
                {
                    break;
                }

                // We should be able to find the target vehicle in the distance treshold
                if (checkDistance > distance)
                {
                    return true;
                }

                // If this happens that means they already passed eachother
                // This signals that this pair can be skipped
                if ((AbstractCollisionDetector.DetermineDirection(veh1.Angle, veh1MappedPoint) ? veh1MappedPoint.Next.Point : veh1MappedPoint.Previous.Point) == null)
                {
                    return true;
                }

                AbstractRoadModel v1NextPoint = AbstractCollisionDetector.DetermineDirection(veh1.Angle, veh1MappedPoint) ? veh1MappedPoint.Next.Point : veh1MappedPoint.Previous.Point;  
                checkDistance += AbstractCollisionDetector.GetDistanceBetweenMapPoints(v1NextPoint, veh1MappedPoint);
                veh1MappedPoint = v1NextPoint;
            }

            return false;
        }

        private void ResolveVehicleMeetPoint(List<Timestep> timesteps)
        {

            List<string> allVehicles = new List<string>();

            foreach(Timestep step in timesteps)
            {

                allVehicles.AddRange(step.Vehicles.Select(x => x.Id).Where(x => !allVehicles.Contains(x)).ToList());

            }

            List<Tuple<string, string>> pairs = AbstractCollisionDetector.GetPermutations(allVehicles, 2).Select(x => new Tuple<string, string>(x.ElementAt(0), x.ElementAt(1))).ToList();
           Console.WriteLine("Number of pairs - " + pairs.Count());

            Dictionary<Tuple<string, string>, List<Tuple<double, Timestep>>> allDistances = new Dictionary<Tuple<string, string>, List<Tuple<double, Timestep>>>();

            foreach(Tuple<string, string> pair in pairs)
            {
                allDistances.Add(pair, new List<Tuple<double, Timestep>>());
            }

            foreach (Timestep step in timesteps)
            {
                List<string> currentVehicles = step.Vehicles.Select(x => x.Id).ToList();

                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Processing timestep - " + step.Time);

                foreach (KeyValuePair<Tuple<string, string>, List<Tuple<double, Timestep>>> vehicles in allDistances)
                {

                    if (currentVehicles.Contains(vehicles.Key.Item1) && currentVehicles.Contains(vehicles.Key.Item2))
                    {

                        Vehicle veh1 = step.Vehicles.Where(x => x.Id.Equals(vehicles.Key.Item1)).First();
                        Vehicle veh2 = step.Vehicles.Where(x => x.Id.Equals(vehicles.Key.Item2)).First();
                        vehicles.Value.Add(new Tuple<double, Timestep>(MapParserUtils.CalculateDistanceBetweenPoints(new LocationPoint(veh1.X, veh1.Y), new LocationPoint(veh2.X, veh2.Y)), step));
                    }
                }

                Console.WriteLine("---------------------------------------------");

            }

            foreach(KeyValuePair<Tuple<string, string>, List<Tuple<double, Timestep>>> vehiclesWithDistances in allDistances)
            {

                Tuple<double, Timestep> smallestDistance = vehiclesWithDistances.Value.DefaultIfEmpty(new Tuple<double, Timestep>(Double.MaxValue, null)).OrderBy(p => p.Item1).First();

                Console.WriteLine("Smallest distance between vehicles " + vehiclesWithDistances.Key.Item1 + "/" + vehiclesWithDistances.Key.Item2 + " is " + smallestDistance.Item1);

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

                bool dangerZone = (1 / v1Point.RadiusOfCircle) > ApplicationConfigurationHandler.CurvatureTreshold;

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

        public override CustomWrapper CreateVehiclePairs()
        {

            ResolveVehicleMeetPoint(SumoExport.Timestemps);
/*            foreach (Timestep timestep in SumoExport.Timestemps)
            {
                var pairs = CreateVehiclePairs(timestep.Vehicles);

                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Processing timestep - " + timestep.Time);
                Console.WriteLine("Number of pairs - " + pairs.Count());

                foreach (var pair in pairs)
                {

                    Vehicle vehicle1 = pair.ElementAt(0);
                    Vehicle vehicle2 = pair.ElementAt(1);

                    (double, double, double) convertedVehicle1 = MapParserUtils.ConvertGPStoCartsian(
                        new LocationPoint(vehicle1.X, vehicle1.Y));
                    (double, double, double) convertedVehicle2 = MapParserUtils.ConvertGPStoCartsian(
                        new LocationPoint(vehicle2.X, vehicle2.Y));

                    KdTree<AbstractRoadModel> model = RoadHandlers["503"].GetParsedRoadData();

                    AbstractRoadModel v1Point = model.NearestNeighbor(new GeoAPI.Geometries.Coordinate(convertedVehicle1.Item1, convertedVehicle1.Item2, convertedVehicle1.Item3)).Data;
                    AbstractRoadModel v2Point = model.NearestNeighbor(new GeoAPI.Geometries.Coordinate(convertedVehicle2.Item1, convertedVehicle2.Item2, convertedVehicle2.Item3)).Data;


                    // Calculate only if they go against each other

                    if (SkipPair(vehicle1, v1Point, vehicle2, v2Point))
                    {
                        continue;
                    }

                    double distance = CalcOffsetBetweenCarAndMapPoint(veh1, veh1MappedPoint) + CalcOffsetBetweenCarAndMapPoint(veh2, veh2MappedPoint);
                    distance += AbstractCollisionDetector.GetDistanceBetweenMapPoints(veh1MappedPoint, veh2MappedPoint);

                    if(distance > 20)

                    double TTC = distance / (vehicle1.Speed + vehicle2.Speed);

                    double distanceToCollision = TTC * vehicle1.Speed;

                    if(distanceToCollision is double.NaN)
                    {
                        continue;
                    }

                    // Find closest road point to calculated distance and determine if it is in curvature region

                    double smallestCumDistance = Double.PositiveInfinity;
                    AbstractRoadModel pointWithSmallestCumDistance = null;
                    AbstractRoadModel v1Next = AbstractCollisionDetector.DetermineDirection(vehicle1.Angle, v1Point) ? v1Point.Next.Point : v1Point.Previous.Point;

                    while (true)
                    {

                        double currentCumDistance = AbstractCollisionDetector.GetDistanceBetweenMapPoints(v1Next, v1Point);

                        if (smallestCumDistance < Math.Abs(currentCumDistance - distanceToCollision))
                        {
                            break;
                        }
                        else
                        {
                            pointWithSmallestCumDistance = v1Next;
                            smallestCumDistance = Math.Abs(currentCumDistance - distanceToCollision);
                        }

                        v1Next = AbstractCollisionDetector.DetermineDirection(vehicle1.Angle, v1Next) ? v1Next.Next.Point : v1Next.Previous.Point;

                    }

                    bool dangerZone;

                    if (distanceToCollision < AbstractCollisionDetector.GetDistanceBetweenMapPoints(pointWithSmallestCumDistance, v1Point))
                    {
                        dangerZone = pointWithSmallestCumDistance.Previous.RadiusOfCurvature > ApplicationConfigurationHandler.CurvatureTreshold;
                    }
                    else
                    {
                        dangerZone = pointWithSmallestCumDistance.Next.RadiusOfCurvature > ApplicationConfigurationHandler.CurvatureTreshold;
                    }

                    WrapperInstance.ResolvedIdPairs.Add(new Tuple<string, string>(vehicle1.Id, vehicle2.Id));

                    if (dangerZone)
                    {
                        WrapperInstance.MeetInDangerousArea.Add(new Tuple<Tuple<string, string>, LocationPoint>(
                            new Tuple<string, string>(vehicle1.Id, vehicle2.Id), v1Next.CurrentLocation));
                    }
                    else
                    {
                        WrapperInstance.MeetInNonDangerousArea.Add(new Tuple<Tuple<string, string>, LocationPoint>(
                            new Tuple<string, string>(vehicle1.Id, vehicle2.Id), v1Next.CurrentLocation));
                    }

                }

                Console.WriteLine("---------------------------------------------");

            }*/
            return WrapperInstance;
        }
    }

    public class CustomWrapper : PairWrapper
    {

        public List<Tuple<Tuple<string, string>, LocationPoint>> MeetInDangerousArea { get; set; } = new List<Tuple<Tuple<string, string>, LocationPoint>>();

        public List<Tuple<Tuple<string, string>, LocationPoint>> MeetInNonDangerousArea { get; set; } = new List<Tuple<Tuple<string, string>, LocationPoint>>();

    }
}

