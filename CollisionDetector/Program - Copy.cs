using ConsoleApp1.Api;
using CoreLibrary;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators;
using CoreLibrary.RoadSectionHandling.Model;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Threading;
using WebSocketLibrary;
using WebSocketLibrary.Models;

namespace CollisionDetector
{
    [CollisionType(CollisionTypeEnum.CURVATURE)]
    public class TestXX : ICollisionCalculatorImplementation
    {
        public double CalculateTTC()
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

        public AbstractRoadModel PerformCollisionCalculations(VehicleData vehicle1, VehicleData vehicle2, KdTree<AbstractRoadModel> currectRoadModel)
        {
            throw new NotImplementedException();
        }
    }
}
