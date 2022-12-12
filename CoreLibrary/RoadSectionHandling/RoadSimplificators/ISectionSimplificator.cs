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

        private List<AbstractRoadModel> SimplifiedModel { get; set; }

        private List<AbstractRoadModel> ConnectedWays { get; set; }

        protected AbstractSimplificationModel Config { get; set; }

        public ISectionSimplificator(List<AbstractRoadModel> connectedWays)
        {
            if (connectedWays is null)
            {
                throw new ArgumentNullException(nameof(connectedWays));
            }

            this.ConnectedWays = connectedWays;
            this.Config = CreateConfig();
        }

        /**
         * This method should implement custom simplification logic
         */
        protected abstract List<AbstractRoadModel> Simplify(List<AbstractRoadModel> model);

        private List<AbstractRoadModel> CreateSortedListOfRoadPoints(List<AbstractRoadModel> connectedWays)
        {

            AbstractRoadModel firstPoint = connectedWays.First();

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
        public List<AbstractRoadModel> GetSimplifiedModel()
        {
            if (SimplifiedModel != null && SimplifiedModel.Count > 0)
            {
                return SimplifiedModel;
            }

            List<AbstractRoadModel> simplifiedModelList = Simplify(CreateSortedListOfRoadPoints(ConnectedWays));

            SimplifiedModel = new List<AbstractRoadModel>();

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

                SimplifiedModel.Add(model);
                previous = model;
            }

            return SimplifiedModel;

        }

        public abstract AbstractSimplificationModel CreateConfig();

    }
}
