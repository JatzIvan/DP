using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;
using WebSocketLibrary;
using System.Linq;
using NetTopologySuite.Index.KdTree;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.CurveCalculators
{

    // This simple implementations just determines if 2 vehicles meet at the same point in curvature parts of road
    [CollisionType(CollisionTypeEnum.CURVATURE)]
    class SimpleCurveRoadCurvatureCalculator : ICollisionCalculatorImplementation
    {

        private bool CollisionWillHappen = false;
        private double TTC;
        private ICollisionCalculatorImplementation.CollisionSeverity Severity = ICollisionCalculatorImplementation.CollisionSeverity.MEDIUM;

        public double CalculateTTC()
        {
            return TTC;
        }

        public bool CollisionOccured()
        {
            return CollisionWillHappen;
        }

        public ICollisionCalculatorImplementation.CollisionSeverity GetCollisionSeverity()
        {
            return Severity;
        }

        // The most simple possible solution is to calculate distance by road and then with some margin of error compare
        // I need to determine 
        public AbstractRoadModel PerformCollisionCalculations(VehicleData vehicle1, VehicleData vehicle2, KdTree<AbstractRoadModel> currectRoadModel)
        {
            /*List<(LocationPoint, LocationPoint)> curvatures = GatherCurvaturesBetweenVehicles(vehicle1, vehicle2, currectRoadModel);

            foreach ((LocationPoint, LocationPoint) curveBoundaries in curvatures)
            {

            }

            return null;*/

            //AbstractRoadModel v1Point = currectRoadModel[AbstractCollisionDetector.findClosestRoadLocationPoint(currectRoadModel.Keys.ToList(), new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat), 0, currectRoadModel.Count, currectRoadModel.Keys.First())];
            //AbstractRoadModel v2Point = currectRoadModel[AbstractCollisionDetector.findClosestRoadLocationPoint(currectRoadModel.Keys.ToList(), new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat), 0, currectRoadModel.Count, currectRoadModel.Keys.First())];
            AbstractRoadModel v1Point = currectRoadModel.NearestNeighbor(new GeoAPI.Geometries.Coordinate(vehicle1.Position.Lon, vehicle1.Position.Lat)).Data;
            AbstractRoadModel v2Point = currectRoadModel.NearestNeighbor(new GeoAPI.Geometries.Coordinate(vehicle2.Position.Lon, vehicle2.Position.Lat)).Data;

            // First calculate road distance between vehicles
            // Take points and vehicle offsets into account
            double distance = AbstractCollisionDetector.GetDistanceBetweenMapPoints(v1Point, v2Point)
                + AbstractCollisionDetector.CalcOffsetBetweenCarAndMapPoint(vehicle1, v1Point) + AbstractCollisionDetector.CalcOffsetBetweenCarAndMapPoint(vehicle2, v2Point);

            TTC = distance / (vehicle1.Speed + vehicle2.Speed);

            double distanceToCollision = TTC * vehicle1.Speed;

            // Find closest road point to calculated distance and determine if it is in curvature region

            double smallestCumDistance = Double.PositiveInfinity;
            AbstractRoadModel v1NextPoint = AbstractCollisionDetector.DetermineDirection(vehicle1.Heading, v1Point) ? v1Point.Next.Point : v1Point.Previous.Point;

            while (true)
            {

                double currentCumDistance = AbstractCollisionDetector.GetDistanceBetweenMapPoints(v1NextPoint, v1Point);
                
                if(smallestCumDistance < Math.Abs(currentCumDistance - distanceToCollision))
                {
                    break;
                }
                else
                {
                    smallestCumDistance = Math.Abs(currentCumDistance - distanceToCollision);
                }

                v1NextPoint = AbstractCollisionDetector.DetermineDirection(vehicle1.Heading, v1NextPoint) ? v1NextPoint.Next.Point : v1NextPoint.Previous.Point;

            }

            if(distanceToCollision < AbstractCollisionDetector.GetDistanceBetweenMapPoints(v1NextPoint, v1Point))
            {
                CollisionWillHappen = v1NextPoint.Previous.RadiusOfCurvature > ApplicationConfigurationHandler.CurvatureTreshold;
            }
            else
            {
                CollisionWillHappen = v1NextPoint.Next.RadiusOfCurvature > ApplicationConfigurationHandler.CurvatureTreshold;
            }

            if (CollisionWillHappen)
            {
                Console.WriteLine("Collision between " + vehicle1.Id + " and " + vehicle2.Id + " At point " + v1NextPoint.CurrentLocation.Longitude + "/" + v1NextPoint.CurrentLocation.Latitude);
            }

            return v1NextPoint;

        }

/*        public List<ValueTuple<LocationPoint, LocationPoint>> GatherCurvaturesBetweenVehicles(VehicleData vehicle1, VehicleData vehicle2, Dictionary<LocationPoint, AbstractRoadModel> currectRoadModel)
        {
            List<ValueTuple<LocationPoint, LocationPoint>> pairs = new List<ValueTuple<LocationPoint, LocationPoint>>();

            // First transform datawrapper to LocationPoints
            // Need to find the closest road segment and then approximate

            AbstractRoadModel v1Point = currectRoadModel[AbstractCollisionDetector.findClosestRoadLocationPoint(currectRoadModel.Keys.ToList(), new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat), 0, currectRoadModel.Count, currectRoadModel.Keys.First())];
            AbstractRoadModel v2Point = currectRoadModel[AbstractCollisionDetector.findClosestRoadLocationPoint(currectRoadModel.Keys.ToList(), new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat), 0, currectRoadModel.Count, currectRoadModel.Keys.First())];

            // Loop until vehicle 1 does not meet position of vehicle 2

            bool isInCurve = false;
            LocationPoint firstPointOfCurve = null;
            while (true)
            {
                // Start of curve
                if (AbstractCollisionDetector.DetermineDirection(vehicle1.Heading, v1Point) ? (1 / v1Point.Next.RadiusOfCurvature) < ApplicationConfigurationHandler.CurvatureTreshold : (1 / v1Point.Previous.RadiusOfCurvature) < ApplicationConfigurationHandler.CurvatureTreshold)
                {
                    if (!isInCurve)
                    {
                        firstPointOfCurve = v1Point.CurrentLocation;
                    }
                    isInCurve = true;
                }
                else
                {
                    if (isInCurve)
                    {
                        pairs.Add((firstPointOfCurve, v1Point.CurrentLocation));
                    }
                    isInCurve = false;
                }

                if (v1Point.CurrentLocation.Equals(v2Point.CurrentLocation))
                {
                    break;
                }

                v1Point = AbstractCollisionDetector.DetermineDirection(vehicle1.Heading, v1Point) ? v1Point.Next.Point : v1Point.Previous.Point;
            }

            return pairs;
        }*/

    }
}
