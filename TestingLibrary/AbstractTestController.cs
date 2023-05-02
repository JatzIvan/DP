using CoreLibrary;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.Model;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestingLibrary
{
    public abstract class AbstractTestController
    {

        protected Mock<RoadDataFetcher> roadDataFetcher;

        public AbstractTestController() 
        {
            //ApplicationConfigurationHandler.LoadConfiguration();
            roadDataFetcher = new Mock<RoadDataFetcher>();
            roadDataFetcher.Setup(x => x.GetRoadFromAPI()).Returns(ParseModel());
        }

        private string GetPath(string relativePath)
        {

            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            return Path.Combine(dirPath, relativePath);

        }

        protected List<RoadPointModel> ParseModel()
        {
            string fileName = @"Resources/Roads.json";
            string jsonString = File.ReadAllText(GetPath(fileName));
            List<RoadPointModel> roadRaw = JsonConvert.DeserializeObject<List<RoadPointModel>>(jsonString)!;
            return roadRaw;
        }

    }
}
