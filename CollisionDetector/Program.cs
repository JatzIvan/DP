using ConsoleApp1.Api;
using ConsoleApp2;
using ConsoleApp2.RoadSectionHandling;
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

            RoadDataHandler roadHandler = new RoadDataHandler("a", "a");

            UdpSocketClientImplementation ws = WebSocketManagerFactory.GetInstance().CreateConnection(ApplicationConfigurationHandler.DataServerHost, ApplicationConfigurationHandler.DataServerPort, new Random().Next());

            //Dictionary<LocationPoint, AbstractRoadModel>  data = roadHandler.GetParsedRoadData(ApplicationConfigurationHandler.GenerateHandlerSetupConfig());

            WebSocketMessageHandler<ICollisionDetector> observer = new WebSocketMessageHandler<ICollisionDetector>("handler1", null);

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
