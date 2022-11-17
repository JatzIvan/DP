using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.MaxSpeedCalculations
{
    public class MaxSpeedCalculatorAttribute : AbstractCalculatorAttribute
    {
        public MaxSpeedCalculatorAttribute() : base("MAX_SPEED_CALCULATOR_IMPL")
        {
        }
    }
}
