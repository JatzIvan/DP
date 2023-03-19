using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.RoadSimplificators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.Data
{
    public class SectionSimplificationFactory: AbstractCalculatorFactory<ISectionSimplificator, RoadSimplificatorAttribute>
    {

        private SectionSimplificationFactory() : base(new RoadSimplificatorAttribute())
        {

        }

        static SectionSimplificationFactory()
        {

            GetInstance();
        }

        private static SectionSimplificationFactory INSTANCE;

        public static SectionSimplificationFactory GetInstance()
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
/*        public ISectionSimplificator GetSimplificatiorImplementation(List<AbstractRoadModel> connectedWays, string simplType)
        {

            ValueTuple<Type, Func<List<AbstractRoadModel>, object>> foundType = loadedSimplificators.Where(i => i.Item1.Name.Equals(simplType))
            .FirstOrDefault();

            if (foundType.Equals(default(ValueTuple<Type, Func<object[], object>>)))
            {
                foundType = defaultConstructor;
            }

            return (ISectionSimplificator)foundType.Item2(connectedWays);

            //return (ISectionSimplificator)Activator.CreateInstance(foundType, connectedWays);

        }*/

        public override (Type, Func<object>) GetDefaultInstance()
        {
            return (typeof(DouglasPeuckerRoadSectionSimplification), CreateCreator(typeof(DouglasPeuckerRoadSectionSimplification)));
        }
    }
    public enum SimplMethods
    {
        DouglasPeuckerRoadSectionSimplification,
        LangRoadSectionSimplification
    }

}
