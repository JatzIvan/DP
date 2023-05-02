using CoreLibrary.RoadSectionHandling.MaxSpeedCalculations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.MaxSpeedCalculators
{
    class MaxSpeedCalculatorFactory : AbstractCalculatorFactory<ISpeedCalculator, MaxSpeedCalculatorAttribute>
    {

        private static MaxSpeedCalculatorFactory INSTANCE { get; set; }

        private MaxSpeedCalculatorFactory(): base(new MaxSpeedCalculatorAttribute())
        {
        }

        static MaxSpeedCalculatorFactory()
        {

            GetInstance();

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

        public ISpeedCalculator GetImplementation()
        {

            return GetResolverImplementation(ApplicationConfigurationHandler.MaxSpeedCalcMethod);

        }

        public override (Type, Func<object>) GetDefaultInstance()
        {
            return (typeof(SimpleSpeedCalculatorBasedOnCurvature), CreateCreator(typeof(SimpleSpeedCalculatorBasedOnCurvature)));
        }
    }
}
