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

        // https://www.researchgate.net/figure/Friction-coefficients-for-varying-types-of-road-surfaces-in-satisfactory-condition_tbl1_330012787
        public double GetFriction()
        {

            //if(Temperature < 0 && Humidity > )

            return 0.5;

            switch (Type)
            {
                case RoadType.Asphalt:
                    return 0.75;
                case RoadType.Gravel:
                    return 0.65;
                case RoadType.Unsurfaced:
                    return 0.55;
                default:
                    return 0.75;
            }
        }

    }

    enum RoadType
    {
        Asphalt,
        Gravel,
        Unsurfaced
    }
}
