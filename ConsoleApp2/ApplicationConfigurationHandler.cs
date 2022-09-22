using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Globalization;

namespace ConsoleApp2
{
    public class ApplicationConfigurationHandler
    {

        public static string DataServerHost { get; set; }

        public static string DataServerPort { get; set; }
        public static string DatabaseConnection { get; set; }

        public static string DigitalMapConnection { get; set; }

        public static string TestRoadQuery { get; set; }

        public static double Longitude1 { get; set; }

        public static double Latitude1 { get; set; }

        public static double Longitude2 { get; set; }

        public static double Latitude2 { get; set; }

        public static string RoadRef { get; set; }

        public static float DPTolerance { get; set; }

        public static float CurvatureTreshold { get; set; }

        public static float CarDistanceSkipTreshold { get; set; }

        public static string CurveCollisionCalculator { get; set; }

        public static string StraightCollisionCalculator { get; set; }

        public static void LoadConfiguration()
        {
            DataServerHost = ConfigurationManager.AppSettings.Get("DataServerHost");
            DataServerPort = ConfigurationManager.AppSettings.Get("DataServerPort");
            DatabaseConnection = ConfigurationManager.AppSettings.Get("DatabaseConnection");
            DigitalMapConnection = ConfigurationManager.AppSettings.Get("DigiMap");
            Longitude1 = float.Parse(ConfigurationManager.AppSettings.Get("RoadQueryLong1"), CultureInfo.InvariantCulture);
            Latitude1 = float.Parse(ConfigurationManager.AppSettings.Get("RoadQueryLat1"), CultureInfo.InvariantCulture);
            Longitude2 = float.Parse(ConfigurationManager.AppSettings.Get("RoadQueryLong2"), CultureInfo.InvariantCulture);
            Latitude2 = float.Parse(ConfigurationManager.AppSettings.Get("RoadQueryLat2"), CultureInfo.InvariantCulture);
            RoadRef = ConfigurationManager.AppSettings.Get("RoadQueryRef");
            TestRoadQuery = $"?ref={RoadRef}&long1={ConfigurationManager.AppSettings.Get("RoadQueryLong1")}" +
                $"&lat1={ConfigurationManager.AppSettings.Get("RoadQueryLat1")}" +
                $"&long2={ConfigurationManager.AppSettings.Get("RoadQueryLong2")}" +
                $"&lat2={ConfigurationManager.AppSettings.Get("RoadQueryLat2")}";
            DPTolerance = float.Parse(ConfigurationManager.AppSettings.Get("LineSimplificationTolerance"), CultureInfo.InvariantCulture);
            CurvatureTreshold = float.Parse(ConfigurationManager.AppSettings.Get("CurvatureTreshold"), CultureInfo.InvariantCulture);
            CarDistanceSkipTreshold = float.Parse(ConfigurationManager.AppSettings.Get("CarDistanceSkipTreshold"), CultureInfo.InvariantCulture);
            CurveCollisionCalculator = ConfigurationManager.AppSettings.Get("CurveCollisionCalculator");
            StraightCollisionCalculator = ConfigurationManager.AppSettings.Get("StraightCollisionCalculator");
        }

    }
}
