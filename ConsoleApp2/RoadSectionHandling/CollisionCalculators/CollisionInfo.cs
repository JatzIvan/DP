using System;
using System.Collections.Generic;
using System.Text;
using static ConsoleApp2.RoadSectionHandling.CollisionCalculators.ICollisionCalculatorImplementation;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators
{
    public class CollisionInfo
    {
        public float TimeToCollision { get; set; }
        public CollisionSeverity CollSeverity { get; set; }

    }
}
