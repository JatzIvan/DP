using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.CollisionCalculators.CurveCalculators;
using CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators;
using CoreLibrary.RoadSectionHandling.CurvatureCalculations;
using CoreLibrary.RoadSectionHandling.Data;
using CoreLibrary.RoadSectionHandling.MaxSpeedCalculations;
using CoreLibrary.RoadSectionHandling.RoadSimplificators;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace CoreLibrary
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

        public static int LangRegionSize { get; set; }

        public static string SimplificationMethod { get; set; } = typeof(DouglasPeuckerRoadSectionSimplification).Name;

        public static string CurvetureCalcMethod { get; set; } = typeof(CircumcircleRoadCircleCurvesResolver).Name;

        public static string MaxSpeedCalcMethod { get; set; } = typeof(SimpleSpeedCalculatorBasedOnCurvature).Name;

        public static float CurvatureTreshold { get; set; }

        public static float CarDistanceSkipTreshold { get; set; }

        public static string CurveCollisionCalculator { get; set; }

        public static string StraightCollisionCalculator { get; set; }

        /*public static AbstractSimplificationModel GetSimplificationModelFromConfiguration()
        {

            switch (SimplificationMethod)
            {
                case SimplMethods.DouglasPeuckerRoadSectionSimplification.ToString():

                    return new DouglasPeuckerConfig(DPTolerance);

                case SimplMethods.LangRoadSectionSimplification.ToString():
                    int regionSize;
                    try
                    {
                        regionSize = int.Parse(ConfigurationManager.AppSettings.Get("LangRegionSize"), CultureInfo.InvariantCulture);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine("Missing or invalid config value for region size, defaulting 4");
                        regionSize = 4;
                    }
                    return new LangConfig(DPTolerance, regionSize);

                default:
                    Console.WriteLine("Missing Simplification method type in config, default with DouglasPeuckerConfig with tolarance of 0.001");
                    return new DouglasPeuckerConfig(0.001f);
            }


        }*/

        public static HandlerSetupConfig GenerateHandlerSetupConfig()
        {
            return new HandlerSetupConfig(SimplificationMethod , CurvetureCalcMethod);
        }

        // Load all type implementations for constructors at the beginig of execution
        public static void InitConstructors()
        {
            Stopwatch sw = Stopwatch.StartNew();
            System.Collections.Generic.List<Type> factoriesToInit = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(p => typeof(AbstractCalculatorFactory).IsAssignableFrom(p)).ToList();
            
            foreach(Type factory in factoriesToInit)
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(factory.TypeHandle);
            }

            Console.WriteLine("Factory init took " + sw.ElapsedMilliseconds);
        }

        public static void LoadConfiguration()
        {
            try
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
                //SimplificationMethod = (SimplMethods)Enum.Parse(typeof(SimplMethods), ConfigurationManager.AppSettings.Get("SimplificationMethod"));
                //CurvetureCalcMethod = (CurvCalcMethods)Enum.Parse(typeof(CurvCalcMethods), ConfigurationManager.AppSettings.Get("CurvetureCalcMethod"));
                SimplificationMethod = ConfigurationManager.AppSettings.Get("SimplificationMethod");
                CurvetureCalcMethod = ConfigurationManager.AppSettings.Get("CurvetureCalcMethod");
                MaxSpeedCalcMethod = ConfigurationManager.AppSettings.Get("MaxSpeedCalcMethod");
                InitConstructors();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured during config parsing");
                Console.WriteLine(e);
            }
        }

    }
}
