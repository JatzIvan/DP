using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling.CollisionCalculators
{
    public interface ICollisionCalculatorFactory
    {
        public ICollisionCalculatorImplementation GetImplementation();

    }
}
