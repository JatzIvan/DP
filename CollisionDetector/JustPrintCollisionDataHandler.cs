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
        public JustPrintCollisionDataHandler() : base(null)
        {

        }

        public override void PerformActions(VehicleObserverWrapper data)
        {

            List<VehicleData> vehicles = data.Data.Vehicles;

            Console.WriteLine(vehicles.Count);

            foreach (VehicleData veh in vehicles)
            {
                Console.WriteLine("Vehicle with Id: " + veh.Id + ", Long: " + veh.Position.Lon + ", Lat: " + veh.Position.Lat);
            }
        }
    }
}
