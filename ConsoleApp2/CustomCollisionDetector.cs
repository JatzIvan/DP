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
            //Console.WriteLine("Latitude: " + data[0].Lat + " ,Longitude:" + data[0].Lon + " ,Velocity:" + data[0].Vel + " ,Orientation:" + data[0].Orientation);

        }
    }
}
