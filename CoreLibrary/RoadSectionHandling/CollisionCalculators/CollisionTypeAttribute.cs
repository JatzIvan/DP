using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators
{
    public class CollisionTypeAttribute: AbstractCalculatorAttribute
    {
        
        public CollisionTypeEnum customType { get; set; }

        public CollisionTypeAttribute(CollisionTypeEnum type): base(type.ToString())
        {
            this.customType = type;
        }

    }

    public enum CollisionTypeEnum
    {
        STRAIGHT,
        CURVATURE
    }
}
