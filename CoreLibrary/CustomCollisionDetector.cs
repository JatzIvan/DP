using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary;
using WebSocketLibrary.Models;

namespace CoreLibrary
{
    class CustomCollisionDetector : AbstractCollisionDetector
    {

        public CustomCollisionDetector(Dictionary<LocationPoint, AbstractRoadModel> currectRoadModel) : base(currectRoadModel)
        {

        }

        public override void PerformCalculations(List<VehicleData> data)
        {
            foreach (VehicleData car in data)
            {
                Console.WriteLine("Latitude: " + car.Position.Lat + " ,Longitude:" + car.Position.Lon + " ,Velocity:" + car.Speed + " ,Orientation:" + car.Heading);

            }
        }
    }
}
