using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.MaxSpeedCalculations
{
    [MaxSpeedCalculator]
    public class SimpleSpeedCalculatorBasedOnCurvature : ISpeedCalculator
    {
        // Maximum superelevation rate by Glennon, John C., State of the Art Related to Safety Criteria for Highway Curve Design, Texas Transportation
        // Institute, Research Report No. 134-4, November, 1969.
        double e = 0.1;

        // Kanellaidis G., Dimitropulos I., (1995), Investigation of current and proposed superelevation design
        // practices on roadway curves, TRB(Transportation Research Board), International Symposium on highway
        // geometric design practises, Boston, Massachussetts, pp. 2-3. 
        double f = 0.15;

        public void CalcMaxSpeedsForRoadSegment(List<AbstractRoadModel> points)
        {
            foreach(AbstractRoadModel point in points)
            {

                point.MaxSpeed = GetMaxSpeed(point);

            }
        }

        // Get max speed in m/s
        public double GetMaxSpeed(AbstractRoadModel point)
        {

            double DDC = (30.49 * 180) / (point.RadiusOfCircle * Math.PI);
            
            // Output speed is in miles per hour
            double Vd = Math.Sqrt((85660 * (e + f)) / DDC);

            return (Vd * 1.6) / 3.6;
        }
    }
}
