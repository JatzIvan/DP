using ConsoleApp2.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;

namespace ConsoleApp2.RoadSectionHandling.CollisionCalculators.StraightCalculators
{
    public class SimpleStraightRoadCurvatureCalculator : ICollisionCalculatorImplementation
    {
        public float CalculateTTC()
        {
            throw new NotImplementedException();
        }

        public bool CollisionOccured()
        {
            throw new NotImplementedException();
        }

        public ICollisionCalculatorImplementation.CollisionSeverity GetCollisionSeverity()
        {
            throw new NotImplementedException();
        }

        public CollisionInfo PerformCollisionCalculations(VehicleData vehicle1, VehicleData vehicle2, Dictionary<LocationPoint, AbstractRoadModel> currectRoadModel)
        {
            throw new NotImplementedException();
        }
    }
}
