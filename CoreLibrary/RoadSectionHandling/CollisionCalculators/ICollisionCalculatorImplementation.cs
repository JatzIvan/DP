using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;
using NetTopologySuite.Index.KdTree;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators
{
    public interface ICollisionCalculatorImplementation
    {
        /**
         * Method determines if collision between cars happens based on currect behaviour
         * If method returns null, there is no collision
         * When CollisionInfo is returned, it should be provided to both vehicles
         */
        public AbstractRoadModel PerformCollisionCalculations(VehicleData vehicle1, VehicleData vehicle2, KdTree<AbstractRoadModel> currectRoadModel);

        public double CalculateTTC();

        public bool CollisionOccured();

        public CollisionSeverity GetCollisionSeverity();

        public enum CollisionSeverity
        {
            MEDIUM,
            SEVERE
        }

        /**
         * We calculate for head-on collisions but we can extend this implementation later
         */
        public enum CollisionType
        {

        }

        public WarningMessage CreateWarningMessage(VehicleData vehicle)
        {
            WarningMessage msg = new WarningMessage();

            msg.Index = new Random().Next();
            msg.VehicleId = vehicle.Id;
            msg.TimeToCollision = CalculateTTC();
            msg.CollisionSeverity = GetCollisionSeverity().ToString();
            msg.CollisionType = "headon";

            return msg;
        }



    }
}
