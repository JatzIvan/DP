using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using WebSocketLibrary.Models;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators
{
    [CollisionType(CollisionTypeEnum.STRAIGHT)]
    class SimpleTTCCalculator : ICollisionCalculatorImplementation
    {

        // Radius of Earth (in m)
        private float R = 6378137;

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

        // Longitude X
        // Latitude Y
        // https://www.movable-type.co.uk/scripts/latlong.html
        public AbstractRoadModel PerformCollisionCalculations(VehicleData vehicle1, VehicleData vehicle2, KdTree<AbstractRoadModel> currectRoadModel)
        {

            //double d = Math.Sqrt(Math.Pow((vehicle1.Position.Lon - vehicle2.Position.Lon), 2) + Math.Pow(vehicle1.Position.Lat - vehicle2.Position.Lat, 2));

            /*double d = MapParserUtils.CalculateDistanceBetweenPoints(new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat), new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat));

            double xInter = vehicle1.Position.Lon + (vehicle1.Speed / (vehicle1.Speed - vehicle2.Speed)) * d * Math.Cos(vehicle1.Heading);

            double yInter = vehicle1.Position.Lat + (vehicle1.Speed / (vehicle1.Speed - vehicle2.Speed)) * d * Math.Sin(vehicle1.Heading);

            double xInter2 = ((vehicle2.Position.Lat - vehicle1.Position.Lat) - (vehicle2.Position.Lon * Math.Tan(vehicle2.Heading)
            - vehicle1.Position.Lon * Math.Tan(vehicle1.Heading))) / (Math.Tan(vehicle1.Heading) - Math.Tan(vehicle2.Heading));

            double yInter2 = ((vehicle2.Position.Lon - vehicle1.Position.Lon) - (vehicle2.Position.Lat * (Math.Cos(vehicle2.Heading) / Math.Sin(vehicle2.Heading))
            - vehicle1.Position.Lat * (Math.Cos(vehicle1.Heading) / Math.Sin(vehicle1.Heading)))) / ((Math.Cos(vehicle1.Heading)/ Math.Sin(vehicle1.Heading)) - (Math.Cos(vehicle2.Heading) / Math.Sin(vehicle2.Heading)));
            
            */
            //double lat = asin(z / R)
            //double lon = atan2(y, x)

            //Convert to radian
            double lon1 = MapParserUtils.ConvertDegreesToRadians(vehicle1.Position.Lon);
            double lat1 = MapParserUtils.ConvertDegreesToRadians(vehicle1.Position.Lat);
            double b1 = MapParserUtils.ConvertDegreesToRadians(vehicle1.Heading);

            double lon2 = MapParserUtils.ConvertDegreesToRadians(vehicle2.Position.Lon);
            double lat2 = MapParserUtils.ConvertDegreesToRadians(vehicle2.Position.Lat);
            double b2 = MapParserUtils.ConvertDegreesToRadians(vehicle2.Heading);

            double dlon = lon2 - lon1;
            double dlat = lat2 - lat1;

            // Great-circle distance between point 1 and point 2
            double haversine = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dlon / 2), 2);

            //angular distance between point 1 and point 2
            double ang_dist_1_2 = 2 * Math.Asin(Math.Sqrt(haversine));

            //Initial and final bearings between point 1 and point 2
            double initial_bearing = Math.Acos((Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(ang_dist_1_2)) / (Math.Sin(ang_dist_1_2) * Math.Cos(lat1)));
            double final_bearing = Math.Acos((Math.Sin(lat1) - Math.Sin(lat2) * Math.Cos(ang_dist_1_2)) / (Math.Sin(ang_dist_1_2) * Math.Cos(lat2)));

            double bearing_1_2;
            double bearing_2_1;

            //Adjust the bearings on the trigonometric circle
            if (Math.Sin(vehicle2.Position.Lon - vehicle1.Position.Lon) > 0) {
                bearing_1_2 = initial_bearing;
                bearing_2_1 = (2 * Math.PI) - final_bearing;
            }
            else {
                bearing_1_2 = (2 * Math.PI) - initial_bearing;
                bearing_2_1 = final_bearing;
            }


            //Angles between different points
            double ang_1 = b1 - bearing_1_2;    //angle p2<--p1-->p3
            double ang_2 = bearing_2_1 - b2;    //angle p1<--p2-->p3
            double ang_3 = Math.Acos(-Math.Cos(ang_1) * Math.Cos(ang_2) + Math.Sin(ang_1) * Math.Sin(ang_2) * Math.Cos(ang_dist_1_2));    // angle p1<--p3-->p2

            if(Math.Sin(ang_1) * Math.Sin(ang_2) < 0)
            {
                Console.WriteLine("Skipping");
                return null;
            }

            // angular distance between point 1 and intersection point (point 3)
            double ang_dist_1_3 = Math.Atan2(Math.Sin(ang_dist_1_2) * Math.Sin(ang_1) * Math.Sin(ang_2), Math.Cos(ang_2) + Math.Cos(ang_1) * Math.Cos(ang_3));

            //Latitude of point 3
            double lat3 = Math.Asin(Math.Sin(lat1) * Math.Cos(ang_dist_1_3) + Math.Cos(lat1) * Math.Sin(ang_dist_1_3) * Math.Cos(b1));

            //Longitude of point 3
            double delta_long_1_3 = Math.Atan2(Math.Sin(b1) * Math.Sin(ang_dist_1_3) * Math.Cos(lat1), Math.Cos(ang_dist_1_3) - Math.Sin(lat1) * Math.Sin(lat3));
            double lon3 = lon1 + delta_long_1_3;

            double TTX1 = MapParserUtils.CalculateDistanceBetweenPointsFromRadians(new LocationPoint(lon3, lat3), new LocationPoint(lon1, lat1)) / vehicle1.Speed;
            double TTX2 = MapParserUtils.CalculateDistanceBetweenPointsFromRadians(new LocationPoint(lon3, lat3), new LocationPoint(lon2, lat2)) / vehicle2.Speed;

            Console.WriteLine(MapParserUtils.ConvertRadiansToDegrees(lat3) + " " + MapParserUtils.ConvertRadiansToDegrees(lon3));

            if(Math.Abs(TTX1 - TTX2) <= 5)
            {
                Console.WriteLine("They should meet");
                CollisionWillHappen = true;
                TTC = (TTX1 < TTX2 ? TTX1 : TTX2);
            }
            else
            {
                Console.WriteLine("They wont probs meet");
            }

            return null;
            /*double xInter = ((vehicle2PosCartesian.Item2 - vehicle1PosCartesian.Item2) - (vehicle2PosCartesian.Item1 * Math.Tan(vehicle2.Heading)
                - vehicle1PosCartesian.Item1 * Math.Tan(vehicle1.Heading))) / (Math.Tan(vehicle1.Heading) - Math.Tan(vehicle2.Heading));

            double yInter = ((vehicle2PosCartesian.Item1 - vehicle1PosCartesian.Item1) - (vehicle2PosCartesian.Item2 * (1/Math.Tan(vehicle2.Heading))
                - vehicle1PosCartesian.Item1 * (1 / Math.Tan(vehicle1.Heading)))) / ((1 / Math.Tan(vehicle1.Heading)) - (1 / Math.Tan(vehicle2.Heading)));
            */
        }
    }
}
