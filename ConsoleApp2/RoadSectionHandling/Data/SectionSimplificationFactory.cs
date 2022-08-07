using ConsoleApp2.RoadSectionHandling.Model;
using ConsoleApp2.RoadSectionHandling.RoadSimplificators;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling.Data
{
    class SectionSimplificationFactory
    {
        private SectionSimplificationFactory()
        {

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
                return SimplMethods.DOUGLAS_PEUCKER;
            }

            if (config is LangConfig)
            {
                return SimplMethods.LANG;
            }

            return null;

        }

        public ISectionSimplificator GetSimplificatiorImplementation(Dictionary<LocationPoint, AbstractRoadModel> connectedWays, AbstractSimplificationModel config)
        {
            switch (resolveMethod(config))
            {
                case SimplMethods.DOUGLAS_PEUCKER:
                    return new DouglasPeuckerRoadSectionSimplification(connectedWays, (DouglasPeuckerConfig) config);
                case SimplMethods.LANG:
                    return new LangRoadSectionSimplification(connectedWays, (LangConfig) config);
                default:
                    return null;
            }
        }
    }
    public enum SimplMethods
    {
        DOUGLAS_PEUCKER,
        LANG
    }

}
