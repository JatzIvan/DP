using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling
{
   /* public class CurvesResolver
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
                    current.RadiusOfCircle = GetCircleRadiusFromPoints(before.CurrentLocation, current.CurrentLocation, after.CurrentLocation);
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
        private double GetCircleRadiusFromPoints(LocationPoint point1, LocationPoint point2, LocationPoint point3)
        {

            double a = MapParserUtils.CalculateDistanceBetweenPoints(point2, point3);
            double b = MapParserUtils.CalculateDistanceBetweenPoints(point1, point3);
            double c = MapParserUtils.CalculateDistanceBetweenPoints(point1, point2);

            return ((a * b * c) / Math.Sqrt((a + b + c) * (b + c - a) * (c + a - b) * (a + b - c)));

        }


    }*/
}
