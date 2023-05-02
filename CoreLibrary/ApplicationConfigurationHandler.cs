using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.CollisionCalculators.CurveCalculators;
using CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators;
using CoreLibrary.RoadSectionHandling.CurvatureCalculations;
using CoreLibrary.RoadSectionHandling.Data;
using CoreLibrary.RoadSectionHandling.MaxSpeedCalculations;
using CoreLibrary.RoadSectionHandling.RoadParameters;
using CoreLibrary.RoadSectionHandling.RoadSimplificators;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using WebSocketLibrary.Models;

namespace CoreLibrary
{
    public class ApplicationConfigurationHandler
    {

        public static string DataServerHost { get; set; }


        public static string DataServerPort { get; set; }

        public static string DigiMapHost { get; set; }

        public static string DigiMapPort { get; set; }
        public static string DigitalMapConnection { get; set; }

        public static string TestRoadQuery { get; set; }

        public static double Longitude1 { get; set; }

        public static double Latitude1 { get; set; }

        public static double Longitude2 { get; set; }

        public static double Latitude2 { get; set; }

        //public static string RoadRef { get; set; }

        public static string CustomRoadParameters { get; set; }

        public static float DPTolerance { get; set; }

        public static int LangRegionSize { get; set; }

        public static string SimplificationMethod { get; set; } = typeof(DouglasPeuckerRoadSectionSimplification).Name;

        public static string CurvetureCalcMethod { get; set; } = typeof(CircumcircleRoadCircleCurvesResolver).Name;

        public static string MaxSpeedCalcMethod { get; set; } = typeof(SimpleSpeedCalculatorBasedOnCurvature).Name;

        public static string RoadStateFetcherImplementation { get; set; } = typeof(DummyRoadStateFetcher).Name;

        public static float CurvatureTreshold { get; set; }

        public static float CarDistanceSkipTreshold { get; set; }

        public static string CurveCollisionCalculator { get; set; }

        public static string StraightCollisionCalculator { get; set; }

        public static string MaxParallelism { get; set; }

        public static int KeepAliveFrequency { get; set; }

        public static string RoadGroupByAttribute { get; set; } = "Ref";

        public static float IntegrationModuleInterval { get; set; }

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

