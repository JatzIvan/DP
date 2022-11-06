using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.CurveCalculators
{
    class CurveCollisionCalculatorFactory : ICollisionCalculatorFactory
    {
        private static ICollisionCalculatorFactory INSTANCE;

        private static List<Type> curveCollisionImplementations = new List<Type>();

        // Initialize before anything to be ready to use
        static CurveCollisionCalculatorFactory(){
            LoadImplementations();
        }

        private static void LoadImplementations()
        {
            Type calculatorType = typeof(ICollisionCalculatorImplementation);

            // Some reflection magic to read all "calculators" from current namespace
            // TODO test this with functional implementation
            curveCollisionImplementations = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == typeof(CurveCollisionCalculatorFactory).Namespace)
                .Where(p => calculatorType.IsAssignableFrom(p)).ToList();

            // Some more magic to create instances of gathered types (we do not need to provide any data to constructors)
            //implKlazzes.ForEach(klazz => curveCollisionImplementations.Add((ICollisionCalculatorImplementation)Activator.CreateInstance(klazz)));

        }

        private CurveCollisionCalculatorFactory()
        {

        }

        public static ICollisionCalculatorFactory GetInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new CurveCollisionCalculatorFactory();
            }

            return INSTANCE;

        }

        public static void SetInstance(ICollisionCalculatorFactory newInstance)
        {
            if (INSTANCE != null)
            {
                Console.WriteLine("Warning: Rewriting existing Factory instance " + INSTANCE.GetType() + " with " + newInstance.GetType());
            }
            INSTANCE = newInstance;
        }

        // Choose from list by config value or else return Dummy implementation
        public ICollisionCalculatorImplementation GetImplementation()
        {

            Type foundType = curveCollisionImplementations.Where(i => i.GetType().Name.Equals(ApplicationConfigurationHandler.CurveCollisionCalculator))
                .FirstOrDefault();

            if (foundType == null)
            {
                foundType = typeof(SimpleCurveRoadCurvatureCalculator);
            }

            return (ICollisionCalculatorImplementation) Activator.CreateInstance(foundType);

        }
    }
}
