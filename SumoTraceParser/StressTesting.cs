using ConsoleApp1.Api;
using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary;
using SumoTraceParser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using System.Diagnostics;
using WebSocketLibrary.Models;
using WebSocketLibrary;
using System.Runtime.InteropServices;
using System.Collections;
using NetTopologySuite.Index.KdTree;

namespace TestingLibrary
{
    public class StressTesting
    {

        protected FcdExport SumoExport;

        Dictionary<string, RoadDataHandler> RoadHandlers = new Dictionary<string, RoadDataHandler>();

        Dictionary<int, List<double>> TimeToCalculateBasedOnNumberOfVehicles = new Dictionary<int, List<double>>();

        CustomCollisionDataHandler handler;

        string OutputPath = "stress_test/";

        public StressTesting(string path, string outputPath)
        {

            using (var reader = new StreamReader(path))
            {
                SumoExport = (FcdExport)new XmlSerializer(typeof(FcdExport)).Deserialize(reader);
            }

            if (SumoExport == null)
            {
                throw new Exception("Parser was unable to open/parse provided file");
            }

            if(outputPath != null && outputPath != "")
            {
                OutputPath = outputPath;
            }

            ApplicationConfigurationHandler.LoadConfiguration();

            ApiHelper.InitializeClient();


            Dictionary<string, List<RoadPointModel>> sections = RoadDataFetcher.GetInstance().GetRoadFromAPIGroupedByAttr();

            foreach (KeyValuePair<string, List<RoadPointModel>> section in sections)
            {

                RoadDataHandler roadHandler = new RoadDataHandler(section.Key, section.Key);
                roadHandler.GetParsedRoadData();
                RoadHandlers.Add(section.Key, roadHandler);
            }

            handler = new CustomCollisionDataHandler(RoadHandlers["503"]);

        }

        private CarUpdateInfo TransformData(List<Vehicle> vehicles)
        {

            CarUpdateInfo carUpdateInfo = new CarUpdateInfo();

            List<VehicleData> data = new List<VehicleData>();

            foreach(Vehicle vehicle in vehicles)
            {
                
                VehicleData vehicleData = new VehicleData();
                vehicleData.Id = 0;
                vehicleData.Speed = vehicle.Speed;
                vehicleData.Heading = vehicle.Angle;
                vehicleData.Position = new PositionWrapper(vehicle.Y, vehicle.X);
                data.Add(vehicleData);

            }
            carUpdateInfo.Vehicles = data;
            return carUpdateInfo;

        }

        public void ProcessDump() 
        {

            foreach(Timestep step in SumoExport.Timestemps)
            { 
                Console.WriteLine("--------------------");
            //Console.WriteLine("Processing step " + step.Time);
            VehicleObserverWrapper data = new VehicleObserverWrapper(TransformData(step.Vehicles), 0);

                Stopwatch sw = Stopwatch.StartNew();

                handler.PerformActions(data);

                double time = sw.Elapsed.TotalMilliseconds;

                Console.WriteLine("Processed " + data.Data.Vehicles.Count + " in " + time);
                Console.WriteLine("--------------------");
                if (!TimeToCalculateBasedOnNumberOfVehicles.ContainsKey(step.Vehicles.Count))
                {
                    TimeToCalculateBasedOnNumberOfVehicles.Add(step.Vehicles.Count, new List<double>());
                }
                TimeToCalculateBasedOnNumberOfVehicles[step.Vehicles.Count].Add(time);
            }

            String csv = String.Join(
            Environment.NewLine,
                TimeToCalculateBasedOnNumberOfVehicles.Select(d => $"{d.Key}|{String.Join(";", d.Value.Select(x => x.ToString()).ToArray())}|")
);
            System.IO.File.WriteAllText(OutputPath + "out_" + ((DateTimeOffset)DateTime.UtcNow).ToString("yyyyMMddHHmmssfff") + ".csv", csv);

        }



        class CustomCollisionDataHandler : AbstractCollisionDetector
        {

            private RoadDataHandler dataStorage;

            public CustomCollisionDataHandler(RoadDataHandler roadHandler) : base(roadHandler.GetParsedRoadData())
            {
               // roadHandler.GatherCurvaturesBetweenVehicles();
                this.dataStorage = roadHandler;
            }

            private bool PushMaxSpeedContent(VehicleData vehicle, NotifyMessage msg, double maxAllowedSpeed)
            {
                if (vehicle.Speed > maxAllowedSpeed)
                {
                 
                    msg.Level = NotificationLevel.danger;
                   
                    msg.Content.MaxSpeedExceededBy = vehicle.Speed - maxAllowedSpeed;

                    return true;
                }

                return false;
            }

            public override void PerformActions(VehicleObserverWrapper data)
            {

                List<VehicleData> vehicles = data.Data.Vehicles;
                if (vehicles.Count >= 2)
                {
                    Stopwatch sw = Stopwatch.StartNew();

                    List<Tuple<VehicleData, AbstractRoadModel>> mappedVehicles = vehicles
                    .Select(veh => (veh, MapParserUtils.ConvertGPStoCartsian(new LocationPoint(veh.Position.Lon, veh.Position.Lat))))
                    .Select(convertedVehicle => new Tuple<VehicleData, AbstractRoadModel>(
                        convertedVehicle.veh, currectRoadModel.NearestNeighbor(new NetTopologySuite.Geometries.CoordinateZ(convertedVehicle.Item2.Item1, convertedVehicle.Item2.Item2, convertedVehicle.Item2.Item3)).Data
                    )).ToList();

                    IEnumerable<IEnumerable<Tuple<VehicleData, AbstractRoadModel>>> pairsToCalc = CreateVehiclePairs(mappedVehicles);

                    Console.WriteLine("Created all pairs in " + sw.ElapsedMilliseconds);
                    // Try threading or something, right now I need to ensure that this concept can work
                   // foreach(var pair in pairsToCalc)
                    Parallel.ForEach(pairsToCalc, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = 6
                    } ,pair =>
                    {
                        ICollisionCalculatorImplementation calcMethod = ResolveCollisionCalculatorBasedOnCurvature(pair.ElementAt(0), pair.ElementAt(1));
                        if (calcMethod != null)
                        {

                            AbstractRoadModel collisionPoint = calcMethod.PerformCollisionCalculations(pair.ElementAt(0), pair.ElementAt(1), currectRoadModel);

                        }
                    });
                }
            }
        }

    }
}
