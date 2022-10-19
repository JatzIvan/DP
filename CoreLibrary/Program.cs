using ConsoleApp1.Api;
using ConsoleApp2.RoadSectionHandling;
using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebSocketLibrary;

namespace ConsoleApp2
{
    class Program
    {

        static void Main(string[] args)
        {

            ApplicationConfigurationHandler.LoadConfiguration();

            ApiHelper.InitializeClient();

            RoadDataHandler roadHandler = new RoadDataHandler("a", "a");

            UdpSocketClientImplementation ws = WebSocketManagerFactory.GetInstance().CreateConnection(ApplicationConfigurationHandler.DataServerHost, ApplicationConfigurationHandler.DataServerPort, "LocalServer");



            Dictionary<LocationPoint, AbstractRoadModel>  data = roadHandler.GetParsedRoadData(ApplicationConfigurationHandler.GenerateHandlerSetupConfig());

            WebSocketMessageHandler<ICollisionDetector> observer = new WebSocketMessageHandler<ICollisionDetector>("handler1", new CustomCollisionDetector(data));

            observer.Subscribe(ws);

            while (true)
            {
                if (ws.CreateConnectionWithDataSocket())
                {
                    break;
                }
                Console.WriteLine("Repeat");
                Thread.Sleep(10000);
            }

            Console.WriteLine("Out");
            /*ws.StartListening();*/

            //new RoadDataHandler("a", "a");
            //RoadVisualisation.OpenWindow();

            while (true)
            {
                Console.ReadKey();
            }

        }
    }
}
