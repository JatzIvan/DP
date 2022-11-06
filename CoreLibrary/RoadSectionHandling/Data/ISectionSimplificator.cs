using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.RoadSimplificators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.Data
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

        /**
         * This method should implement custom simplification logic
         */
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

        /**
         * Generic implementation to get Simplified road model. 
         * This function performs all necessary steps so implementing new "Algorithms" should be easy
         */
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
                    // Link road points and calculate Heading between segments
                    previous.Next.Point = model;
                    previous.Next.Heading = MapParserUtils.CalculateBearingBetweenPoints(previous.CurrentLocation, model.CurrentLocation);

                    model.Previous.Point = previous;
                    model.Previous.Heading = MapParserUtils.CalculateBearingBetweenPoints(model.CurrentLocation, previous.CurrentLocation);
                }

                SimplifiedModel.Add(model.CurrentLocation, model);
                previous = model;
            }

            return SimplifiedModel;

        }

    }
}
