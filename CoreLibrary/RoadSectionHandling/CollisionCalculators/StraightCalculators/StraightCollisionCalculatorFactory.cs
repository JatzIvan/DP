using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators
{
    class StraightCollisionCalculatorFactory : AbstractCalculatorFactory, ICollisionCalculatorFactory
    {

        private static ICollisionCalculatorFactory INSTANCE;
        //private static List<Type> straightCollisionTypes = new List<Type>();

        private static List<ValueTuple<Type, Func<object>>> straightCollisionTypes = new List<ValueTuple<Type, Func<object>>>();
        private static ValueTuple<Type, Func<object>> defaultConstructor;


        // Initialize before anything to be ready to use
        static StraightCollisionCalculatorFactory()
        {
            List<Type> calculators = LoadImplementations(new CollisionTypeAttribute(CollisionTypeEnum.STRAIGHT), typeof(ICollisionCalculatorImplementation));
            foreach (Type calculator in calculators)
            {
                straightCollisionTypes.Add((calculator, CreateCreator(calculator)));
            }

            defaultConstructor = (typeof(SimpleStraightRoadCurvatureCalculator), CreateCreator(typeof(SimpleStraightRoadCurvatureCalculator)));


        }

        public override List<Type> GetLoadedTypes()
        {
            return straightCollisionTypes.Select(_ => _.Item1).ToList();
        }

        /*private static void LoadImplementationTypes()
        {
            Type calculatorType = typeof(ICollisionCalculatorImplementation);

            // Some reflection magic to read all "calculators" from current namespace
            // TODO test this with functional implementation
            straightCollisionTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == typeof(StraightCollisionCalculatorFactory).Namespace)
                .Where(p => calculatorType.IsAssignableFrom(p)).ToList();

            // Some more magic to create instances of gathered types (we do not need to provide any data to constructors)
            //implKlazzes.ForEach(klazz => straightCollisionImplementations.Add((ICollisionCalculatorImplementation)Activator.CreateInstance(klazz)));

        }*/
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

            ValueTuple<Type, Func<object>> foundType = straightCollisionTypes.Where(i => i.Item1.Name.Equals(ApplicationConfigurationHandler.StraightCollisionCalculator))
                .FirstOrDefault();

            if(foundType.Equals(default(ValueTuple<Type, Func<object>>)))
            {
                foundType = defaultConstructor;
            }

            return (ICollisionCalculatorImplementation) foundType.Item2();

        }
    }
}
