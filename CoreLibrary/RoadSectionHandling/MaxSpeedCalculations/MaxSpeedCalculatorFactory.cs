using CoreLibrary.RoadSectionHandling.MaxSpeedCalculations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.MaxSpeedCalculators
{
    class MaxSpeedCalculatorFactory : AbstractCalculatorFactory
    {

        //private static List<Type> loadedCalculators;

        private static List<ValueTuple<Type, Func<object>>> loadedCalculators = new List<ValueTuple<Type, Func<object>>>();
        private static ValueTuple<Type, Func<object>> defaultConstructor;

        private static MaxSpeedCalculatorFactory INSTANCE { get; set; }

        private MaxSpeedCalculatorFactory()
        {
        }

        static MaxSpeedCalculatorFactory()
        {
            //loadedCalculators = LoadImplementations(new MaxSpeedCalculatorAttribute(), typeof(ISpeedCalculator));

            List<Type> calculators = LoadImplementations(new MaxSpeedCalculatorAttribute(), typeof(ISpeedCalculator));
            foreach (Type calculator in calculators)
            {
                loadedCalculators.Add((calculator, CreateCreator(calculator)));
            }

            defaultConstructor = (typeof(SimpleSpeedCalculatorBasedOnCurvature), CreateCreator(typeof(SimpleSpeedCalculatorBasedOnCurvature)));


        }

        public override List<Type> GetLoadedTypes()
        {
            return loadedCalculators.Select(_ => _.Item1).ToList();
        }

        public static MaxSpeedCalculatorFactory GetInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new MaxSpeedCalculatorFactory();
            }

            return INSTANCE;

        }

        public static void SetInstance(MaxSpeedCalculatorFactory newInstance)
        {
            if (INSTANCE != null)
            {
                Console.WriteLine("Warning: Rewriting existing Factory instance " + INSTANCE.GetType() + " with " + newInstance.GetType());
            }
            INSTANCE = newInstance;
        }

        // Choose from list by config value or else return Dummy implementation
        // Always create new implementation because we will work with multiple Threads
        public ISpeedCalculator GetImplementation(string type)
        {

            ValueTuple<Type, Func<object>> foundType = loadedCalculators.Where(i => i.Item1.Name.Equals(type))
                .FirstOrDefault();

            if (foundType.Equals(default(ValueTuple<Type, Func<object>>)))
            {
                foundType = defaultConstructor;
            }

            return (ISpeedCalculator)foundType.Item2();

        }

        public ISpeedCalculator GetImplementation()
        {

            return GetImplementation(ApplicationConfigurationHandler.MaxSpeedCalcMethod);

        }

    }
}
