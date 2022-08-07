using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling.RoadSimplificators
{
    public class DouglasPeuckerConfig : AbstractSimplificationModel
    {
        public float Tolerance { get; set; }

        public DouglasPeuckerConfig(float tolerance)
        {
            this.Tolerance = tolerance;
        }

        public DouglasPeuckerConfig()
        {
        }

    }
}
