using CoreLibrary;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using CoreLibrary.RoadSectionHandling.CurvatureCalculations;
using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.RoadSimplificators;
using NetTopologySuite.Index.KdTree;
using System.Diagnostics;
using WebSocketLibrary.Models;
using WebSocketLibrary;
using Xunit;
using Newtonsoft.Json;

namespace TestingLibrary
{
    [Collection("Sequential")]
    public class TestController : AbstractTestController
    {

        public TestController() : base()
        {

        }

        [Fact]
        public void TestRoadHandlerWithDefaultConfiguration()
        {
            ApplicationConfigurationHandler.DPTolerance = 0;
            ApplicationConfigurationHandler.SimplificationMethod = typeof(DouglasPeuckerRoadSectionSimplification).Name;
            RoadDataHandler handler = new RoadDataHandler("503", "503");
            handler.roadDataFether = roadDataFetcher.Object;

            List<AbstractRoadModel> parsedList = handler.GetParsedRoadDataList();
            Xunit.Assert.Equal(889, parsedList.Count);

            KdTree<AbstractRoadModel> kdTree = handler.GetParsedRoadData();
            Xunit.Assert.Equal(889, kdTree.Count);
        }

        [Theory]
        [InlineData(0, 889)]
        [InlineData(0.5, 605)]
        [InlineData(1, 415)]
        public void TestRoadSimplificationWithDouglasPeucker(float simplificationTolerance, float expectedNumberOfPoints)
        {
            ApplicationConfigurationHandler.LoadConfiguration();
            ApplicationConfigurationHandler.SimplificationMethod = typeof(DouglasPeuckerRoadSectionSimplification).Name;
            ApplicationConfigurationHandler.DPTolerance = simplificationTolerance;

            RoadDataHandler handler = new RoadDataHandler("503", "503");
            handler.roadDataFether = roadDataFetcher.Object;

            List<AbstractRoadModel> parsedList = handler.GetParsedRoadDataList();
            Xunit.Assert.Equal(expectedNumberOfPoints, parsedList.Count);

            KdTree<AbstractRoadModel> kdTree = handler.GetParsedRoadData();
            Xunit.Assert.Equal(expectedNumberOfPoints, kdTree.Count);
        }

        [Theory]
        [InlineData(0, 5, 889)]
        [InlineData(0.5, 5, 553)]
        [InlineData(1, 5, 367)]
        public void TestRoadSimplificationWithLang(float simplificationTolerance, int regionSize, float expectedNumberOfPoints)
        {
            ApplicationConfigurationHandler.LoadConfiguration();
            ApplicationConfigurationHandler.SimplificationMethod = typeof(LangRoadSectionSimplification).Name;
            ApplicationConfigurationHandler.DPTolerance = simplificationTolerance;
            ApplicationConfigurationHandler.LangRegionSize = regionSize;

            RoadDataHandler handler = new RoadDataHandler("503", "503");
            handler.roadDataFether = roadDataFetcher.Object;

            List<AbstractRoadModel> parsedList = handler.GetParsedRoadDataList();
            Xunit.Assert.Equal(expectedNumberOfPoints, parsedList.Count);

            KdTree<AbstractRoadModel> kdTree = handler.GetParsedRoadData();
            Xunit.Assert.Equal(expectedNumberOfPoints, kdTree.Count);
        }


        [Theory]
        // 888 is one less than number of points, because we resolve based on curvature between points
        [InlineData(0, 888)]
        [InlineData(0.006, 435)]
        [InlineData(0.01, 343)]
        public void TestCircleCurvatureResolver(float curveTolerance, int numberOfDangerousRegions)
        {
            ApplicationConfigurationHandler.LoadConfiguration();
            ApplicationConfigurationHandler.DPTolerance = 0;
            ApplicationConfigurationHandler.SimplificationMethod = typeof(DouglasPeuckerRoadSectionSimplification).Name;
            ApplicationConfigurationHandler.CurvetureCalcMethod = typeof(CircumcircleRoadCircleCurvesResolver).Name;

            RoadDataHandler handler = new RoadDataHandler("503", "503");
            handler.roadDataFether = roadDataFetcher.Object;

            List<AbstractRoadModel> parsedList = handler.GetParsedRoadDataList();

            int count = 0;
            AbstractRoadModel first = parsedList[0];
            while (true)
            {
                if (first.Next == null)
                {
                    break;
                }

                if (first.Next.RadiusOfCurvature > curveTolerance)
                {
                    count++;
                }

                first = first.Next.Point;

            }

            Xunit.Assert.Equal(numberOfDangerousRegions, count);

        }

    }
}