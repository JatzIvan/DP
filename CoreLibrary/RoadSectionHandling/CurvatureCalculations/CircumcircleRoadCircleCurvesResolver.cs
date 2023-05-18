using CoreLibrary.RoadSectionHandling.Data;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.CurvatureCalculations
{
    /**
     * Simple curvature calculator based on 3 road points
     * This calculator takes into account curvature direction (small curvature in right direction is canceled by small curvature in left direction)
     */

    [CurvatureResolver]
    public class CircumcircleRoadCircleCurvesResolver : ICurvesResolver
    {

        private List<AbstractRoadModel> ConnectedWays { get; set; }

        public CircumcircleRoadCircleCurvesResolver()
        {
        }

        public List<AbstractRoadModel> CalculateCurvesForWays(List<AbstractRoadModel> connectedWays)
        {

            this.ConnectedWays = connectedWays;
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
                    current.Angle = CalculateAngle(before.CurrentLocation, current.CurrentLocation, after.CurrentLocation);
                }
                //double circleRadius = getCircleRadiusFromPoints(before.CurrentLocation, current.CurrentLocation, after.CurrentLocation);
                current = current.Next.Point;

            }

            // Approximate edge values
            current = GetModel(true);
            current.RadiusOfCircle = current.Next != null ? current.Next.Point.RadiusOfCircle : 0;
            current.Angle = current.Next != null ? current.Next.Point.Angle : 0;
            current = GetModel(false);
            current.RadiusOfCircle = current.Previous != null ? current.Previous.Point.RadiusOfCircle : 0;
            current.Angle = current.Previous != null ? current.Previous.Point.Angle : 0;

            // Calculate curviture 
            current = GetModel(true);
            while (true)
            {
                AbstractRoadModel after = current.Next != null ? current.Next.Point : null;

                if (after == null)
                {
                    break;
                }


                /*current.Next.RadiusOfCurvature = current.RadiusOfCircle > 0 
                    ? 1/(current.RadiusOfCircle + current.Next.Point.RadiusOfCircle)/ 2 
                    : 1/current.Next.Point.RadiusOfCircle;*/

                // Use harmonic mean (which is just transformed aritmetic mean)
                current.Next.RadiusOfCurvature = current.RadiusOfCircle > 0
                    ? 1 / (
                    Math.Abs(2 * current.RadiusOfCircle * current.Next.Point.RadiusOfCircle / 
                    (Math.Sign(current.Angle) * current.RadiusOfCircle + Math.Sign(current.Next.Point.Angle) * current.Next.Point.RadiusOfCircle)))
                    : 1 / current.Next.Point.RadiusOfCircle;


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

                /*current.Previous.RadiusOfCurvature = current.RadiusOfCircle > 0 
                    ? 1/((current.RadiusOfCircle + current.Previous.Point.RadiusOfCircle) / 2)
                    : 1/current.Previous.Point.RadiusOfCircle;*/

                // Use harmonic mean (which is just transformed aritmetic mean)
                current.Previous.RadiusOfCurvature = current.RadiusOfCircle > 0
                        ? 1 / (
                        Math.Abs(2 * current.RadiusOfCircle * current.Previous.Point.RadiusOfCircle /
                        (Math.Sign(current.Angle) * current.RadiusOfCircle + Math.Sign(current.Previous.Point.Angle) * current.Previous.Point.RadiusOfCircle)))
                        : 1 / current.Previous.Point.RadiusOfCircle;

                current = current.Previous.Point;

            }

            return ConnectedWays;

        }

        /**
         * Utility method to get first or last point of road
         */
        private AbstractRoadModel GetModel(bool first)
        {

            AbstractRoadModel currentModel = ConnectedWays.First();

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

        /**
         * Signed angle is used to determine direction of curvature (from vectors)
         */
        private double CalculateAngle(LocationPoint bef, LocationPoint curr, LocationPoint after)
        {
            Vector2 v0 = new Vector2(Convert.ToSingle(curr.Longitude - bef.Longitude), Convert.ToSingle(curr.Latitude - bef.Latitude));
            Vector2 v1 = new Vector2(Convert.ToSingle(after.Longitude - curr.Longitude), Convert.ToSingle(after.Latitude - curr.Latitude));

            double angle = MapParserUtils.ConvertRadiansToDegrees(Math.Atan2(MapParserUtils.CalculateCrossProduct(v0, v1), Vector2.Dot(v0, v1)));

            return angle != 0 ? angle : 1e-6;

        }



        /**
         * Simple osculating circle from triangle
         */
        private double GetCircleRadiusFromPoints(LocationPoint point1, LocationPoint point2, LocationPoint point3)
        {

            double a = MapParserUtils.CalculateDistanceBetweenPoints(point2, point3);
            double b = MapParserUtils.CalculateDistanceBetweenPoints(point1, point3);
            double c = MapParserUtils.CalculateDistanceBetweenPoints(point1, point2);

            return ((a * b * c) / Math.Sqrt((a + b + c) * (b + c - a) * (c + a - b) * (a + b - c)));

        }
    }
}
