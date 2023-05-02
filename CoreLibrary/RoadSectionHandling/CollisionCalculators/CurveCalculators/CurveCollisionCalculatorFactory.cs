using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.CurveCalculators
{
    class CurveCollisionCalculatorFactory : AbstractCalculatorFactory<ICollisionCalculatorImplementation, CollisionTypeAttribute>, ICollisionCalculatorFactory
    {
        private static ICollisionCalculatorFactory INSTANCE;

        // Initialize before anything to be ready to use
        static CurveCollisionCalculatorFactory()
        {
      
            GetInstance();

        }

        private CurveCollisionCalculatorFactory(): base(new CollisionTypeAttribute(CollisionTypeEnum.CURVATURE))
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

            return GetResolverImplementation(ApplicationConfigurationHandler.CurveCollisionCalculator);

        }

        public override (Type, Func<object>) GetDefaultInstance()
        {
            return (typeof(SimpleCurveRoadCurvatureCalculator), CreateCreator(typeof(SimpleCurveRoadCurvatureCalculator)));
        }
    }
}
