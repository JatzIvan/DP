using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.RoadParameters
{
    // Simple POCO which holds current state of road
    public class RoadParameters
    {

        public float Humidity { get; set; } = 0;
        public float Temperature { get; set; } = 0;

        public float Pressure { get; set; } = 0;

        RoadType Type { get; set; } = RoadType.Asphalt;

        public double Friction { get; set; } = 0.5;

        // https://www.researchgate.net/figure/Friction-coefficients-for-varying-types-of-road-surfaces-in-satisfactory-condition_tbl1_330012787
        public double GetFriction()
        {

            //if(Temperature < 0 && Humidity > )

            // Development of an approach to determination of coupling qualities of road covering using weather-climate factor
            if (Temperature <= -10) 
            {
                return 0.4;
            }else if(Temperature <= -5)
            {
                return 0.3;
            }else if(Temperature <= -3)
            {
                return 0.2;
            }else if (Temperature <= 0)
            {
                return 0.1;
            }else if(Temperature <= 1) 
            {
                return 0.3;
            }else if(Temperature <= 2) 
            {
                return 0.5;
            }else if(Temperature <= 5)
            {
                return 0.6;
            }else
            {
                return 0.7;
            }

            /*return 0.5;*/

/*            switch (Type)
            {
                case RoadType.Asphalt:
                    return 0.75;
                case RoadType.Gravel:
                    return 0.65;
                case RoadType.Unsurfaced:
                    return 0.55;
                default:
                    return 0.75;
            }*/



        }

    }

    enum RoadType
    {
        Asphalt,
        Gravel,
        Unsurfaced
    }

    enum SurfaceCondition
    {
        Dry,
        Wet,
        Snow,
        Ice
    }

}
