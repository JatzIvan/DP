using ConsoleApp2.RoadSectionHandling.Data;
using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling.RoadSimplificators
{
    public class LangRoadSectionSimplification : ISectionSimplificator
    {

        public LangRoadSectionSimplification(Dictionary<LocationPoint, AbstractRoadModel> connectedWays, LangConfig conf) : base(connectedWays, conf)
        {
        }

        private List<AbstractRoadModel> LangSimpl(List<AbstractRoadModel> model, int regSize)
        {

            if (model.Count <= 2 || regSize <= 1)
            {
                return model;
            }

            // Calculate for points from first point to the point with largest distance
            List<AbstractRoadModel> firstPartOutput = new List<AbstractRoadModel>(model.Take(regSize + 1));


            bool noSkip = false;

            for(int i = 1; i < firstPartOutput.Count - 1; i++)
            {
                double currDistance = MapParserUtils.CalcShortestDistancePointToLine(firstPartOutput.First().CurrentLocation, firstPartOutput.Last().CurrentLocation, firstPartOutput[i].CurrentLocation);
                if(currDistance > Config.Tolerance)
                {
                    noSkip = true;
                    break;
                }
            }

            // When at least one point is out of tolerance skip last and repeat
            if (noSkip)
            {
                return LangSimpl(model, regSize - 1);
            }

            // This construction is mostly to keep sorted list property
            List<AbstractRoadModel> newModel = new List<AbstractRoadModel>
                {
                    firstPartOutput.First(), firstPartOutput.Last()
                };

            newModel.AddRange(model.Skip(regSize + 1));
            return newModel;

        }

        protected override List<AbstractRoadModel> Simplify(List<AbstractRoadModel> model)
        {
            LangConfig currentConfig = (LangConfig) Config;

            int index = 0;

            while(index < model.Count)
            {
                List<AbstractRoadModel> pointsToKeep = new List<AbstractRoadModel>(model.Take(index));
                pointsToKeep.AddRange(LangSimpl(model.Skip(index).ToList() , currentConfig.RegionSize));
                model = pointsToKeep;
                index++;
            }

            return model;
        }
    }
}
