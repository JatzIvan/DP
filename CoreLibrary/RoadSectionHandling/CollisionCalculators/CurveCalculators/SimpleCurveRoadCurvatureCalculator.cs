using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;
using WebSocketLibrary;
using System.Linq;
using NetTopologySuite.Index.KdTree;
using NetTopologySuite.Operation.Distance;

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

        // The most simple possible solution is to calculate distance by road
        // I need to determine meeting point (simple distance / speed1 + speed2). When meeting point is in curve change CollisionWillHappen to true
        public AbstractRoadModel PerformCollisionCalculations(Tuple<VehicleData, AbstractRoadModel> vehicle1Tuple, Tuple<VehicleData, AbstractRoadModel> vehicle2Tuple,
            KdTree<AbstractRoadModel> currectRoadModel)
        {

            /*            (double, double, double) convertedVehicle1 = MapParserUtils.ConvertGPStoCartsian(
                new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat));
                        (double, double, double) convertedVehicle2 = MapParserUtils.ConvertGPStoCartsian(
                            new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat));*/

            /*AbstractRoadModel v1Point = currectRoadModel.NearestNeighbor(new NetTopologySuite.Geometries.CoordinateZ(convertedVehicle1.Item1, convertedVehicle1.Item2, convertedVehicle1.Item3)).Data;
            AbstractRoadModel v2Point = currectRoadModel.NearestNeighbor(new NetTopologySuite.Geometries.CoordinateZ(convertedVehicle2.Item1, convertedVehicle2.Item2, convertedVehicle2.Item3)).Data;*/

            AbstractRoadModel v1Point = vehicle1Tuple.Item2;
            AbstractRoadModel v2Point = vehicle2Tuple.Item2;

            VehicleData vehicle1 = vehicle1Tuple.Item1;
            VehicleData vehicle2 = vehicle2Tuple.Item1;

            // First calculate road distance between vehicles
            // Take points and vehicle offsets into account

            double v1Offset = AbstractCollisionDetector.CalcOffsetBetweenCarAndMapPoint(vehicle1, v1Point);
            double v2Offset = AbstractCollisionDetector.CalcOffsetBetweenCarAndMapPoint(vehicle2, v2Point);
            double distance = AbstractCollisionDetector.GetDistanceBetweenMapPoints(v1Point, v2Point) + v1Offset + v2Offset;

            TTC = distance / (vehicle1.Speed + vehicle2.Speed);

            if(TTC < 1 || TTC is double.PositiveInfinity)
            {
                return null;
            }

            double distanceToCollision = TTC * vehicle1.Speed;

            // Find closest road point to calculated distance and determine if it is in curvature region

            bool v1Direction = AbstractCollisionDetector.DetermineDirection(vehicle1.Heading, v1Point);

            double smallestCumDistance = Double.PositiveInfinity;
            AbstractRoadModel pointWithSmallestCumDistance = null;
            AbstractRoadModel v1NextPoint = v1Point;
            //AbstractRoadModel v1NextPoint = v1Direction ? v1Point.Next.Point : v1Point.Previous.Point;

            while (true)
            {

                double currentCumDistance = AbstractCollisionDetector.GetDistanceBetweenMapPoints(v1NextPoint, v1Point) + v1Offset;
                
                if(smallestCumDistance < Math.Abs(currentCumDistance - distanceToCollision))
                {
                    //Logger.GetLogger().WriteLine("cum distance -- " + smallestCumDistance + " and distance to col - " + distanceToCollision);
                    break;
                }
                else
                {
                    pointWithSmallestCumDistance = v1NextPoint;
                    smallestCumDistance = Math.Abs(currentCumDistance - distanceToCollision);
                }

                if(v1NextPoint.Next == null || v1NextPoint.Previous == null)
                {
                    return null;
                }

                v1NextPoint = v1Direction ? v1NextPoint.Next.Point : v1NextPoint.Previous.Point;

            }

            if (distanceToCollision < (AbstractCollisionDetector.GetDistanceBetweenMapPoints(pointWithSmallestCumDistance, v1Point) + v1Offset))
            {
                //Logger.GetLogger().WriteLine((v1Direction ? "Previous" : "Next") + " with radius of curv " + (v1Direction ? pointWithSmallestCumDistance.Previous.RadiusOfCurvature : pointWithSmallestCumDistance.Next.RadiusOfCurvature));
                CollisionWillHappen = (v1Direction ? pointWithSmallestCumDistance.Previous.RadiusOfCurvature : pointWithSmallestCumDistance.Next.RadiusOfCurvature) > ApplicationConfigurationHandler.CurvatureTreshold;
            }
            else
            {
                //Logger.GetLogger().WriteLine((v1Direction ? "Next" : "Previous") + " with radius of curv " + (v1Direction ? pointWithSmallestCumDistance.Next.RadiusOfCurvature : pointWithSmallestCumDistance.Previous.RadiusOfCurvature));
                CollisionWillHappen = (v1Direction ? pointWithSmallestCumDistance.Next.RadiusOfCurvature : pointWithSmallestCumDistance.Previous.RadiusOfCurvature) > ApplicationConfigurationHandler.CurvatureTreshold;
            }


            if (CollisionWillHappen)
            {

                Logger.GetLogger().WriteLine($"Vehicle {vehicle1.Id}-({vehicle1.Position.Lat.ToString().Replace(",", ".")},{vehicle1.Position.Lon.ToString().Replace(",", ".")}/{vehicle1.Heading}/{vehicle1.Speed}) " +
                    $"and {vehicle2.Id}-({vehicle2.Position.Lat.ToString().Replace(",", ".")},{vehicle2.Position.Lon.ToString().Replace(",", ".")}/{vehicle2.Heading}/{vehicle2.Speed}) will meet in dangerous area");
                Logger.GetLogger().WriteLine("They will meet at mapped point - " + pointWithSmallestCumDistance.CurrentLocation.Latitude.ToString().Replace(",", ".") + "," + pointWithSmallestCumDistance.CurrentLocation.Longitude.ToString().Replace(",", "."));
                //Logger.GetLogger().WriteLine($"TTC - {TTC} at approximated meet point - " + realCollisionPoint.Latitude.ToString().Replace(",", ".") + "," + realCollisionPoint.Longitude.ToString().Replace(",", "."));
                Logger.GetLogger().WriteLine("TTC - " + TTC);

            }

            return pointWithSmallestCumDistance;

        }
    }
}
