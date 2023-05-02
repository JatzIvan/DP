using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.RoadSectionHandling.RoadParameters
{
    [RoadDataFetcher]
    internal class DummyRoadStateFetcher: GenericRoadStateFetcher
    {

        public DummyRoadStateFetcher() : base()
        {

        }

        public override RoadParameters FetchRoadParameters(LocationPoint point)
        {

            Console.WriteLine("Dummy implementation, returning empty object");

            return new RoadParameters();

        }

        public override string GenerateUrl(LocationPoint point)
        {
            return "";
        }

    }
}
