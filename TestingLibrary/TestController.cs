using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestingLibrary
{
    public class TestController: AbstractTestController
    {

        public TestController(): base()
        {

        }

        [Fact]
        public void TestRoadHandlerWithDefaultConfiguration()
        {
            RoadDataHandler handler = new RoadDataHandler("503", "503");
            handler.roadDataFether = roadDataFetcher.Object;

            List<AbstractRoadModel> parsedList = handler.GetParsedRoadDataList();
            Xunit.Assert.Equal(889, parsedList.Count);
        }


    }
}
