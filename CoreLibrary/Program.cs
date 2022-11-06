using ConsoleApp1.Api;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebSocketLibrary;
using WebSocketLibrary.Models;

namespace CoreLibrary
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

            WebSocketMessageHandler<ICollisionDetector> observer = new WebSocketMessageHandler<ICollisionDetector>("handler1", new CustomCollisionDetector(null));

            WebSocketManagerFactory.GetInstance().OpenConnection(ws, new List<IObserver<List<VehicleData>>> { observer });

           /* while (true)
            {
                if (ws.CreateConnectionWithDataSocket())
                {
                    break;
                }
                Console.WriteLine("Repeat");
                Thread.Sleep(10000);
            }*/

            Console.WriteLine("Out");
            /*ws.StartListening();*/

            //new RoadDataHandler("a", "a");
            //RoadVisualisation.OpenWindow();

            Thread td = new Thread(new SocketConnecterThread().HandlePending);
            td.Start();

            Thread td2 = new Thread(new SocketKeepAlive().KeepAliveActiveConnections);
            td2.Start();

            while (true)
            {
                //WebSocketManagerFactory.GetInstance().KeepAlive();
                //Thread.Sleep(3000);
            }

        }
    }
}
