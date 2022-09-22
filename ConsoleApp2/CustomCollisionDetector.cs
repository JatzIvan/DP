using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary;
using WebSocketLibrary.Models;

namespace ConsoleApp2
{
    class CustomCollisionDetector : ICollisionDetector
    {
        public void PerformCalculations(List<CarUpdateInfo> data)
        {
            foreach(CarUpdateInfo segment in data)
            {
                foreach(VehicleData car in segment.Vehicles)
                {
                    Console.WriteLine("Latitude: " + car.Position.Lat + " ,Longitude:" + car.Position.Lon + " ,Velocity:" + car.Speed+ " ,Orientation:" + car.Heading);

                }
            }
        }

        public void ResolveCollisionCalculationBasedOnCurvature(VehicleData vehicle1, VehicleData vehicle2)
        {
            throw new NotImplementedException();
        }
    }
}
