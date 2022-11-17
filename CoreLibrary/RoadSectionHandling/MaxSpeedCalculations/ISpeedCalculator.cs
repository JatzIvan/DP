using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.MaxSpeedCalculations
{
    public interface ISpeedCalculator
    {

        public double GetMaxSpeed(AbstractRoadModel point);

        public void CalcMaxSpeedsForRoadSegment(List<AbstractRoadModel> points);
    }
}
