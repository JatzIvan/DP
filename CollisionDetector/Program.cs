using ConsoleApp1.Api;
using CoreLibrary;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators;
using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WebSocketLibrary;
using WebSocketLibrary.Models;
using WebSocketLibrary.SocketImplementations;
using static System.Net.Mime.MediaTypeNames;

namespace CollisionDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            ApplicationConfigurationHandler.LoadConfiguration();
            
            ApiHelper.InitializeClient();

            //Thread td = new Thread(new SocketConnecterThread().HandlePending);
            //td.Start();

            Thread td2 = new Thread(new SocketKeepAlive().KeepAliveActiveConnections);
            td2.Start();

            Thread td3 = new Thread(new UnacknowledgedMessagesThread().HandleUnresolved);
            td3.Start();

            Console.WriteLine("Simplification tolerance" + ApplicationConfigurationHandler.DPTolerance);
            Console.WriteLine("Curvature treshold" + ApplicationConfigurationHandler.CurvatureTreshold);
            Console.WriteLine("Car skip distance" + ApplicationConfigurationHandler.CarDistanceSkipTreshold);

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

            UDPSocketForAreaHandling ws2 = WebSocketManagerFactory.GetInstance()
                .CreateConnection<UDPSocketForAreaHandling, AreaObserverWrapper>(ApplicationConfigurationHandler.DataServerHost, ApplicationConfigurationHandler.DataServerPort, new Random().Next(), new List<IObserver<AreaObserverWrapper>>());

            ApplicationConfigurationHandler.RecalculateTestRoadQuery(false, ws2.AreaFetched());

            WebSocketManagerFactory.GetInstance().CloseConnection(ws2);
            
            Dictionary<string, List<RoadPointModel>> sections = RoadDataFetcher.GetInstance().GetRoadFromAPIGroupedByRef();
            
            foreach(KeyValuePair<string, List<RoadPointModel>> section in sections)
            {

                //RoadDataHandler roadHandler = new RoadDataHandler(section.Key, section.Key);
                RoadDataHandler roadHandler = RoadDataManager.GetInstance().AddDataHandler(section.Key, section.Key);


                IObserver<VehicleObserverWrapper> observer = new WebSocketMessageHandler<VehicleObserverWrapper>("handler1", new CustomCollisionDataHandler(roadHandler));

                // IObserver<VehicleObserverWrapper> observer = new WebSocketMessageHandler<VehicleObserverWrapper>("handler1", new JustPrintCollisionDataHandler());

                WebSocketManagerFactory.GetInstance()
                    .CreateConnection<UdpSocketForCarConnection, VehicleObserverWrapper>(ApplicationConfigurationHandler.DataServerHost, ApplicationConfigurationHandler.DataServerPort, new Random().Next(), new List<IObserver<VehicleObserverWrapper>> { observer });

            }   


/*            RoadDataHandler roadHandler = new RoadDataHandler("a", "a");


            IObserver<VehicleObserverWrapper> observer = new WebSocketMessageHandler<VehicleObserverWrapper>("handler1", new CustomCollisionDataHandler(roadHandler));

            // IObserver<VehicleObserverWrapper> observer = new WebSocketMessageHandler<VehicleObserverWrapper>("handler1", new JustPrintCollisionDataHandler());


            UdpSocketForCarConnection ws = WebSocketManagerFactory.GetInstance()
                .CreateConnection<UdpSocketForCarConnection, VehicleObserverWrapper>(ApplicationConfigurationHandler.DataServerHost, ApplicationConfigurationHandler.DataServerPort, new Random().Next(), new List<IObserver<VehicleObserverWrapper>> { observer });
*/
   /*         IObserver<VehicleObserverWrapper> observer3 = new WebSocketMessageHandler<VehicleObserverWrapper>("handler1", new CustomCollisionDataHandler(roadHandler));
            UdpSocketForCarConnection ws3 = WebSocketManagerFactory.GetInstance()
                .CreateConnection<UdpSocketForCarConnection, VehicleObserverWrapper>(ApplicationConfigurationHandler.DataServerHost, ApplicationConfigurationHandler.DataServerPort, new Random().Next(), new List<IObserver<VehicleObserverWrapper>> { observer3 });

            IObserver<VehicleObserverWrapper> observer4 = new WebSocketMessageHandler<VehicleObserverWrapper>("handler1", new CustomCollisionDataHandler(roadHandler));
            UdpSocketForCarConnection ws4 = WebSocketManagerFactory.GetInstance()
                .CreateConnection<UdpSocketForCarConnection, VehicleObserverWrapper>(ApplicationConfigurationHandler.DataServerHost, ApplicationConfigurationHandler.DataServerPort, new Random().Next(), new List<IObserver<VehicleObserverWrapper>> { observer4 });

            IObserver<VehicleObserverWrapper> observer5 = new WebSocketMessageHandler<VehicleObserverWrapper>("handler1", new CustomCollisionDataHandler(roadHandler));
            UdpSocketForCarConnection ws5 = WebSocketManagerFactory.GetInstance()
                .CreateConnection<UdpSocketForCarConnection, VehicleObserverWrapper>(ApplicationConfigurationHandler.DataServerHost, ApplicationConfigurationHandler.DataServerPort, new Random().Next(), new List<IObserver<VehicleObserverWrapper>> { observer5 });
   */

            //KdTree<AbstractRoadModel> data = roadHandler.GetParsedRoadData(ApplicationConfigurationHandler.GenerateHandlerSetupConfig());

            //List<(Dictionary<LocationPoint, AbstractRoadModel>, double)> vvv = roadHandler.GatherCurvaturesBetweenVehicles();


            //WebSocketMessageHandler<ICollisionDetector> observer = new WebSocketMessageHandler<ICollisionDetector>("handler1", new JustPrintCollisionDataHandler(null));

            //WebSocketManagerFactory.GetInstance().OpenConnection(ws, new List<IObserver<ObserverWrapper>> { observer });

            Console.WriteLine("Out");


            /*Thread td = new Thread(new SocketConnecterThread().HandlePending);
            td.Start();

            Thread td2 = new Thread(new SocketKeepAlive().KeepAliveActiveConnections);
            td2.Start();*/

            Console.Read();

            /*while (true)
            {
               // Console.WriteLine("Blabla");
                Thread.Sleep(10000);
            }*/
        }
    }
}
