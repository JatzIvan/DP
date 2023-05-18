using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.RoadParameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.MaxSpeedCalculations
{
    [MaxSpeedCalculator]
    public class SimpleMaxSpeedCalcBasedOnFriction : ISpeedCalculator
    {

        private double g = 9.832;

        public void CalcMaxSpeedsForRoadSegment(List<AbstractRoadModel> points, string roadRef)
        {

            foreach (AbstractRoadModel point in points)
            {

                point.MaxSpeed = GetMaxSpeed(point, roadRef);

                // For Debug purposes
                if(point.MaxSpeed < 10)
                {
                    Logger.GetLogger().WriteLine(point.MaxSpeed + " -- " + point.CurrentLocation.Longitude + "," + point.CurrentLocation.Latitude);
                }
            }
        }

        public double GetMaxSpeed(AbstractRoadModel point, string roadRef)
        {
            return Math.Sqrt(point.RadiusOfCircle * g * RoadParametersHolder.GetInstance().GetParametersForRoad(roadRef, false).GetFriction());
            //return Math.Sqrt(point.RadiusOfCircle * g * 0.5);
        }
    }
}
