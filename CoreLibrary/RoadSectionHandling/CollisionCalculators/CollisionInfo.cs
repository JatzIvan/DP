using System;
using System.Collections.Generic;
using System.Text;
using static CoreLibrary.RoadSectionHandling.CollisionCalculators.ICollisionCalculatorImplementation;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators
{
    public class CollisionInfo
    {
        public float TimeToCollision { get; set; }
        public CollisionSeverity CollSeverity { get; set; }

    }
}
