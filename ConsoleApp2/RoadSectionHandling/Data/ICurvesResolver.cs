using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling.Data
{
    interface ICurvesResolver
    {

        public Dictionary<LocationPoint, AbstractRoadModel> CalculateCurvesForWays();

    }
}
