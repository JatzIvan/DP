using ConsoleApp2.RoadSectionHandling.Model;
using ConsoleApp2.RoadSectionHandling.RoadSimplificators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling.Data
{
    public abstract class ISectionSimplificator
    {

        private Dictionary<LocationPoint, AbstractRoadModel> SimplifiedModel { get; set; }

        private Dictionary<LocationPoint, AbstractRoadModel> ConnectedWays { get; set; }

        protected AbstractSimplificationModel Config { get; set; }

        public ISectionSimplificator(Dictionary<LocationPoint, AbstractRoadModel> connectedWays, AbstractSimplificationModel config)
        {
            if (connectedWays is null || config is null)
            {
                throw new ArgumentNullException(nameof(connectedWays));
            }

            this.ConnectedWays = connectedWays;
            this.Config = config;
        }

        protected abstract List<AbstractRoadModel> Simplify(List<AbstractRoadModel> model);

        private List<AbstractRoadModel> CreateSortedListOfRoadPoints(Dictionary<LocationPoint, AbstractRoadModel> connectedWays)
        {

            AbstractRoadModel firstPoint = connectedWays.First().Value;

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

        public Dictionary<LocationPoint, AbstractRoadModel> GetSimplifiedModel()
        {
            if (SimplifiedModel != null && SimplifiedModel.Count > 0)
            {
                return SimplifiedModel;
            }

            List<AbstractRoadModel> simplifiedModelList = Simplify(CreateSortedListOfRoadPoints(ConnectedWays));

            SimplifiedModel = new Dictionary<LocationPoint, AbstractRoadModel>();

            AbstractRoadModel previous = null;

            foreach (AbstractRoadModel model in simplifiedModelList)
            {

                if (previous != null)
                {
                    previous.Next.Point = model;
                    model.Previous.Point = previous;
                }

                SimplifiedModel.Add(model.CurrentLocation, model);
                previous = model;
            }

            return SimplifiedModel;

        }

    }
}
