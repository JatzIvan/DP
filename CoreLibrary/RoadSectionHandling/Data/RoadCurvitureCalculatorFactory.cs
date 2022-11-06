using CoreLibrary.RoadSectionHandling.CircleCurvitureModel;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.Data
{
    class RoadCurvitureCalculatorFactory
    {

        private RoadCurvitureCalculatorFactory()
        {

        }

        private static RoadCurvitureCalculatorFactory INSTANCE;

        public static RoadCurvitureCalculatorFactory getInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new RoadCurvitureCalculatorFactory();
            }

            return INSTANCE;
        }

        /**
         * TODO: Try something like annotations later
         */
        public ICurvesResolver GetResolverImplementation(CurvCalcMethods type, Dictionary<LocationPoint, AbstractRoadModel> connectedWays)
        {

            switch (type)
            {
                case CurvCalcMethods.SIMPLE_CIRCLE:
                    return new RoadCircleCurvesResolver(connectedWays);
                case CurvCalcMethods.OSCULATING_CICRCE:
                    return new OsculatingCircleCurvesResolver(connectedWays);
            }

            return null;
        }
    }

    public enum CurvCalcMethods
    {
        SIMPLE_CIRCLE,
        OSCULATING_CICRCE
    }
}
