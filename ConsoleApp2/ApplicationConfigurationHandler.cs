using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;

namespace ConsoleApp2
{
    public class ApplicationConfigurationHandler
    {

        public static string DataServer { get; set; }
        public static string DatabaseConnection { get; set; }

        public static string DigitalMapConnection { get; set; }

        public static string TestRoadQuery { get; set; }
        public static void LoadConfiguration()
        {
            DataServer = ConfigurationManager.AppSettings.Get("DataServer");
            DatabaseConnection = ConfigurationManager.AppSettings.Get("DatabaseConnection");
            DigitalMapConnection = ConfigurationManager.AppSettings.Get("DigiMap");
            TestRoadQuery = ConfigurationManager.AppSettings.Get("RoadQuery");
        }

    }
}
