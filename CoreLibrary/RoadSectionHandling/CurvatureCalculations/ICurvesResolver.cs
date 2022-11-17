using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.Data
{
    public interface ICurvesResolver
    {
        /**
         * Method is used to calculate "curvature" between points on map
         */
        public List<AbstractRoadModel> CalculateCurvesForWays();

    }
}
