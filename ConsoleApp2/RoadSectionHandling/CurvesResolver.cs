using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling
{
    public class CurvesResolver
    {

        private Dictionary<LocationPoint, RoadCurvitureModel> ConnectedWays { get; set; }

        public CurvesResolver(Dictionary<LocationPoint, RoadCurvitureModel> connectedWays)
        {
            this.ConnectedWays = connectedWays;
        }

        public Dictionary<LocationPoint, RoadCurvitureModel> CalculateCurvesForWays()
        {


            // Calculate both ways

            // From first to last

            RoadCurvitureModel current = GetModel(true);
            
            // Calculate Circle Radius
            while (true)
            {
                RoadCurvitureModel before = current.Previous != null ? current.Previous.Point : null;
                RoadCurvitureModel after = current.Next != null ? current.Next.Point : null;

                if(after == null)
                {
                    break;
                }

                if(before != null)
                {
                    current.RadiusOfCircle = getCircleRadiusFromPoints(before.CurrentLocation, current.CurrentLocation, after.CurrentLocation);
                }
                //double circleRadius = getCircleRadiusFromPoints(before.CurrentLocation, current.CurrentLocation, after.CurrentLocation);
                current = current.Next.Point;

            }

            // Calculate curviture 
            current = GetModel(true);
            while (true)
            {
                RoadCurvitureModel after = current.Next != null ? current.Next.Point : null;

                if (after == null)
                {
                    break;
                }

                current.Next.RadiusOfCurvature = current.RadiusOfCircle > 0 ? (current.RadiusOfCircle + current.Next.Point.RadiusOfCircle) / 2 : current.Next.Point.RadiusOfCircle;

                current = current.Next.Point;

            }

            current = GetModel(false);
            while (true)
            {
                RoadCurvitureModel before = current.Previous != null ? current.Previous.Point : null;

                if (before == null)
                {
                    break;
                }

                current.Previous.RadiusOfCurvature = current.RadiusOfCircle > 0  ? (current.RadiusOfCircle + current.Previous.Point.RadiusOfCircle) / 2 : current.Previous.Point.RadiusOfCircle;

                current = current.Previous.Point;

            }

            return ConnectedWays;

        }

        private double ConvertToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }

        // https://stackoverflow.com/questions/27928/calculate-distance-between-two-latitude-longitude-points-haversine-formula
        private double CalculateDistanceBetweenPoints(LocationPoint a, LocationPoint b)
        {

            double rEarth = 6371; // Radius of earth
            
            double lat1 = ConvertToRadians(a.Latitude);
            double long1 = ConvertToRadians(a.Longitude);
            double lat2 = ConvertToRadians(b.Latitude);
            double long2 = ConvertToRadians(b.Longitude);

            double dlon = long2 - long1;
            double dlat = lat2 - lat1;
            double h1 = Math.Pow(Math.Sin(dlat / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Pow(Math.Sin(dlon / 2), 2);

            double h2 = 2 * Math.Asin(Math.Sqrt(h1));

            return h2 * rEarth;

        }

        private RoadCurvitureModel GetModel(bool first)
        {

            RoadCurvitureModel currentModel = ConnectedWays.First().Value;

            while (true)
            {
            
                if ((first && currentModel.Previous != null) || (!first && currentModel.Next !=null))
                {
                    currentModel = first ? currentModel.Previous.Point : currentModel.Next.Point;
                }
                else
                {
                    break;
                }
           
            }

            return currentModel;

        }



        // Use https://roadcurvature.com/how-it-works/
        private double getCircleRadiusFromPoints(LocationPoint point1, LocationPoint point2, LocationPoint point3)
        {

            double a = CalculateDistanceBetweenPoints(point2, point3);
            double b = CalculateDistanceBetweenPoints(point1, point3);
            double c = CalculateDistanceBetweenPoints(point1, point2);

            return ((a * b * c) / Math.Sqrt((a + b + c) * (b + c - a) * (c + a - b) * (a + b - c)));

        }


    }
}
