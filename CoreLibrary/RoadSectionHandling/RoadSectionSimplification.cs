using CoreLibrary.RoadSectionHandling.Data;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling
{
   /* class RoadSectionSimplification : ISectionSimplificator
    {

        private float Tolerance { get; set; } = 0.05f;

        private Dictionary<LocationPoint, AbstractRoadModel> ConnectedWays { get; set; }

        private Dictionary<LocationPoint, AbstractRoadModel> SimplifiedModel { get; set; }

        public RoadSectionSimplification(float tolerance, Dictionary<LocationPoint, AbstractRoadModel> connectedWays)
        {
            if (connectedWays is null)
            {
                throw new ArgumentNullException(nameof(connectedWays));
            }

            this.Tolerance = tolerance;
            this.ConnectedWays = connectedWays;
        }

        public RoadSectionSimplification(Dictionary<LocationPoint, AbstractRoadModel> connectedWays)
        {
            if (connectedWays is null)
            {
                throw new ArgumentNullException(nameof(connectedWays));
            }

            this.ConnectedWays = connectedWays;

        }

        public Dictionary<LocationPoint, AbstractRoadModel> GetSimplifiedModel()
        {
            if (SimplifiedModel != null && SimplifiedModel.Count > 0)
            {
                return SimplifiedModel;
            }

            List<AbstractRoadModel> simplifiedModelList = Simplify(CreateSortedListOfRoadPoints());

            SimplifiedModel = new Dictionary<LocationPoint, AbstractRoadModel>();

            AbstractRoadModel previous = null;

            foreach (AbstractRoadModel model in simplifiedModelList)
            {

                if(previous != null)
                {
                    previous.Next.Point = model;
                    model.Previous.Point = previous;
                }

                SimplifiedModel.Add(model.CurrentLocation, model);
                previous = model;
            }

            return SimplifiedModel;

        }

        private List<AbstractRoadModel> CreateSortedListOfRoadPoints()
        {

            AbstractRoadModel firstPoint = ConnectedWays.First().Value;

            //Find first
            while (true)
            {
                
                if (firstPoint.Previous != null)
                {
                    firstPoint = firstPoint.Previous.Point;                
                }
                else
                {
                    break;
                }

            }

            //Create sorted list for easier manipulation

            List<AbstractRoadModel> sortedModel = new List<AbstractRoadModel>();

            while (true)
            {
                sortedModel.Add(firstPoint);
                if (firstPoint.Next != null)
                {
                    firstPoint = firstPoint.Next.Point;
                }
                else
                {
                    break;
                }
            }

            return sortedModel;

        }

        private Tuple<double, int> FindPointWithMaximumDistance(List<AbstractRoadModel> model)
        {

            if(model.Count <= 2) 
            {
                return new Tuple<double, int>(0D, 0);    
            }

            //double distance = RoadDataHandler.CalculateDistanceBetweenPoints(model.First().CurrentLocation, model.Last().CurrentLocation);


            double largestDistance = 0D;
            int index = 0;

            for (int i = 1; i<model.Count - 1; i++)
            {

                double currDistance = MapParserUtils.CalcShortestDistancePointToLine(model.First().CurrentLocation, model.Last().CurrentLocation, model[i].CurrentLocation);

                if(currDistance > largestDistance)
                {
                    largestDistance = currDistance;
                    index = i;
                }

            }

            return new Tuple<double, int>(largestDistance, index);
        }

        private List<AbstractRoadModel> Simplify(List<AbstractRoadModel> model)
        {

            Tuple<double, int> maximumDistance = FindPointWithMaximumDistance(model);

            //List<RoadCurvitureModel> newOutputList = new List<RoadCurvitureModel>();


            // We cannot remove points, because they are important for road definition
            // Simplify further
            if(maximumDistance.Item1 > Tolerance)
            {

                // Calculate for points from first point to the point with largest distance
                List<AbstractRoadModel> firstPartOutput = Simplify(new List<AbstractRoadModel>(model.Take(maximumDistance.Item2 + 1)));

                // Calculate for points from the point with largest distance to the last point
                List<AbstractRoadModel> secondPartOutput = Simplify(new List<AbstractRoadModel>(model.Skip(maximumDistance.Item2)));

                List<AbstractRoadModel>  outPutList = new List<AbstractRoadModel>(firstPartOutput.SkipLast(1));
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


    }*/
}
