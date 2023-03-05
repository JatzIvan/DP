using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CoreLibrary.RoadSectionHandling
{
    public static class MapParserUtils
    {

        public static double rEarth = 6371000; // Radius of earth

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

            double h2 = 2 * Math.Atan2(Math.Sqrt(h1), Math.Sqrt(1 - h1));

            return h2 * rEarth;

        }

        public static double CalculateDistanceBetweenPointsFromRadians(LocationPoint a, LocationPoint b)
        {

            double lat1 = a.Latitude;
            double long1 = a.Longitude;
            double lat2 = b.Latitude;
            double long2 = b.Longitude;

            double dlon = long2 - long1;
            double dlat = lat2 - lat1;
            double h1 = Math.Pow(Math.Sin(dlat / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Pow(Math.Sin(dlon / 2), 2);

            double h2 = 2 * Math.Atan2(Math.Sqrt(h1), Math.Sqrt(1 - h1));

            return h2 * rEarth;

        }

        public static double CalculateLocationNorm(LocationPoint a)
        {
            return Math.Sqrt((a.Latitude * a.Latitude) + (a.Longitude * a.Longitude));
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
        public static LocationPoint CalculateDistanceToNearestPointGreatCircle(LocationPoint a, LocationPoint b, LocationPoint c)
        {
            ValueTuple<double, double, double> a_ = ConvertGPStoCartsian(a);
            ValueTuple<double, double, double> b_ = ConvertGPStoCartsian(b);
            ValueTuple<double, double, double> c_ = ConvertGPStoCartsian(c);

            ValueTuple<double, double, double> G = CalculateVectorProduct(a_, b_);
            ValueTuple<double, double, double> F = CalculateVectorProduct(c_, G);
            ValueTuple<double, double, double> t = CalculateVectorProduct(G, F);

            return ConvertFromCartsianToGPS(MultiplyVectorByScalar(NormalizeXYZCoordinates(t), rEarth));
        }


        // source: http://stackoverflow.com/questions/1185408/converting-from-longitude-latitude-to-cartesian-coordinates
        /*
         * return Tuple<x,y,z>
         */
        public static ValueTuple<double, double, double> ConvertGPStoCartsian(LocationPoint coord)
        {
            double x, y, z;
            x = rEarth * Math.Cos(ConvertDegreesToRadians(coord.Latitude)) * Math.Cos(ConvertDegreesToRadians(coord.Longitude));
            y = rEarth * Math.Cos(ConvertDegreesToRadians(coord.Latitude)) * Math.Sin(ConvertDegreesToRadians(coord.Longitude));
            z = rEarth * Math.Sin(ConvertDegreesToRadians(coord.Latitude));
            return (x, y, z);
        }

        private static LocationPoint ConvertFromCartsianToGPS((double x, double y, double z) xyCoords)
        {
            double latitude = ConvertRadiansToDegrees(Math.Asin(xyCoords.z / rEarth));
            double longitude = ConvertRadiansToDegrees(Math.Atan2(xyCoords.y, xyCoords.x));

            return new LocationPoint(longitude, latitude);
        }

        // https://gis.stackexchange.com/questions/340567/formula-to-calculate-the-next-coordinate-given-a-coordinate-distance-direction
        public static LocationPoint CalculatedPointFromPoint(LocationPoint start, double heading, double distance)
        {
            double cLat1 = ConvertDegreesToRadians(start.Latitude);
            double cLon1 = ConvertDegreesToRadians(start.Longitude);

            double lat2 = Math.Asin(Math.Sin(cLat1) * Math.Cos(distance / rEarth) +
                  Math.Cos(cLat1) * Math.Sin(distance / rEarth) * Math.Cos(heading));
            double lon2 = cLon1 + Math.Atan2(Math.Sin(heading) * Math.Sin(distance / rEarth) * Math.Cos(cLat1),
                         Math.Cos(distance / rEarth) - Math.Sin(cLat1) * Math.Sin(lat2));
            return new LocationPoint(ConvertRadiansToDegrees(lon2), ConvertRadiansToDegrees(lat2));

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

        public static double CalculateCrossProduct(Vector2 vect1, Vector2 vect2)
        {
            return vect1.X * vect2.Y - vect2.X * vect1.Y;
        }

        public static double CalculateXYZMagnitude((double x , double y, double z) t)
        {
            return Math.Sqrt((t.x * t.x) + (t.y * t.y) + (t.z * t.z));
        }

        public static ValueTuple<double, double, double> NormalizeXYZCoordinates((double x, double y, double z) t)
        {
            double length = CalculateXYZMagnitude(t);
            double normX, normY, normZ;
            normX = t.x / length;
            normY = t.y / length;
            normZ = t.z / length;
            return (normX, normY, normZ);
        }

        public static ValueTuple<double, double, double> MultiplyVectorByScalar((double x, double y, double z) vector, double k)
        {
            double multX, multY, multZ;
            multX = vector.x * k;
            multY = vector.y * k;
            multZ = vector.z * k;
            return (multX, multY, multZ);
        }

        // http://www.movable-type.co.uk/scripts/latlong.html
        public static double CalculateBearingBetweenPoints(LocationPoint a, LocationPoint b)
        {

            double lat1 = ConvertDegreesToRadians(a.Latitude);
            double long1 = ConvertDegreesToRadians(a.Longitude);
            double lat2 = ConvertDegreesToRadians(b.Latitude);
            double long2 = ConvertDegreesToRadians(b.Longitude);

            double y = Math.Sin(long2 - long1) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) -
                      Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(long2 - long1);
            double θ = Math.Atan2(y, x);
            return (θ * 180 / Math.PI + 360) % 360; // in degrees

        }

        public static KdTree<LocationPoint> CreateKdTreeWithCoordinates(List<LocationPoint> points)
        {
            KdTree<LocationPoint> finalTree = new KdTree<LocationPoint>();

            foreach(LocationPoint point in points)
            {
                finalTree.Insert(new GeoAPI.Geometries.Coordinate(point.Longitude, point.Latitude), point);
            }

            return finalTree;
        }

        public static KdTree<AbstractRoadModel> CreateKdTreeWithCoordinates(List<AbstractRoadModel> points)
        {
            KdTree<AbstractRoadModel> finalTree = new KdTree<AbstractRoadModel>();

            foreach (AbstractRoadModel point in points)
            {
                (double, double, double) converted = ConvertGPStoCartsian(point.CurrentLocation);
                //finalTree.Insert(
                //    new GeoAPI.Geometries.Coordinate(point.CurrentLocation.Longitude, point.CurrentLocation.Latitude), point);
                finalTree.Insert(
                    new GeoAPI.Geometries.Coordinate(converted.Item1, converted.Item2, converted.Item3), point);
            }

            return finalTree;
        }

    }
}
