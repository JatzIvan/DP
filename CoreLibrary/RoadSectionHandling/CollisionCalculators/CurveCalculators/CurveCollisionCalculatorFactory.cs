using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.CurveCalculators
{
    class CurveCollisionCalculatorFactory : AbstractCalculatorFactory, ICollisionCalculatorFactory
    {
        private static ICollisionCalculatorFactory INSTANCE;

        //private static List<Type> curveCollisionImplementations = new List<Type>();
        private static List<ValueTuple<Type, Func<object>>> curveCollisionImplementations = new List<ValueTuple<Type, Func<object>>>();
        private static ValueTuple<Type, Func<object>> defaultConstructor;


        // Initialize before anything to be ready to use
        static CurveCollisionCalculatorFactory(){
            //LoadImplementations();
            //curveCollisionImplementations = LoadImplementations(new CollisionTypeAttribute(CollisionTypeEnum.CURVATURE), typeof(ICollisionCalculatorImplementation));
            List<Type> calculators = LoadImplementations(new CollisionTypeAttribute(CollisionTypeEnum.CURVATURE), typeof(ICollisionCalculatorImplementation));
            foreach (Type calculator in calculators)
            {
                curveCollisionImplementations.Add((calculator, CreateCreator(calculator)));
            }

            defaultConstructor = (typeof(SimpleCurveRoadCurvatureCalculator), CreateCreator(typeof(SimpleCurveRoadCurvatureCalculator)));


        }

        public override List<Type> GetLoadedTypes()
        {
            return curveCollisionImplementations.Select(_ => _.Item1).ToList();
        }

        /*private static void LoadImplementations()
        {
            Type calculatorType = typeof(ICollisionCalculatorImplementation);

            // Some reflection magic to read all "calculators" from current namespace
            // TODO test this with functional implementation
            curveCollisionImplementations = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == typeof(CurveCollisionCalculatorFactory).Namespace)
                .Where(p => calculatorType.IsAssignableFrom(p)).ToList();

            // Some more magic to create instances of gathered types (we do not need to provide any data to constructors)
            //implKlazzes.ForEach(klazz => curveCollisionImplementations.Add((ICollisionCalculatorImplementation)Activator.CreateInstance(klazz)));

        }*/

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

            ValueTuple <Type, Func<object>> foundType = curveCollisionImplementations.Where(i => i.Item1.Name.Equals(ApplicationConfigurationHandler.CurveCollisionCalculator))
                .FirstOrDefault();

            if (foundType.Equals(default(ValueTuple<Type, Func<object>>)))
            {
                foundType = defaultConstructor;
            }

            return (ICollisionCalculatorImplementation)foundType.Item2();

        }
    }
}
