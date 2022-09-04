using ConsoleApp1.Api;
using ConsoleApp2.RoadSectionHandling;
using System;
using System.Threading;
using WebSocketLibrary;

namespace ConsoleApp2
{
    class Program
    {

        static void Main(string[] args)
        {

            ApplicationConfigurationHandler.LoadConfiguration();

            ApiHelper.InitializeClient(ApplicationConfigurationHandler.DigitalMapConnection);

            UdpSocketClientImplementation ws = WebSocketManagerFactory.GetInstance().CreateConnection(ApplicationConfigurationHandler.DataServerHost, ApplicationConfigurationHandler.DataServerPort, "LocalServer");

            WebSocketMessageHandler<ICollisionDetector> observer = new WebSocketMessageHandler<ICollisionDetector>("handler1", new CustomCollisionDetector());

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
