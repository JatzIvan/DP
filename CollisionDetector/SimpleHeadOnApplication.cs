using CoreLibrary;
using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketLibrary.SocketImplementations;
using WebSocketLibrary;

namespace CollisionDetector
{
    internal class SimpleHeadOnApplication : AbstractApplication
    {
        public override void MainLogicStartup()
        {
            UDPSocketForAreaHandling ws2 = WebSocketManagerFactory.GetInstance()
                .CreateConnection<UDPSocketForAreaHandling, AreaObserverWrapper>(ApplicationConfigurationHandler.DataServerHost, ApplicationConfigurationHandler.DataServerPort, new Random().Next(), new List<IObserver<AreaObserverWrapper>>());

            ApplicationConfigurationHandler.RecalculateTestRoadQuery(false, ws2.AreaFetched());

            WebSocketManagerFactory.GetInstance().CloseConnection(ws2);

            Dictionary<string, List<RoadPointModel>> sections = RoadDataFetcher.GetInstance().GetRoadFromAPIGroupedByRef();

            foreach (KeyValuePair<string, List<RoadPointModel>> section in sections)
            {

                //RoadDataHandler roadHandler = new RoadDataHandler(section.Key, section.Key);
                RoadDataHandler roadHandler = RoadDataManager.GetInstance().AddDataHandler(section.Key, section.Key);


                IObserver<VehicleObserverWrapper> observer = new WebSocketMessageHandler<VehicleObserverWrapper>("handler1", new CustomCollisionDataHandler(roadHandler));

                // IObserver<VehicleObserverWrapper> observer = new WebSocketMessageHandler<VehicleObserverWrapper>("handler1", new JustPrintCollisionDataHandler());

                WebSocketManagerFactory.GetInstance()
                    .CreateConnection<UdpSocketForCarConnection, VehicleObserverWrapper>(ApplicationConfigurationHandler.DataServerHost, ApplicationConfigurationHandler.DataServerPort, new Random().Next(), new List<IObserver<VehicleObserverWrapper>> { observer });

            }
        }
    }
}
