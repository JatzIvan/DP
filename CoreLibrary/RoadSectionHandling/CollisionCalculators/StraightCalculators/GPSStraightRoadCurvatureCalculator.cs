using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators
{
    [CollisionType(CollisionTypeEnum.STRAIGHT)]
    public class GPSStraightRoadCurvatureCalculator : ICollisionCalculatorImplementation
    {

        // Earth equatorial circumference (in m)
        private float L = 40075016;

        private double PI = 3.1415926;

        // Radius of Earth (in m)
        private float R = 6378137;

        //Each degree latitude changes, Longitude keeps same, distance will change
        private float LatL = 111319;

        public double CalculateTTC()
        {
            throw new NotImplementedException();
        }

        public bool CollisionOccured()
        {
            throw new NotImplementedException();
        }

        public ICollisionCalculatorImplementation.CollisionSeverity GetCollisionSeverity()
        {
            throw new NotImplementedException();
        }

        /**
         * This calculation uses one car as host (vehicle 1) and one as target (vehicle 2)
         * To correctly calculate collision we need to have lat, long, speed and heading
         * 
         * TODO: I do not think how well will this work in our application
         * TODO: find out when collision can be discarded
         */
        public AbstractRoadModel PerformCollisionCalculations(VehicleData vehicle1, VehicleData vehicle2, KdTree<AbstractRoadModel> currectRoadModel)
        {

            // Geographic position angle of target vehicle relative to host vehicle

            double angleOfTarget = Math.Atan((Math.Cos(vehicle2.Position.Lat) * (vehicle2.Position.Lon - vehicle1.Position.Lon))
                / (vehicle2.Position.Lat - vehicle1.Position.Lat));

            // Target vehicle orientation
            double alpha = (((angleOfTarget - vehicle1.Heading) + 360) % 360);
            //double alpha = angleOfTarget - vehicle1.Heading;

            // Heading deference -- angle between vehicle headings
            // TODO: This is the most likely contender to determine if collision occured

            // This will ensure, that difference between 2 headings will be between 0-360
            double beta = (((vehicle2.Heading - vehicle1.Heading) + 360) % 360);
            //double beta = vehicle2.Heading - vehicle1.Heading;


            double betaBasedOnHeading;

            if (beta < 90)
            {
                betaBasedOnHeading = beta;
            } else if (beta < 180)
            {
                betaBasedOnHeading = 180 - beta;
            } else if (beta < 270)
            {
                betaBasedOnHeading = beta - 180;
            } else
            {
                betaBasedOnHeading = 360 - beta;
            }

            //Tuple<double, double> vehicleDimensionsH = GetVehicleLengthAndWidthBasedOnType(vehicle1.Type);
            //Tuple<double, double> vehicleDimensionsT = GetVehicleLengthAndWidthBasedOnType(vehicle2.Type);

            Tuple<double, double> vehicleDimensionsH = new Tuple<double, double>(5.1,2.1);
            Tuple<double, double> vehicleDimensionsT = new Tuple<double, double>(4.8, 1.9);

            //Distance between front part of car and intersection point
            double Xt = ((vehicleDimensionsT.Item2 / 2) * Math.Cos(betaBasedOnHeading) + vehicleDimensionsH.Item2 / 2) / Math.Sin(betaBasedOnHeading);
            double Xh = ((vehicleDimensionsH.Item2 / 2) * Math.Cos(betaBasedOnHeading) + vehicleDimensionsT.Item2 / 2) / Math.Sin(betaBasedOnHeading);

            double Xst = vehicleDimensionsT.Item2 / Math.Tan(betaBasedOnHeading);
            double Xsh = vehicleDimensionsH.Item2 / Math.Tan(betaBasedOnHeading);

            // Calculate distance between vehicles

            double deltaLat = vehicle2.Position.Lat - vehicle1.Position.Lat;
            double deltaLon = vehicle2.Position.Lon - vehicle1.Position.Lon;

            //double Xht = MapParserUtils.CalculateDistanceBetweenPoints(new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat), new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat)) * 1000;
            double Xht = 2 * R * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(deltaLat / 2), 2) + Math.Cos(vehicle1.Position.Lat) * Math.Cos(vehicle2.Position.Lat) * Math.Pow(Math.Sin(deltaLon / 2), 2)));


            //Distance between "Collision point" and host

            double Xch = Xht * (Math.Cos(alpha) - Math.Sin(alpha) / Math.Tan(beta));

            //Distance between "Collision point" and target

            double Xct = -Xht * Math.Sin(alpha) / Math.Sin(beta);

            List<double> Th = CalculateKeyTimes(vehicle1, Xch, Xh, Xsh, vehicleDimensionsH);
            List<double> Tt = CalculateKeyTimes(vehicle2, Xct, Xt, Xst, vehicleDimensionsT);
            
            Dictionary<CollisionCase, double> TTCs = ReturnAllPossibleTTCs(vehicle1, vehicle2, beta, betaBasedOnHeading, Tt, Th, vehicleDimensionsH, vehicleDimensionsT);

            // TODO: Think this over
            return null;
        }

        // This method is used for testing purposes only (we should get precise vehicle info later)
        // return Length,Width
        private Tuple<double, double> GetVehicleLengthAndWidthBasedOnType(VehicleType type)
        {
            switch (type)
            {
                case VehicleType.car:
                    return new Tuple<double, double>(4.5d, 2d);
                default:
                    return null;
            }
        }

        // Determines, which vehicle part will be hit
        private Dictionary<CollisionCase, double> ReturnAllPossibleTTCs(VehicleData vehicleH, VehicleData vehicleT, double beta, double betaBasedOnHeading, List<double> Tt, List<double> Th, Tuple<double, double> vehicleDimensionsH, Tuple<double, double> vehicleDimensionsT)
        {
            Dictionary<CollisionCase, double> dict = new Dictionary<CollisionCase, double>();

            if(beta > 90 && beta < 270)
            {
                dict.Add(CollisionCase.TC1, Tt[0]);
                dict.Add(CollisionCase.TC2, Th[0]);
                dict.Add(CollisionCase.TC3, (vehicleDimensionsT.Item2/Math.Sin(betaBasedOnHeading) + Th[1]*vehicleH.Speed + (Tt[0]*vehicleT.Speed)/Math.Cos(betaBasedOnHeading))/(vehicleT.Speed/Math.Cos(betaBasedOnHeading) + vehicleH.Speed));
                dict.Add(CollisionCase.TC4, (vehicleDimensionsH.Item2 / Math.Sin(betaBasedOnHeading) + Tt[1] * vehicleT.Speed + (Th[0] * vehicleH.Speed) / Math.Cos(betaBasedOnHeading)) / (vehicleH.Speed / Math.Cos(betaBasedOnHeading) + vehicleT.Speed));
            }
            else
            {
                dict.Add(CollisionCase.TC5, Tt[0]);
                dict.Add(CollisionCase.TC6, (Th[4]*vehicleH.Speed - Tt[1]*vehicleT.Speed*Math.Cos(betaBasedOnHeading))/(vehicleH.Speed - vehicleT.Speed*Math.Cos(betaBasedOnHeading)));
                dict.Add(CollisionCase.TC7, (Th[0] * vehicleH.Speed - Tt[3] * vehicleT.Speed * Math.Cos(betaBasedOnHeading)) / (vehicleH.Speed - vehicleT.Speed * Math.Cos(betaBasedOnHeading)));
                dict.Add(CollisionCase.TC8, (Th[3]*vehicleH.Speed - (Tt[0]*vehicleT.Speed)/(Math.Cos(betaBasedOnHeading)))/(vehicleH.Speed - vehicleT.Speed / Math.Cos(betaBasedOnHeading)));
                dict.Add(CollisionCase.TC9, Th[0]);
                dict.Add(CollisionCase.TC10, (Th[1] * vehicleH.Speed - (Tt[4] * vehicleT.Speed) / (Math.Cos(betaBasedOnHeading))) / (vehicleH.Speed - vehicleT.Speed / Math.Cos(betaBasedOnHeading)));
            }

            return dict;
        }

        // Calculate Key time frames for vehicle
        // Xc - distance from middle of car to collision point, X - distance of front part to intersection point, Xs - edges of vehicles
        private List<double> CalculateKeyTimes(VehicleData basicVehicleData, double Xc, double X, double Xs, Tuple<double, double> vehicleDimensions)
        {
            List<double> times = new List<double>();
            times.Add((Xc - X - vehicleDimensions.Item1 / 2) / basicVehicleData.Speed);
            times.Add((Xc - X + Xs - vehicleDimensions.Item1/2) / basicVehicleData.Speed);
            times.Add((Xc + X - vehicleDimensions.Item1/2) / basicVehicleData.Speed);
            times.Add((Xc - X + vehicleDimensions.Item1/2) / basicVehicleData.Speed);
            times.Add((Xc + X - Xs + vehicleDimensions.Item1/2) / basicVehicleData.Speed);
            times.Add((Xc + X + vehicleDimensions.Item1/2) / basicVehicleData.Speed);
            return times;
        }

    }

    public enum CollisionCase
    {
        TC1,
        TC2,
        TC3,
        TC4,
        TC5,
        TC6,
        TC7,
        TC8,
        TC9,
        TC10
    }

}
