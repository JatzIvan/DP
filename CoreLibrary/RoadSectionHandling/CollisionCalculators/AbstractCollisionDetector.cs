using CoreLibrary;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using CoreLibrary.RoadSectionHandling.CollisionCalculators.CurveCalculators;
using CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators;
using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    public abstract class AbstractCollisionDetector : IMessageHandler<VehicleObserverWrapper>
    {
        protected KdTree<AbstractRoadModel> currectRoadModel;

        public AbstractCollisionDetector(KdTree<AbstractRoadModel> currectRoadModel)
        {
            this.currectRoadModel = currectRoadModel;
        }

        /**
         * Simple utility to determine in which direction is our car heading
         * This is a long shot but the only possible way is to compare heading of car and road (closer value means the correct direction)
         * return true -> forward / false -> back
         */
        public static bool DetermineDirection(double heading, AbstractRoadModel carPoint)
        {

            if(carPoint.Previous == null)
            {
                return true;
            }

            if(carPoint.Next == null)
            {
                return false;
            }

            //return Math.Abs(carPoint.Next.Heading - heading) < Math.Abs(carPoint.Previous.Heading - heading);

            return CalcClosestHeading(carPoint.Next.Heading, heading) < CalcClosestHeading(carPoint.Previous.Heading, heading);

            //return heading >= 180 && heading < 360;
        }

        // Take into account 360/0 transformation
        private static double CalcClosestHeading(double heading1, double heading2)
        {

            double transformedHeading1 = heading1 < 30 && heading2 > 270 ? heading1 + 360 : heading1;
            double transformedHeading2 = heading2 < 30 && heading1 > 270 ? heading2 + 360 : heading2;

            double originalSub = Math.Abs(heading1 - heading2);
            double transformedSub = Math.Abs(transformedHeading1 - transformedHeading2);

            return transformedSub < originalSub ? transformedSub : originalSub;

        }

        // true -> forward / false -> back
        // Calculates distance using cumulative distance
        public static double GetDistanceBetweenMapPoints(AbstractRoadModel point1, AbstractRoadModel point2)
        {
            return Math.Abs(point1.Distance - point2.Distance);
        }

        // Ofset is caused by gaps between road points
        // These gaps can be insignificant or quite large
        public static double CalcOffsetBetweenCarAndMapPoint(VehicleData vehicle, AbstractRoadModel point)
        {
            AbstractRoadModel nextPoint = (DetermineDirection(vehicle.Heading, point) ? point.Next.Point : point.Previous.Point);
            double realDistance = MapParserUtils.CalculateDistanceBetweenPoints(new LocationPoint(vehicle.Position.Lon, vehicle.Position.Lat), nextPoint.CurrentLocation);

            double offsetDistance = MapParserUtils.CalculateDistanceBetweenPoints(new LocationPoint(vehicle.Position.Lon, vehicle.Position.Lat), point.CurrentLocation);

            return realDistance > GetDistanceBetweenMapPoints(point, nextPoint) ? offsetDistance : - offsetDistance;
        }

        /**
         * function is used to determine if we skip collision calculations based on distance
         * If distance between cars is greater than {constant} in config, do not calculate collision to save time
         */
        // TODO: discuss this. This can cause much slowdown (probably less that doing all collision calculations). Find a better solution (if doable)
        private bool SkipCalculations(VehicleData vehicle1, VehicleData vehicle2)
        {


            // First transform datawrapper to LocationPoints
            // Need to find the closest road segment and then approximate
            // TODO: figure out the best possible way
            //AbstractRoadModel v1Point = currectRoadModel[findClosestRoadLocationPoint(currectRoadModel.Keys.ToList(), new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat), 0, currectRoadModel.Count, currectRoadModel.Keys.First())];
            //AbstractRoadModel v2Point = currectRoadModel[findClosestRoadLocationPoint(currectRoadModel.Keys.ToList(), new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat), 0, currectRoadModel.Count, currectRoadModel.Keys.First())];

            (double, double, double) convertedVehicle1 = MapParserUtils.ConvertGPStoCartsian(
                new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat));
            (double, double, double) convertedVehicle2 = MapParserUtils.ConvertGPStoCartsian(
                new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat));

            //AbstractRoadModel v1Point = currectRoadModel.NearestNeighbor(new GeoAPI.Geometries.Coordinate(vehicle1.Position.Lon, vehicle1.Position.Lat)).Data;
            //AbstractRoadModel v2Point = currectRoadModel.NearestNeighbor(new GeoAPI.Geometries.Coordinate(vehicle2.Position.Lon, vehicle2.Position.Lat)).Data;

            AbstractRoadModel v1Point = currectRoadModel.NearestNeighbor(new NetTopologySuite.Geometries.CoordinateZ(convertedVehicle1.Item1, convertedVehicle1.Item2, convertedVehicle1.Item3)).Data;
            AbstractRoadModel v2Point = currectRoadModel.NearestNeighbor(new NetTopologySuite.Geometries.CoordinateZ(convertedVehicle2.Item1, convertedVehicle2.Item2, convertedVehicle2.Item3)).Data;


            // Calculate only if they go against each other
            if (DetermineDirection(vehicle1.Heading, v1Point) == DetermineDirection(vehicle2.Heading, v2Point))
            {
                //Console.WriteLine("Skip: Wrong direction");
                return true;
            }

            // Point on road wont be perfectly on the car position, start with the offset distance
            // Calc ofset of both vehicles for better accuracy
            // We need to take into account, if car is closer to second vehicle than point or vice versa
            double distance = CalcOffsetBetweenCarAndMapPoint(vehicle1, v1Point) + CalcOffsetBetweenCarAndMapPoint(vehicle2, v2Point);
            distance += GetDistanceBetweenMapPoints(v2Point, v1Point);
            if(distance > ApplicationConfigurationHandler.CarDistanceSkipTreshold || distance < 10)
            {
                //Console.WriteLine("Skip: Distance");
                return true;
            }

            double checkDistance = CalcOffsetBetweenCarAndMapPoint(vehicle1, v1Point) + CalcOffsetBetweenCarAndMapPoint(vehicle2, v2Point);

            bool direction = DetermineDirection(vehicle1.Heading, v1Point);

            // Loop until vehicle 1 does not meet position of vehicle 2
            // This is to check if cars did not pass eachother
            while (true)
            {
                // We can stop prematurely when distance treshold was exceeded 
               if (v1Point.CurrentLocation.Equals(v2Point.CurrentLocation))
                {
                    break;
                }
             
               // We should be able to find the target vehicle in the distance treshold
                if(checkDistance > distance)
                {
                    //Console.WriteLine("Skip: Could not find in said distance passed eachother");
                    return true;
                }

                // If this happens that means they already passed eachother
                // This signals that this pair can be skipped
                if ((direction ? v1Point.Next.Point : v1Point.Previous.Point) == null)
                {
                    //Console.WriteLine("Skip: Could not find in said distance passed eachother");
                    return true;
                }

                //Console.WriteLine("Direction: " + DetermineDirection(vehicle1.Heading, v1Point) + " Heading: " + vehicle1.Heading + "-" + v1Point.Next.Heading + "/" + v1Point.Previous.Heading);

               AbstractRoadModel v1NextPoint = direction ? v1Point.Next.Point : v1Point.Previous.Point;
            // Calculate Distance between found points (fast and simple)   
               checkDistance += GetDistanceBetweenMapPoints(v1NextPoint, v1Point);
               v1Point = v1NextPoint;
                    //MapParserUtils.CalculateDistanceBetweenPoints(v1NextPoint.CurrentLocation, v1Point.CurrentLocation);

            }

            // TODO figure out the best value (preferably setup value in config)
            //return distance > ApplicationConfigurationHandler.CarDistanceSkipTreshold;
            return false;
        }

        /**
         * TODO Check if List in dictionary is really sorted (if not, sort it)
         * This method utilizes binary search (we assume that list should be sorted)
         * TODO test this !!!!
         */
        /*public static LocationPoint findClosestRoadLocationPoint(List<LocationPoint> points, LocationPoint vehiclePosition, int min, int max, LocationPoint bestVal)
        {

            if (min > max)
            {
                return bestVal;
            }
            else
            {
                int mid = (min + max) / 2;

                LocationPoint chosenVal = points[mid];

                if (MapParserUtils.CalculateDistanceBetweenPoints(chosenVal, vehiclePosition) < MapParserUtils.CalculateDistanceBetweenPoints(bestVal, vehiclePosition))
                {
                    bestVal = chosenVal;
                }

                // Check which way to go
                if (MapParserUtils.CalculateDistanceBetweenPoints(chosenVal, vehiclePosition) > MapParserUtils.CalculateDistanceBetweenPoints(points[mid + 1], vehiclePosition))
                {
                    return findClosestRoadLocationPoint(points, vehiclePosition, mid + 1, max, bestVal);
                }
                else
                {
                    return findClosestRoadLocationPoint(points, vehiclePosition, min, mid - 1, bestVal);
                }
            }
        }*/

        /**
         * Determine if road contains curves
         * The most naive approach would be to check if any road segment contains curve at least once
         * TODO figure out more effective approach
         */
        private bool UseSpecialCurvatureCalculations(VehicleData vehicle1, VehicleData vehicle2)
        {
            // First transform datawrapper to LocationPoints
            // Need to find the closest road segment and then approximate
            // TODO: figure out the best possible way

            (double, double, double) convertedVehicle1 = MapParserUtils.ConvertGPStoCartsian(
    new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat));
            (double, double, double) convertedVehicle2 = MapParserUtils.ConvertGPStoCartsian(
                new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat));

            //AbstractRoadModel v1Point = currectRoadModel.NearestNeighbor(new GeoAPI.Geometries.Coordinate(vehicle1.Position.Lon, vehicle1.Position.Lat)).Data;
            //AbstractRoadModel v2Point = currectRoadModel.NearestNeighbor(new GeoAPI.Geometries.Coordinate(vehicle2.Position.Lon, vehicle2.Position.Lat)).Data;

            AbstractRoadModel v1Point = currectRoadModel.NearestNeighbor(new NetTopologySuite.Geometries.CoordinateZ(convertedVehicle1.Item1, convertedVehicle1.Item2, convertedVehicle1.Item3)).Data;
            AbstractRoadModel v2Point = currectRoadModel.NearestNeighbor(new NetTopologySuite.Geometries.CoordinateZ(convertedVehicle2.Item1, convertedVehicle2.Item2, convertedVehicle2.Item3)).Data;

            //AbstractRoadModel v1Point = currectRoadModel[findClosestRoadLocationPoint(currectRoadModel.Keys.ToList(), new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat), 0, currectRoadModel.Count, currectRoadModel.Keys.First())];
            //AbstractRoadModel v2Point = currectRoadModel[findClosestRoadLocationPoint(currectRoadModel.Keys.ToList(), new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat), 0, currectRoadModel.Count, currectRoadModel.Keys.First())];
            //AbstractRoadModel v1Point = currectRoadModel[new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat)];
            //AbstractRoadModel v2Point = currectRoadModel[new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat)];

            // Loop until vehicle 1 does not meet position of vehicle 2
            while (true)
            {
                //RadiusOfCurvature
                // TODO: use value from config
                // TODO: test and determine best radius for method 
                if (DetermineDirection(vehicle1.Heading, v1Point) ? v1Point.Next.RadiusOfCurvature > ApplicationConfigurationHandler.CurvatureTreshold : v1Point.Previous.RadiusOfCurvature > ApplicationConfigurationHandler.CurvatureTreshold)
                {
                    return true;
                }

                if (v1Point.CurrentLocation.Equals(v2Point.CurrentLocation))
                {
                    break;
                }

                v1Point = DetermineDirection(vehicle1.Heading, v1Point) ? v1Point.Next.Point : v1Point.Previous.Point;
            }

            return false;
        }

        /**
         * Simple method pairs cars with eachother for later
         */
        public IEnumerable<IEnumerable<VehicleData>> CreateVehiclePairs(List<VehicleData> vehicles)
        {

            List<ValueTuple<VehicleData, VehicleData>> pairs = new List<ValueTuple<VehicleData, VehicleData>>();
            return GetPermutations(vehicles, 2);

            /*foreach (VehicleData vehicle1 in vehicles)
            {
                foreach(VehicleData vehicle2 in vehicles)
                {
                    if(vehicle1.Id != vehicle2.Id)
                    {
                        pairs.Add((vehicle1, vehicle2));
                    }
                }
            }

            return pairs;*/

        }

        // https://stackoverflow.com/questions/12249051/unique-combinations-of-list
        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (count == 1)
                    yield return new T[] { item };
                else
                {
                    foreach (var result in GetPermutations(items.Skip(i + 1), count - 1))
                        yield return new T[] { item }.Concat(result);
                }

                ++i;
            }
        }

        /**
         * Method determines which type of calculator to use
         * Currectly we distinguish between straight and curve
         */
        public ICollisionCalculatorImplementation ResolveCollisionCalculatorBasedOnCurvature(VehicleData vehicle1, VehicleData vehicle2)
        {

            // Now check if they are not too far away
            if(!SkipCalculations(vehicle1, vehicle2))
            {
                // TODO: for now we will just find and resolve parts of road with curvature. This implementation is prepared
                // to distinguish between "straight"/curve parts

                //return UseSpecialCurvatureCalculations(vehicle1, vehicle2) ? CurveCollisionCalculatorFactory.GetInstance().GetImplementation()
                //    : null;

                return CurveCollisionCalculatorFactory.GetInstance().GetImplementation();

                //return UseSpecialCurvatureCalculations(vehicle1, vehicle2) ? CurveCollisionCalculatorFactory.GetInstance().GetImplementation()
                //    : StraightCollisionCalculatorFactory.GetInstance().GetImplementation();
            }

            return null;
        }

        public abstract void PerformActions(VehicleObserverWrapper data);
    }
}
