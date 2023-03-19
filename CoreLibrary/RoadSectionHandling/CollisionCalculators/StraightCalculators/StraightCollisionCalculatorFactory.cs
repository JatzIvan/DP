using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators
{
    class StraightCollisionCalculatorFactory : AbstractCalculatorFactory<ICollisionCalculatorImplementation, CollisionTypeAttribute>, ICollisionCalculatorFactory
    {

        private static ICollisionCalculatorFactory INSTANCE;


        // Initialize before anything to be ready to use
        static StraightCollisionCalculatorFactory()
        {

            GetInstance();

        }

        private StraightCollisionCalculatorFactory(): base(new CollisionTypeAttribute(CollisionTypeEnum.STRAIGHT))
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

        public override (Type, Func<object>) GetDefaultInstance()
        {
            return (typeof(SimpleStraightRoadCurvatureCalculator), CreateCreator(typeof(SimpleStraightRoadCurvatureCalculator)));
        }

        // Choose from list by config value or else return Dummy implementation
        // Always create new implementation because we will work with multiple Threads
        public ICollisionCalculatorImplementation GetImplementation()
        {
            return GetResolverImplementation(ApplicationConfigurationHandler.StraightCollisionCalculator);

        }
    }
}
