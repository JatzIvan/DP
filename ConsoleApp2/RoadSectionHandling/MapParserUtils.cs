using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling
{
    public static class MapParserUtils
    {

        public static double rEarth = 6371; // Radius of earth

        public static double ConvertDegreesToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }

        public static double ConvertRadiansToDegrees(double val)
        {
            return (180 / Math.PI) * val;
        }

        // https://stackoverflow.com/questions/27928/calculate-distance-between-two-latitude-longitude-points-haversine-formula
        public static double CalculateDistanceBetweenPoints(LocationPoint a, LocationPoint b)
        {



            double lat1 = ConvertDegreesToRadians(a.Latitude);
            double long1 = ConvertDegreesToRadians(a.Longitude);
            double lat2 = ConvertDegreesToRadians(b.Latitude);
            double long2 = ConvertDegreesToRadians(b.Longitude);

            double dlon = long2 - long1;
            double dlat = lat2 - lat1;
            double h1 = Math.Pow(Math.Sin(dlat / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Pow(Math.Sin(dlon / 2), 2);

            double h2 = 2 * Math.Asin(Math.Sqrt(h1));

            return h2 * rEarth;

        }


        /**
         *  Analyze deeper 
         */
        // Distance between a point and a line
        public static double CalcShortestDistancePointToLine(LocationPoint a, LocationPoint b, LocationPoint c)
        {

            LocationPoint nearestNode = CalculateDistanceToNearestPointGreatCircle(a, b, c);
            double result = CalculateDistanceBetweenPoints(c, nearestNode);

            return result;
        }

        // source: http://stackoverflow.com/questions/1299567/how-to-calculate-distance-from-a-point-to-a-line-segment-on-a-sphere
        private static LocationPoint CalculateDistanceToNearestPointGreatCircle(LocationPoint a, LocationPoint b, LocationPoint c)
        {
            ValueTuple<double, double, double> a_ = ConvertGPStoCartsian(a);
            ValueTuple<double, double, double> b_ = ConvertGPStoCartsian(b);
            ValueTuple<double, double, double> c_ = ConvertGPStoCartsian(c);

            ValueTuple<double, double, double> G = CalculateVectorProduct(a_, b_);
            ValueTuple<double, double, double> F = CalculateVectorProduct(c_, G);
            ValueTuple<double, double, double> t = CalculateVectorProduct(G, F);

            return ConvertFromCartsianToGPS(MultiplyVectorByScalar(NormalizeXYZCoordinates(t), 6371));
        }


        // source: http://stackoverflow.com/questions/1185408/converting-from-longitude-latitude-to-cartesian-coordinates
        /*
         * return Tuple<x,y,z>
         */
        private static ValueTuple<double, double, double> ConvertGPStoCartsian(LocationPoint coord)
        {
            double x, y, z;
            x = rEarth * Math.Cos(ConvertDegreesToRadians(coord.Latitude)) * Math.Cos(ConvertDegreesToRadians(coord.Longitude));
            y = rEarth * Math.Cos(ConvertDegreesToRadians(coord.Latitude)) * Math.Sin(ConvertDegreesToRadians(coord.Longitude));
            z = rEarth * Math.Sin(ConvertDegreesToRadians(coord.Latitude));
            return (x,y,z);
        }

        private static LocationPoint ConvertFromCartsianToGPS((double x, double y, double z) xyCoords)
        {
            double latitude = ConvertRadiansToDegrees(Math.Asin(xyCoords.z / rEarth));
            double longitude = ConvertRadiansToDegrees(Math.Atan2(xyCoords.y, xyCoords.x));

            return new LocationPoint(longitude, latitude);
        }


        // Basic functions
        /*
        * return ValueTuple<x,y,z>
        */
        private static ValueTuple<double, double, double> CalculateVectorProduct((double x, double y, double z) a, (double x, double y, double z) b)
        {
            //double[] result = new double[3];
            double x, y, z;
            x = a.y * b.z - a.z * b.y;
            y = a.z * b.x - a.x * b.z;
            z = a.x * b.y - a.y * b.x;

            return (x, y, z);
        }

        private static ValueTuple<double, double, double> NormalizeXYZCoordinates((double x, double y, double z) t)
        {
            double length = Math.Sqrt((t.x * t.x) + (t.y * t.y) + (t.z * t.z));
            double normX, normY, normZ;
            normX = t.x / length;
            normY = t.y / length;
            normZ = t.z / length;
            return (normX, normY, normZ);
        }

        private static ValueTuple<double, double, double> MultiplyVectorByScalar((double x, double y, double z) vector, double k)
        {
            double multX, multY, multZ;
            multX = vector.x * k;
            multY = vector.y * k;
            multZ = vector.z * k;
            return (multX, multY, multZ);
        }

    }
}
