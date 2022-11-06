using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.RoadSimplificators
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
