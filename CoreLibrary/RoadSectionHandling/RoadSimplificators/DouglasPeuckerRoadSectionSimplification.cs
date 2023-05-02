using CoreLibrary.RoadSectionHandling.Data;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.RoadSimplificators
{
    [RoadSimplificator]
    public class DouglasPeuckerRoadSectionSimplification : ISectionSimplificator{ 
/*        private float Tolerance { get; set; } = 0.05f;*/

        public DouglasPeuckerRoadSectionSimplification() : base()
        {
        }

        private Tuple<double, int> FindPointWithMaximumDistance(List<AbstractRoadModel> model)
        {

            if (model.Count <= 2)
            {
                return new Tuple<double, int>(0D, 0);
            }

            //double distance = RoadDataHandler.CalculateDistanceBetweenPoints(model.First().CurrentLocation, model.Last().CurrentLocation);


            double largestDistance = 0D;
            int index = 0;

            for (int i = 1; i < model.Count - 1; i++)
            {

                double currDistance = MapParserUtils.CalcShortestDistancePointToLine(model.First().CurrentLocation, model.Last().CurrentLocation, model[i].CurrentLocation);

                if (currDistance > largestDistance)
                {
                    largestDistance = currDistance;
                    index = i;
                }

            }

            return new Tuple<double, int>(largestDistance, index);
        }

        protected override List<AbstractRoadModel> Simplify(List<AbstractRoadModel> model)
        {

            Tuple<double, int> maximumDistance = FindPointWithMaximumDistance(model);

            //List<RoadCurvitureModel> newOutputList = new List<RoadCurvitureModel>();


            // We cannot remove points, because they are important for road definition
            // Simplify further
            if (maximumDistance.Item1 > Config.Tolerance)
            {

                // Calculate for points from first point to the point with largest distance
                List<AbstractRoadModel> firstPartOutput = Simplify(new List<AbstractRoadModel>(model.Take(maximumDistance.Item2 + 1)));

                // Calculate for points from the point with largest distance to the last point
                List<AbstractRoadModel> secondPartOutput = Simplify(new List<AbstractRoadModel>(model.Skip(maximumDistance.Item2)));

                List<AbstractRoadModel> outPutList = new List<AbstractRoadModel>(firstPartOutput.SkipLast(1));
                outPutList.AddRange(secondPartOutput);

                return outPutList;

            }
            // We can remove all points, because they are not important for road definition
            else
            {
                return new List<AbstractRoadModel>
                {
                    model.First(), model.Last()
                };

            }

        }

        public override AbstractSimplificationModel CreateConfig()
        {
            return new DouglasPeuckerConfig(ApplicationConfigurationHandler.DPTolerance);
        }
    }
}
