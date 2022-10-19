using ConsoleApp2.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;

namespace ConsoleApp2.RoadSectionHandling.CollisionCalculators
{
    public interface ICollisionCalculatorImplementation
    {
        /**
         * Method determines if collision between cars happens based on currect behaviour
         * If method returns null, there is no collision
         * When CollisionInfo is returned, it should be provided to both vehicles
         */
        public CollisionInfo PerformCollisionCalculations(VehicleData vehicle1, VehicleData vehicle2, Dictionary<LocationPoint, AbstractRoadModel> currectRoadModel);

        public float CalculateTTC();

        public bool CollisionOccured();

        public CollisionSeverity GetCollisionSeverity();

        public enum CollisionSeverity
        {

        }

        /**
         * We calculate for head-on collisions but we can extend this implementation later
         */
        public enum CollisionType
        {

        }

    }
}
