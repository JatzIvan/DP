using ConsoleApp2.RoadSectionHandling.CollisionCalculators;
using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators
{
    class GPSStraightRoadCurvatureCalculator : ICollisionCalculatorImplementation
    {

        // Earth equatorial circumference (in m)
        private float L = 40075016;

        private double PI = 3.1415926;

        // Radius of Earth (in m)
        private float R = 6378137;

        //Each degree latitude changes, Longitude keeps same, distance will change
        private float LatL = 111319; 

        public float CalculateTTC()
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
        public CollisionInfo PerformCollisionCalculations(VehicleData vehicle1, VehicleData vehicle2, Dictionary<LocationPoint, AbstractRoadModel> currectRoadModel)
        {

            // Geographic position angle of target vehicle relative to host vehicle
            double angleOfTarget = Math.Atan((Math.Cos(vehicle2.Position.Lat) * (vehicle2.Position.Lon - vehicle1.Position.Lon))
                / (vehicle2.Position.Lat - vehicle1.Position.Lat));

            // Target vehicle orientation
            double alpha = angleOfTarget - vehicle1.Heading;

            // Heading deference -- angle between vehicle headings
            // TODO: Should be between 0 and 360 -- if not modify -- not sure how
            // TODO: This is the most likely contender to determine if collision occured
            double beta = vehicle2.Heading - vehicle1.Heading;


            // Calculate distance between vehicles

            double deltaLat = vehicle2.Position.Lat - vehicle1.Position.Lat;
            double deltaLon = vehicle2.Position.Lon - vehicle1.Position.Lon;

            double Xht = Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(deltaLat / 2), 2) + Math.Cos(vehicle1.Position.Lat) * Math.Cos(vehicle2.Position.Lat) * Math.Pow(Math.Sin(deltaLon / 2), 2)));

            //Distance between "Collision point" and host

            double Xch = Xht * (Math.Cos(alpha) - Math.Sin(alpha) / Math.Tan(beta));

            //Distance between "Collision point" and target

            double Xct = - Xht * Math.Sin(alpha) / Math.Sin(beta);

            // TODO: Think this over
            return null;
        }

    }
}
