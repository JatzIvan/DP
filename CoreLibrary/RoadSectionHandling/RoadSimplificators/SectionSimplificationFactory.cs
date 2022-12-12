using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.RoadSimplificators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.Data
{
    public class SectionSimplificationFactory: AbstractCalculatorFactory
    {

        private static List<ValueTuple<Type, Func<List<AbstractRoadModel>, object>>> loadedSimplificators = new List<ValueTuple<Type, Func<List<AbstractRoadModel>, object>>>();
        private static ValueTuple<Type, Func<List<AbstractRoadModel>, object>> defaultConstructor;

        private SectionSimplificationFactory()
        {

        }

        static SectionSimplificationFactory()
        {

            List<Type> simplificators = LoadImplementations(new RoadSimplificatorAttribute(), typeof(ISectionSimplificator));
            foreach(Type simplificator in simplificators)
            {
                loadedSimplificators.Add((simplificator, CreateCreator<List<AbstractRoadModel>>(simplificator)));
            }

            defaultConstructor = (typeof(DouglasPeuckerRoadSectionSimplification), CreateCreator<List<AbstractRoadModel>>(typeof(DouglasPeuckerRoadSectionSimplification)));

        }

        private static SectionSimplificationFactory INSTANCE;

        public static SectionSimplificationFactory getInstance()
        {
            if(INSTANCE == null)
            {
                INSTANCE = new SectionSimplificationFactory();
            }

            return INSTANCE;
        }

        public SimplMethods? resolveMethod(AbstractSimplificationModel config)
        {
            if(config is DouglasPeuckerConfig)
            {
                return SimplMethods.DouglasPeuckerRoadSectionSimplification;
            }

            if (config is LangConfig)
            {
                return SimplMethods.LangRoadSectionSimplification;
            }

            return null;

        }

        /**
         *  Return simplification method based on config Type.
         */
        public ISectionSimplificator GetSimplificatiorImplementation(List<AbstractRoadModel> connectedWays, string simplType)
        {

            ValueTuple<Type, Func<List<AbstractRoadModel>, object>> foundType = loadedSimplificators.Where(i => i.Item1.Name.Equals(simplType))
            .FirstOrDefault();

            if (foundType.Equals(default(ValueTuple<Type, Func<object[], object>>)))
            {
                foundType = defaultConstructor;
            }

            return (ISectionSimplificator)foundType.Item2(connectedWays);

            //return (ISectionSimplificator)Activator.CreateInstance(foundType, connectedWays);

        }

        public override List<Type> GetLoadedTypes()
        {
            return loadedSimplificators.Select(_ => _.Item1).ToList();
        }
    }
    public enum SimplMethods
    {
        DouglasPeuckerRoadSectionSimplification,
        LangRoadSectionSimplification
    }

}
