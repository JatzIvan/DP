using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketLibrary.Models;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.CurveCalculators
{
    [CollisionType(CollisionTypeEnum.CURVATURE)]
    class AdvancedCurveRoadCollisionCalculator : ICollisionCalculatorImplementation
    {

        private bool CollisionWillHappen = false;
        private double TTC;
        private ICollisionCalculatorImplementation.CollisionSeverity Severity;

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


        public AbstractRoadModel PerformCollisionCalculations(VehicleData vehicle1, VehicleData vehicle2, KdTree<AbstractRoadModel> currectRoadModel)
        {

            double Vb = vehicle2.Speed;
            double Va = vehicle1.Speed;
            double alpha;
            double delta;
            double Dab = MapParserUtils.CalculateDistanceBetweenPoints(new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat), 
                new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat));
            double totalDistance = 0;
            int i = 0;
            LocationPoint TPn = new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat);
            List<LocationPoint> TP = new List<LocationPoint>();
            TP.Add(TPn);
            float vehicleCB = vehicle2.Heading;
            float vehicleCA = vehicle1.Heading;

            do
            {

                /*Vb += vehicle2.Acceleration;
                Va += vehicle1.Acceleration;
                i++;
                delta = -Va + Vb + vehicle2.Acceleration;

                alpha = (Math.Acos(Math.Pow(delta, 2) + Math.Pow(Dab, 2) - Math.Pow(Dab, 2) * delta)) / (2 * delta * Dab);
                TPi = Dab * Math.Sin(alpha);
                Console.WriteLine(); */
                vehicleCB -= vehicle2.SteeringAngle;
                vehicleCA -= vehicle1.SteeringAngle;

                vehicleCA = (vehicleCA + 360) % 360;
                vehicleCB = (vehicleCB + 360) % 360;

                LocationPoint VPBi = getVPn(vehicleCB, vehicle2.Speed, TPn);
                LocationPoint TPnnew = getTPn(vehicleCA, vehicle1.Speed, VPBi);
                totalDistance += MapParserUtils.CalculateDistanceBetweenPoints(TPnnew, TPn);
                TP.Add(TPnnew);
                TPn = TPnnew;

                if(Dab > totalDistance)
                {
                    i++;
                }

            } while (Dab > totalDistance);

            LocationPoint CPA = MapParserUtils.CalculateDistanceToNearestPointGreatCircle(TP[TP.Count -1 ], TP[TP.Count - 2], 
                new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat));

            double DCPA = MapParserUtils.CalculateDistanceBetweenPoints(new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat), CPA) * 1000;

            double TCPA = (MapParserUtils.CalculateDistanceBetweenPoints(CPA, TP[TP.Count - 2]) /
                MapParserUtils.CalculateDistanceBetweenPoints(TP[TP.Count - 1], TP[TP.Count - 2])) + i - 1; 

            // Need to discuss and define Warning distance/ttc
            if(DCPA <= 10 && TCPA <= 6)
            {
                Console.WriteLine("Collision is possible");
                CollisionWillHappen = true;
                TTC = (float) TCPA;

            }

            return null;
        }

        private LocationPoint getVPn(float heading, float Speed, LocationPoint originalLocation)
        {
            LocationPoint newLocation;

            if(heading >= 0 && heading < 90)
            {
                newLocation = new LocationPoint(
                    originalLocation.Longitude + Math.Sin(heading) * Speed,
                    originalLocation.Latitude + Math.Cos(heading) * Speed
                );
            }
            else if(heading < 180)
            {
                newLocation = new LocationPoint(
                    originalLocation.Longitude + Math.Sin(heading) * Speed,
                    originalLocation.Latitude - Math.Cos(heading) * Speed
                );
            }
            else if(heading < 270)
            {
                newLocation = new LocationPoint(
                    originalLocation.Longitude - Math.Sin(heading) * Speed,
                    originalLocation.Latitude - Math.Cos(heading) * Speed
                );
            }
            else
            {
                newLocation = new LocationPoint(
                    originalLocation.Longitude - Math.Sin(heading) * Speed,
                    originalLocation.Latitude + Math.Cos(heading) * Speed
                );
            }

            return newLocation;
        }

        private LocationPoint getTPn(float heading, float Speed, LocationPoint originalLocation)
        {
            LocationPoint newLocation;

            if (heading >= 0 && heading < 90)
            {
                newLocation = new LocationPoint(
                    originalLocation.Longitude - Math.Sin(heading) * Speed,
                    originalLocation.Latitude - Math.Cos(heading) * Speed
                );
            }
            else if (heading < 180)
            {
                newLocation = new LocationPoint(
                    originalLocation.Longitude - Math.Sin(heading) * Speed,
                    originalLocation.Latitude + Math.Cos(heading) * Speed
                );
            }
            else if (heading < 270)
            {
                newLocation = new LocationPoint(
                    originalLocation.Longitude + Math.Sin(heading) * Speed,
                    originalLocation.Latitude + Math.Cos(heading) * Speed
                );
            }
            else
            {
                newLocation = new LocationPoint(
                    originalLocation.Longitude + Math.Sin(heading) * Speed,
                    originalLocation.Latitude - Math.Cos(heading) * Speed
                );
            }

            return newLocation;
        }

    }
}
