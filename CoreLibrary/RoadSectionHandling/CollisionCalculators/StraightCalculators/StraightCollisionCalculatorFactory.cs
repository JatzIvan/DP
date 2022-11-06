using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators
{
    class StraightCollisionCalculatorFactory : ICollisionCalculatorFactory
    {

        private static ICollisionCalculatorFactory INSTANCE;
        private static List<Type> straightCollisionTypes = new List<Type>();

        // Initialize before anything to be ready to use
        static StraightCollisionCalculatorFactory()
        {
            LoadImplementationTypes();
        }

        private static void LoadImplementationTypes()
        {
            Type calculatorType = typeof(ICollisionCalculatorImplementation);

            // Some reflection magic to read all "calculators" from current namespace
            // TODO test this with functional implementation
            straightCollisionTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == typeof(StraightCollisionCalculatorFactory).Namespace)
                .Where(p => calculatorType.IsAssignableFrom(p)).ToList();

            // Some more magic to create instances of gathered types (we do not need to provide any data to constructors)
            //implKlazzes.ForEach(klazz => straightCollisionImplementations.Add((ICollisionCalculatorImplementation)Activator.CreateInstance(klazz)));

        }
        private StraightCollisionCalculatorFactory()
        {

        }

        public static ICollisionCalculatorFactory GetInstance()
        {
            if(INSTANCE == null)
            {
                INSTANCE = new StraightCollisionCalculatorFactory();
            }

            return INSTANCE;

        }

        public static void SetInstance(ICollisionCalculatorFactory newInstance)
        {
            if(INSTANCE != null)
            {
                Console.WriteLine("Warning: Rewriting existing Factory instance " + INSTANCE.GetType() + " with " + newInstance.GetType());
            }
            INSTANCE = newInstance;
        }

        // Choose from list by config value or else return Dummy implementation
        // Always create new implementation because we will work with multiple Threads
        public ICollisionCalculatorImplementation GetImplementation()
        {

            Type foundType = straightCollisionTypes.Where(i => i.Name.Equals(ApplicationConfigurationHandler.StraightCollisionCalculator))
                .FirstOrDefault();

            if(foundType == null)
            {
                foundType = typeof(SimpleStraightRoadCurvatureCalculator);
            }

            return (ICollisionCalculatorImplementation) Activator.CreateInstance(foundType);

        }
    }
}
