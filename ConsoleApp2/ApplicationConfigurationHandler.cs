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

        public static float DPTolerance { get; set; }

        public static void LoadConfiguration()
        {
            DataServerHost = ConfigurationManager.AppSettings.Get("DataServerHost");
            DataServerPort = ConfigurationManager.AppSettings.Get("DataServerPort");
            DatabaseConnection = ConfigurationManager.AppSettings.Get("DatabaseConnection");
            DigitalMapConnection = ConfigurationManager.AppSettings.Get("DigiMap");
            TestRoadQuery = ConfigurationManager.AppSettings.Get("RoadQuery");
            DPTolerance = float.Parse(ConfigurationManager.AppSettings.Get("LineSimplificationTolerance"), CultureInfo.InvariantCulture);
        }

    }
}
