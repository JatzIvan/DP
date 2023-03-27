using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLibrary.RoadSectionHandling.RoadParameters
{
    public class RoadDataFetcherFactory : AbstractCalculatorFactory<GenericRoadStateFetcher, RoadDataFetcherAttribute>
    {

/*        private static List<ValueTuple<Type, Func<LocationPoint, object>>> loadedCalculators = new List<ValueTuple<Type, Func<LocationPoint, object>>>();
        private static ValueTuple<Type, Func<LocationPoint, object>> defaultConstructor;*/

        private RoadDataFetcherFactory(): base(new RoadDataFetcherAttribute())
        {

        }

        static RoadDataFetcherFactory()
        {
            GetInstance();
        }

        private static RoadDataFetcherFactory INSTANCE;

        public static RoadDataFetcherFactory GetInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new RoadDataFetcherFactory();
            }

            return INSTANCE;
        }

        public override (Type, Func<object>) GetDefaultInstance()
        {
            return (typeof(DummyRoadStateFetcher), CreateCreator(typeof(DummyRoadStateFetcher)));
        }

    }
}
