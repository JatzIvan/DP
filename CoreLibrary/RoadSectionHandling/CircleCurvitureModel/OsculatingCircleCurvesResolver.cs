using ConsoleApp2.RoadSectionHandling.Data;
using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling.CircleCurvitureModel
{
    class OsculatingCircleCurvesResolver : ICurvesResolver
    {
        private Dictionary<LocationPoint, AbstractRoadModel> ConnectedWays { get; set; }

        public OsculatingCircleCurvesResolver(Dictionary<LocationPoint, AbstractRoadModel> connectedWays)
        {
            this.ConnectedWays = connectedWays;
        }

        public Dictionary<LocationPoint, AbstractRoadModel> CalculateCurvesForWays()
        {


            // Calculate both ways

            // From first to last
            /*
                        AbstractRoadModel current = GetModel(true);*/

            // Calculate Circle Radius
            AbstractRoadModel current = GetModel(true);
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
                    // Lets play that "Radius" is tangent
                    current.TangentOfPoint = CalcTangentVectorOfCurve(current);
                }
                //double circleRadius = getCircleRadiusFromPoints(before.CurrentLocation, current.CurrentLocation, after.CurrentLocation);
                current = current.Next.Point;

            }

            // Calculate curviture 
            current = GetModel(true);
            while (true)
            {
                AbstractRoadModel after = current.Next != null ? current.Next.Point : null;
                AbstractRoadModel before = current.Previous != null ? current.Previous.Point : null;

                if (after == null)
                {
                    break;
                }

                if (after.TangentOfPoint == null || before == null || before.TangentOfPoint == null)
                {
                    current = current.Next.Point;
                    continue;
                }

                current.Next.RadiusOfCurvature = MapParserUtils.CalculateLocationNorm(subtractLocations(after.TangentOfPoint, before.TangentOfPoint)) /
                    MapParserUtils.CalculateLocationNorm(subtractLocations(after.CurrentLocation, before.CurrentLocation));

                current = current.Next.Point;

            }

            current = GetModel(false);
            while (true)
            {
                AbstractRoadModel after = current.Next != null ? current.Next.Point : null;
                AbstractRoadModel before = current.Previous != null ? current.Previous.Point : null;

                if (before == null)
                {
                    break;
                }

                if (after == null || after.TangentOfPoint == null || before.TangentOfPoint == null)
                {
                    current = current.Previous.Point;
                    continue;
                }

                current.Previous.RadiusOfCurvature = MapParserUtils.CalculateLocationNorm(subtractLocations(before.TangentOfPoint, after.TangentOfPoint)) /
                    MapParserUtils.CalculateLocationNorm(subtractLocations(before.CurrentLocation, after.CurrentLocation));

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

        private LocationPoint subtractLocations(LocationPoint p1, LocationPoint p2)
        {
            return new LocationPoint((p1.Longitude - p2.Longitude), p1.Latitude - p2.Latitude);
        }

        private LocationPoint CalcTangentVectorOfCurve(AbstractRoadModel point)
        {
            if (point.Next == null || point.Previous == null)
            {
                return null;
            }
            LocationPoint pointx_p1 = point.Next.Point.CurrentLocation;
            LocationPoint pointx_m1 = point.Previous.Point.CurrentLocation;

            LocationPoint newPoint = subtractLocations(pointx_p1, pointx_m1);

            return new LocationPoint(newPoint.Longitude / MapParserUtils.CalculateLocationNorm(newPoint), newPoint.Latitude / MapParserUtils.CalculateLocationNorm(newPoint));
        }
    }
}
