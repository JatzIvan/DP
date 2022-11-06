using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.RoadSimplificators
{
    public class LangConfig : AbstractSimplificationModel
    {
        public float Tolerance { get; set; }

        public int RegionSize { get; set; }

        public LangConfig(float tolerance, int regionSize)
        {
            this.Tolerance = tolerance;
            this.RegionSize = regionSize;
        }

    }
}
