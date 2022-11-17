using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary;
using WebSocketLibrary.Models;

namespace CollisionDetector
{
    public class JustPrintCollisionDataHandler : AbstractCollisionDetector
    {
        public JustPrintCollisionDataHandler(KdTree<AbstractRoadModel> currectRoadModel) : base(currectRoadModel)
        {
        }

        public override void PerformCalculations(ObserverWrapper data)
        {
            foreach(VehicleData veh in data.Data)
            {
                Console.WriteLine("Vehicle with Id: " + veh.Id + ", Long: " + veh.Position.Lon + ", Lat: " + veh.Position.Lat);
            }
        }
    }
}
