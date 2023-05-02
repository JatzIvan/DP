using CoreLibrary.RoadSectionHandling.Data;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.RoadSimplificators
{
    [RoadSimplificator]
    public class LangRoadSectionSimplification : ISectionSimplificator
    {

        public LangRoadSectionSimplification() : base()
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

        public override AbstractSimplificationModel CreateConfig()
        {
            int regionSize;
            try
            {
                if(ApplicationConfigurationHandler.LangRegionSize == 0)
                {
                    regionSize = int.Parse(ConfigurationManager.AppSettings.Get("LANG_REGION_SIZE"), CultureInfo.InvariantCulture);
                }
                else
                {
                    regionSize = ApplicationConfigurationHandler.LangRegionSize;
                }
                //regionSize = int.Parse(ConfigurationManager.AppSettings.Get("LangRegionSize"), CultureInfo.InvariantCulture);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Missing or invalid config value for region size, defaulting 4");
                regionSize = 4;
            }
            return new LangConfig(ApplicationConfigurationHandler.DPTolerance, regionSize);
        }
    }
}
