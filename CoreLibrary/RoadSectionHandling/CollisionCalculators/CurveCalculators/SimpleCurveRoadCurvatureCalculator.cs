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

            (double, double, double) convertedVehicle1 = MapParserUtils.ConvertGPStoCartsian(
    new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat));
            (double, double, double) convertedVehicle2 = MapParserUtils.ConvertGPStoCartsian(
                new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat));

            //AbstractRoadModel v1Point = currectRoadModel.NearestNeighbor(new GeoAPI.Geometries.Coordinate(vehicle1.Position.Lon, vehicle1.Position.Lat)).Data;
            //AbstractRoadModel v2Point = currectRoadModel.NearestNeighbor(new GeoAPI.Geometries.Coordinate(vehicle2.Position.Lon, vehicle2.Position.Lat)).Data;

            AbstractRoadModel v1Point = currectRoadModel.NearestNeighbor(new NetTopologySuite.Geometries.CoordinateZ(convertedVehicle1.Item1, convertedVehicle1.Item2, convertedVehicle1.Item3)).Data;
            AbstractRoadModel v2Point = currectRoadModel.NearestNeighbor(new NetTopologySuite.Geometries.CoordinateZ(convertedVehicle2.Item1, convertedVehicle2.Item2, convertedVehicle2.Item3)).Data;


            /*AbstractRoadModel v1Point = currectRoadModel.NearestNeighbor(new GeoAPI.Geometries.Coordinate(vehicle1.Position.Lon, vehicle1.Position.Lat)).Data;
            AbstractRoadModel v2Point = currectRoadModel.NearestNeighbor(new GeoAPI.Geometries.Coordinate(vehicle2.Position.Lon, vehicle2.Position.Lat)).Data;
            */
            // First calculate road distance between vehicles
            // Take points and vehicle offsets into account
            double distance = AbstractCollisionDetector.GetDistanceBetweenMapPoints(v1Point, v2Point)
                + AbstractCollisionDetector.CalcOffsetBetweenCarAndMapPoint(vehicle1, v1Point) + AbstractCollisionDetector.CalcOffsetBetweenCarAndMapPoint(vehicle2, v2Point);

            TTC = distance / (vehicle1.Speed + vehicle2.Speed);

            if(TTC < 1)
            {
                return null;
            }

            double distanceToCollision = TTC * vehicle1.Speed;

            // Find closest road point to calculated distance and determine if it is in curvature region

            double smallestCumDistance = Double.PositiveInfinity;
            AbstractRoadModel pointWithSmallestCumDistance = null;
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
                    pointWithSmallestCumDistance = v1NextPoint;
                    smallestCumDistance = Math.Abs(currentCumDistance - distanceToCollision);
                }

                v1NextPoint = AbstractCollisionDetector.DetermineDirection(vehicle1.Heading, v1NextPoint) ? v1NextPoint.Next.Point : v1NextPoint.Previous.Point;

            }

            if(distanceToCollision < AbstractCollisionDetector.GetDistanceBetweenMapPoints(pointWithSmallestCumDistance, v1Point))
            {
                CollisionWillHappen = pointWithSmallestCumDistance.Previous.RadiusOfCurvature > ApplicationConfigurationHandler.CurvatureTreshold;
            }
            else
            {
                CollisionWillHappen = pointWithSmallestCumDistance.Next.RadiusOfCurvature > ApplicationConfigurationHandler.CurvatureTreshold;
            }

            if (CollisionWillHappen)
            {

                LocationPoint realCollisionPoint = MapParserUtils.CalculatedPointFromPoint(pointWithSmallestCumDistance.CurrentLocation, ((distanceToCollision < AbstractCollisionDetector.GetDistanceBetweenMapPoints(pointWithSmallestCumDistance, v1Point))
                    ? pointWithSmallestCumDistance.Previous.Heading : pointWithSmallestCumDistance.Next.Heading), Math.Abs(distanceToCollision - AbstractCollisionDetector.GetDistanceBetweenMapPoints(pointWithSmallestCumDistance, v1Point)));

                Console.WriteLine("-------------------------------------------");

                Console.WriteLine("Collision between " + vehicle1.Position.Lat.ToString().Replace(",", ".") + "," + vehicle1.Position.Lon.ToString().Replace(",", ".") + "/" + vehicle1.Heading + "/" + vehicle1.Speed +
                    " and " + vehicle2.Position.Lat.ToString().Replace(",", ".") + "," + vehicle2.Position.Lon.ToString().Replace(",", ".") + "/" + vehicle2.Heading + "/" + vehicle2.Speed
                    + " At Real point " + realCollisionPoint.Latitude.ToString().Replace(",", ".") + "," + realCollisionPoint.Longitude.ToString().Replace(",", ".")
                    + " At Mapped point " + pointWithSmallestCumDistance.CurrentLocation.Latitude.ToString().Replace(",",".") + "," + pointWithSmallestCumDistance.CurrentLocation.Longitude.ToString().Replace(",", "."));

                Console.WriteLine("TTC " + TTC + " Distance to coll " + distanceToCollision + " Distance between points " + AbstractCollisionDetector.GetDistanceBetweenMapPoints(v1NextPoint, v1Point)
                    + " Correct Point heading " + ((distanceToCollision < AbstractCollisionDetector.GetDistanceBetweenMapPoints(pointWithSmallestCumDistance, v1Point))
                    ? pointWithSmallestCumDistance.Previous.Heading + "-Prev" : pointWithSmallestCumDistance.Next.Heading + "-Next"));

                Console.WriteLine("-------------------------------------------");
            }

            return pointWithSmallestCumDistance;

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
