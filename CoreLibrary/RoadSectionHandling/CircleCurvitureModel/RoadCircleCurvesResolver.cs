using ConsoleApp2.RoadSectionHandling.Data;
using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling.CircleCurvitureModel
{
    class RoadCircleCurvesResolver : ICurvesResolver
    {

        private Dictionary<LocationPoint, AbstractRoadModel> ConnectedWays { get; set; }

        public RoadCircleCurvesResolver(Dictionary<LocationPoint, AbstractRoadModel> connectedWays)
        {
            this.ConnectedWays = connectedWays;
        }

        public Dictionary<LocationPoint, AbstractRoadModel> CalculateCurvesForWays()
        {


            // Calculate both ways

            // From first to last

            AbstractRoadModel current = GetModel(true);

            // Calculate Circle Radius
            while (true)
            {
                AbstractRoadModel before = current.Previous != null ? current.Previous.Point : null;
                AbstractRoadModel after = current.Next != null ? current.Next.Point : null;

                if (after == null)
                {
                    break;
                }

                if (before != null)
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
                AbstractRoadModel after = current.Next != null ? current.Next.Point : null;

                if (after == null)
                {
                    break;
                }

                current.Next.RadiusOfCurvature = current.RadiusOfCircle > 0 
                    ? 1/(current.RadiusOfCircle + current.Next.Point.RadiusOfCircle)/ 2 
                    : 1/current.Next.Point.RadiusOfCircle;

                current = current.Next.Point;

            }

            current = GetModel(false);
            while (true)
            {
                AbstractRoadModel before = current.Previous != null ? current.Previous.Point : null;

                if (before == null)
                {
                    break;
                }

                current.Previous.RadiusOfCurvature = current.RadiusOfCircle > 0 
                    ? 1/(current.RadiusOfCircle + current.Previous.Point.RadiusOfCircle) / 2
                    : 1/current.Previous.Point.RadiusOfCircle;

                current = current.Previous.Point;

            }

            return ConnectedWays;

        }

        private AbstractRoadModel GetModel(bool first)
        {

            AbstractRoadModel currentModel = ConnectedWays.First().Value;

            while (true)
            {

                if ((first && currentModel.Previous != null) || (!first && currentModel.Next != null))
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
    }
}
