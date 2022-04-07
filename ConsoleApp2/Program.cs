using ConsoleApp1.Api;
using ConsoleApp2.RoadSectionHandling;
using System;
using WebSocketLibrary;

namespace ConsoleApp2
{
    class Program
    {

        static void Main(string[] args)
        {

            ApplicationConfigurationHandler.LoadConfiguration();

            ApiHelper.InitializeClient(ApplicationConfigurationHandler.DigitalMapConnection);

            WebSocketImplementation ws = WebSocketManagerFactory.GetInstance().CreateConnection(ApplicationConfigurationHandler.DataServer, "LocalServer");

            WebSocketMessageHandler<CollisionDetector> observer = new WebSocketMessageHandler<CollisionDetector>("handler1", new CustomCollisionDetector());

            observer.Subscribe(ws);

            new RoadDataHandler("a", "a");

            while (true)
            {
                Console.ReadKey();
            }

        }
    }
}
