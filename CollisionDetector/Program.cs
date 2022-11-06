using ConsoleApp1.Api;
using CoreLibrary;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using WebSocketLibrary;
using WebSocketLibrary.Models;

namespace CollisionDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            ApplicationConfigurationHandler.LoadConfiguration();

            ApiHelper.InitializeClient(ApplicationConfigurationHandler.DigitalMapConnection);

            // Test

            /*VehicleData veh1 = new VehicleData();
            veh1.Heading = 105.1F;
            veh1.Speed = 4.25F;
            veh1.Position = new PositionWrapper(31.25956982F, 121.61139076F);

            VehicleData veh2 = new VehicleData();
            veh2.Heading = 122.9F;
            veh2.Speed = 3.21F;
            veh2.Position = new PositionWrapper(31.25961488F, 121.61155024F);

            new GPSStraightRoadCurvatureCalculator().PerformCollisionCalculations(veh1, veh2, null);*/

            RoadDataHandler roadHandler = new RoadDataHandler("a", "a");

            UdpSocketClientImplementation ws = WebSocketManagerFactory.GetInstance().CreateConnection(ApplicationConfigurationHandler.DataServerHost, ApplicationConfigurationHandler.DataServerPort, new Random().Next());

            Dictionary<LocationPoint, AbstractRoadModel>  data = roadHandler.GetParsedRoadData(ApplicationConfigurationHandler.GenerateHandlerSetupConfig());

            WebSocketMessageHandler<ICollisionDetector> observer = new WebSocketMessageHandler<ICollisionDetector>("handler1", new CustomCollisionDataHandler(data));

            WebSocketManagerFactory.GetInstance().OpenConnection(ws, new List<IObserver<List<VehicleData>>> { observer });

            Console.WriteLine("Out");


            Thread td = new Thread(new SocketConnecterThread().HandlePending);
            td.Start();

            Thread td2 = new Thread(new SocketKeepAlive().KeepAliveActiveConnections);
            td2.Start();

            while (true)
            {

            }
        }
    }
}
