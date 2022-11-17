using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary;
using WebSocketLibrary.Models;

namespace CoreLibrary
{
    class CustomCollisionDetector : AbstractCollisionDetector
    {

        public CustomCollisionDetector(KdTree<AbstractRoadModel> currectRoadModel) : base(currectRoadModel)
        {

        }

        public override void PerformCalculations(ObserverWrapper data)
        {
            foreach (VehicleData car in data.Data)
            {
                Console.WriteLine("Latitude: " + car.Position.Lat + " ,Longitude:" + car.Position.Lon + " ,Velocity:" + car.Speed + " ,Orientation:" + car.Heading);

            }
        }
    }
}
