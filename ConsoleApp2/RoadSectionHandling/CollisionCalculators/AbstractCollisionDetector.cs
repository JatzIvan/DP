using ConsoleApp2;
using ConsoleApp2.RoadSectionHandling;
using ConsoleApp2.RoadSectionHandling.CollisionCalculators;
using ConsoleApp2.RoadSectionHandling.CollisionCalculators.CurveCalculators;
using ConsoleApp2.RoadSectionHandling.CollisionCalculators.StraightCalculators;
using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    abstract class AbstractCollisionDetector : ICollisionDetector
    {
        Dictionary<LocationPoint, AbstractRoadModel> currectRoadModel;

        public AbstractCollisionDetector(Dictionary<LocationPoint, AbstractRoadModel> currectRoadModel)
        {
            this.currectRoadModel = currectRoadModel;
        }

        /**
         * Simple utility to determine in which direction is our car heading
         * This is a long shot but the only possible way is to compare heading of car and road (closer value means the correct direction)
         * return true -> forward / false -> back
         */
        private bool DetermineDirection(float heading, AbstractRoadModel carPoint)
        {

            return Math.Abs(carPoint.Next.Heading - heading) < Math.Abs(carPoint.Previous.Heading - heading);

            //return heading >= 180 && heading < 360;
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
            AbstractRoadModel v1Point = currectRoadModel[findClosestRoadLocationPoint(currectRoadModel.Keys.ToList(), new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat), 0, currectRoadModel.Count, currectRoadModel.Keys.First())];
            AbstractRoadModel v2Point = currectRoadModel[findClosestRoadLocationPoint(currectRoadModel.Keys.ToList(), new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat), 0, currectRoadModel.Count, currectRoadModel.Keys.First())];

            // Calculate only if they go against each other
            if (DetermineDirection(vehicle1.Heading, v1Point) == DetermineDirection(vehicle2.Heading, v2Point))
            {
                return true;
            }

            double distance = 0;


            // Loop until vehicle 1 does not meet position of vehicle 2
            while (true)
            {

                // We can stop prematurely when distance treshold was exceeded 
                if (v1Point.CurrentLocation.Equals(v2Point.CurrentLocation) || distance > ApplicationConfigurationHandler.CarDistanceSkipTreshold)
                {
                    break;
                }

                // If this happens that means they already passed eachother
                // This signals that this pair can be skipped
                if((DetermineDirection(vehicle1.Heading, v1Point) ? v1Point.Next.Point : v1Point.Previous.Point) == null)
                {
                    return true;
                }

                AbstractRoadModel v1NextPoint = DetermineDirection(vehicle1.Heading, v1Point) ? v1Point.Next.Point : v1Point.Previous.Point;
                distance += MapParserUtils.CalculateDistanceBetweenPoints(v1NextPoint.CurrentLocation, v1Point.CurrentLocation);

            }

            // TODO figure out the best value (preferably setup value in config)
            return distance <= ApplicationConfigurationHandler.CarDistanceSkipTreshold;
        }

        /**
         * TODO Check if List in dictionary is really sorted (if not, sort it)
         * This method utilizes binary search (we assume that list should be sorted)
         * TODO test this !!!!
         */
        private LocationPoint findClosestRoadLocationPoint(List<LocationPoint> points, LocationPoint vehiclePosition, int min, int max, LocationPoint bestVal)
        {

            if(min > max)
            {
                return bestVal;
            }
            else
            {
                int mid = (min + max) / 2;

                LocationPoint chosenVal = points[mid];

                if(MapParserUtils.CalculateDistanceBetweenPoints(chosenVal, vehiclePosition) < MapParserUtils.CalculateDistanceBetweenPoints(bestVal, vehiclePosition))
                {
                    bestVal = chosenVal;
                }

                // Check which way to go
                if(MapParserUtils.CalculateDistanceBetweenPoints(chosenVal, vehiclePosition) > MapParserUtils.CalculateDistanceBetweenPoints(points[mid + 1], vehiclePosition))
                {
                    return findClosestRoadLocationPoint(points, vehiclePosition, mid + 1, max, bestVal);
                }
                else
                {
                    return findClosestRoadLocationPoint(points, vehiclePosition, min, mid - 1, bestVal);
                }
            }
        }

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

            AbstractRoadModel v1Point = currectRoadModel[findClosestRoadLocationPoint(currectRoadModel.Keys.ToList(), new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat), 0, currectRoadModel.Count, currectRoadModel.Keys.First())];
            AbstractRoadModel v2Point = currectRoadModel[findClosestRoadLocationPoint(currectRoadModel.Keys.ToList(), new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat), 0, currectRoadModel.Count, currectRoadModel.Keys.First())];
            //AbstractRoadModel v1Point = currectRoadModel[new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat)];
            //AbstractRoadModel v2Point = currectRoadModel[new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat)];

            // Loop until vehicle 1 does not meet position of vehicle 2
            while (true)
            {
                //RadiusOfCurvature
                // TODO: use value from config
                // TODO: test and determine best radius for method 
                if (DetermineDirection(vehicle1.Heading, v1Point) ? v1Point.Next.RadiusOfCurvature > 0.05 : v1Point.Previous.RadiusOfCurvature > 0.05)
                {
                    return true;
                }

                if (v1Point.CurrentLocation.Equals(v2Point.CurrentLocation))
                {
                    break;
                }

                AbstractRoadModel v1NextPoint = DetermineDirection(vehicle1.Heading, v1Point) ? v1Point.Next.Point : v1Point.Previous.Point;
            }

            return false;
        }

        /**
         * Simple method pairs cars with eachother for later
         */
        public List<ValueTuple<VehicleData, VehicleData>> CreateVehiclePairs(List<VehicleData> vehicles)
        {

            List<ValueTuple<VehicleData, VehicleData>> pairs = new List<ValueTuple<VehicleData, VehicleData>>();

            foreach (VehicleData vehicle1 in vehicles)
            {
                foreach(VehicleData vehicle2 in vehicles)
                {
                    if(vehicle1.Id != vehicle2.Id)
                    {
                        pairs.Add((vehicle1, vehicle2));
                    }
                }
            }

            return pairs;

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
                return UseSpecialCurvatureCalculations(vehicle1, vehicle2) ? CurveCollisionCalculatorFactory.GetInstance().GetImplementation()
                    : StraightCollisionCalculatorFactory.GetInstance().GetImplementation();
            }

            return null;
        }
        public abstract void PerformCalculations(List<CarUpdateInfo> data);
    }
}