        private static bool IsFactory(Type t)
        {
            while (t != null)
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(AbstractCalculatorFactory<,>))
                {
                    return true;
                }
                t = t.BaseType;
            }
            return false;
        }

        public static HandlerSetupConfig GenerateHandlerSetupConfig()
        {
            return new HandlerSetupConfig(SimplificationMethod , CurvetureCalcMethod);
        }

        // Load all type implementations for constructors at the beginig of execution
        public static void InitConstructors()
        {
            Stopwatch sw = Stopwatch.StartNew();
            //System.Collections.Generic.List<Type> factoriesToInit = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(p => typeof(AbstractCalculatorFactory<,>).IsAssignableFrom(p)).ToList();

            System.Collections.Generic.List<Type> factoriesToInit = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(p => IsFactory(p)).ToList();


            foreach (Type factory in factoriesToInit)
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(factory.TypeHandle);
            }

            Console.WriteLine("Factory init took " + sw.ElapsedMilliseconds);
        }

        public static void RecalculateTestRoadQuery(bool skipCustomParams, AreaMessage msg)
        {
            TestRoadQuery = (skipCustomParams || String.IsNullOrEmpty(CustomRoadParameters) ? "?" : $"?{CustomRoadParameters}&") +
                $"long1={msg.TopLeft.Lon.ToString().Replace(",",".")}" +
                $"&lat1={msg.TopLeft.Lat.ToString().Replace(",", ".")}" +
                $"&long2={msg.BottomRight.Lon.ToString().Replace(",", ".")}" +
                $"&lat2={msg.BottomRight.Lat.ToString().Replace(",", ".")}";
        }

        private static string CreateDigiMapUrl()
        {
            string mapAddr = LoadVariable("DIGIMAP_URL") ?? ("http://" +
                (Uri.CheckHostName(DigiMapHost).Equals(UriHostNameType.Dns) ?
                Dns.GetHostEntry(DigiMapHost).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork) :
                IPAddress.Parse(DigiMapHost))
                + ":" + DigiMapPort);

            Console.WriteLine(mapAddr);

            return mapAddr;
        }

        public static void LoadConfiguration()
        {
            try
            {

                DataServerHost = LoadVariable("DATASERVER_HOST");
                DataServerPort = LoadVariable("DATASERVER_PORT");
                DigiMapHost = LoadVariable("DIGIMAP_HOST");
                DigiMapPort = LoadVariable("DIGIMAP_PORT");
                DigitalMapConnection = CreateDigiMapUrl();
                Longitude1 = float.Parse(LoadVariable("RoadQueryLong1"), CultureInfo.InvariantCulture);
                Latitude1 = float.Parse(LoadVariable("RoadQueryLat1"), CultureInfo.InvariantCulture);
                Longitude2 = float.Parse(LoadVariable("RoadQueryLong2"), CultureInfo.InvariantCulture);
                Latitude2 = float.Parse(LoadVariable("RoadQueryLat2"), CultureInfo.InvariantCulture);
                CustomRoadParameters = LoadVariable("CustomRoadParameters");
                TestRoadQuery =
                    (String.IsNullOrEmpty(CustomRoadParameters) ? "?" : $"?{CustomRoadParameters}&") +
                    $"long1={ConfigurationManager.AppSettings.Get("RoadQueryLong1")}" +
                    $"&lat1={ConfigurationManager.AppSettings.Get("RoadQueryLat1")}" +
                    $"&long2={ConfigurationManager.AppSettings.Get("RoadQueryLong2")}" +
                    $"&lat2={ConfigurationManager.AppSettings.Get("RoadQueryLat2")}";
                DPTolerance = float.Parse(LoadVariable("LINE_SIMPLIFICATION_TOLERANCE"), CultureInfo.InvariantCulture);
                CurvatureTreshold = float.Parse(LoadVariable("CURVATURE_TRESHOLD"), CultureInfo.InvariantCulture);
                CarDistanceSkipTreshold = float.Parse(LoadVariable("CAR_DISTANCE_SKIP_TRESHOLD"), CultureInfo.InvariantCulture);
                CurveCollisionCalculator = LoadVariable("CURVE_COLLISION_CALCULATOR");
                StraightCollisionCalculator = LoadVariable("STRAIGHT_COLLISION_CALCULATOR");
                //SimplificationMethod = (SimplMethods)Enum.Parse(typeof(SimplMethods), ConfigurationManager.AppSettings.Get("SimplificationMethod"));
                //CurvetureCalcMethod = (CurvCalcMethods)Enum.Parse(typeof(CurvCalcMethods), ConfigurationManager.AppSettings.Get("CurvetureCalcMethod"));
                SimplificationMethod = LoadVariable("SIMPLIFICATION_METHOD");
                CurvetureCalcMethod = LoadVariable("CURVETURE_CALC_METHOD");
                MaxSpeedCalcMethod = LoadVariable("MAX_SPEED_CALC_METHOD");
                RoadStateFetcherImplementation = LoadVariable("ROAD_STATE_FETCHER");
                MaxParallelism = LoadVariable("MAX_PARALLELISM");
                KeepAliveFrequency = int.Parse(LoadVariable("KEEP_ALIVE_FREQUENCY") ?? "0", CultureInfo.InvariantCulture);
                RoadGroupByAttribute = LoadVariable("RoadGroupByAttribute");
                IntegrationModuleInterval = float.Parse(LoadVariable("INTEGRATION_MODULE_INTERVAL") ?? "0.2", CultureInfo.InvariantCulture);
                if (String.IsNullOrEmpty(RoadGroupByAttribute))
                {
                    RoadGroupByAttribute = "Ref";
                }
                InitConstructors();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured during config parsing");
                Console.WriteLine(e);
            }
        }

        public static string LoadVariable(string variable)
        {

            string val = LoadEnvironmentVariable(variable) ?? LoadConfigurationVariable(variable);
            Console.WriteLine("Loaded variable " + variable + " with value " + val);
            return LoadEnvironmentVariable(variable) ?? LoadConfigurationVariable(variable);
        }

        public static string LoadEnvironmentVariable(string variable)
        {
            string envVar = Environment.GetEnvironmentVariable(variable);
            return envVar == null || envVar == "" ? null : envVar;
        }

        public static string LoadConfigurationVariable(string variable)
        {
            return ConfigurationManager.AppSettings.Get(variable);
        }

    }
}
