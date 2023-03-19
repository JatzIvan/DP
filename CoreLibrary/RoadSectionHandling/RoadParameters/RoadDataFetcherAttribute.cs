using CoreLibrary.RoadSectionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.RoadSectionHandling.RoadParameters
{
    public class RoadDataFetcherAttribute : AbstractCalculatorAttribute
    {

        public RoadDataFetcherAttribute() : base("ROAD_STATE_FETCHER_IMPL")
        {

        }

    }
}
