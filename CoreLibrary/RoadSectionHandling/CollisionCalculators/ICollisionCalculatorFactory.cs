using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators
{
    public interface ICollisionCalculatorFactory
    {
        public ICollisionCalculatorImplementation GetImplementation();

    }
}
