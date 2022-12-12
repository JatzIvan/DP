using CoreLibrary.RoadSectionHandling.CurvatureCalculations;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.Data
{
    public class RoadCurvitureCalculatorFactory: AbstractCalculatorFactory
    {

        //private static List<Type> loadedCalculators;

        private static List<ValueTuple<Type, Func<List<AbstractRoadModel>, object>>> loadedCalculators = new List<ValueTuple<Type, Func<List<AbstractRoadModel>, object>>>();
        private static ValueTuple<Type, Func<List<AbstractRoadModel>, object>> defaultConstructor;

        private RoadCurvitureCalculatorFactory()
        {

        }

        static RoadCurvitureCalculatorFactory()
        {
            //loadedCalculators = LoadImplementations(new CurvatureResolverAttribute(), typeof(ICurvesResolver));
            List<Type> calculators = LoadImplementations(new CurvatureResolverAttribute(), typeof(ICurvesResolver));
            foreach (Type calculator in calculators)
            {
                loadedCalculators.Add((calculator, CreateCreator<List<AbstractRoadModel>>(calculator)));
            }

            defaultConstructor = (typeof(CircumcircleRoadCircleCurvesResolver), CreateCreator<List<AbstractRoadModel>>(typeof(CircumcircleRoadCircleCurvesResolver)));

        }

        public override List<Type> GetLoadedTypes()
        {
            return loadedCalculators.Select(_ => _.Item1).ToList();
        }

        private static RoadCurvitureCalculatorFactory INSTANCE;

        public static RoadCurvitureCalculatorFactory getInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new RoadCurvitureCalculatorFactory();
            }

            return INSTANCE;
        }

        /**
         * TODO: Try something like annotations later
         */
        public ICurvesResolver GetResolverImplementation(string type, List<AbstractRoadModel> connectedWays)
        {

            ValueTuple<Type, Func<List<AbstractRoadModel>, object>> foundType = loadedCalculators.Where(i => i.Item1.Name.Equals(type))
                .FirstOrDefault();

            if (foundType.Equals(default(ValueTuple<Type, Func<object[], object>>)))
            {
                foundType = defaultConstructor;
            }

            return (ICurvesResolver)foundType.Item2(connectedWays);

        }

    }
}
