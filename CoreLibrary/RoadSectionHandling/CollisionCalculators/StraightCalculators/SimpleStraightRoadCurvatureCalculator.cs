using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;
using NetTopologySuite.Index.KdTree;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators
{
    [CollisionType(CollisionTypeEnum.STRAIGHT)]
    public class SimpleStraightRoadCurvatureCalculator : ICollisionCalculatorImplementation
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
