using CoreLibrary.RoadSectionHandling.CurvatureCalculations;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.Data
{
    public class RoadCurvitureCalculatorFactory: AbstractCalculatorFactory<ICurvesResolver, CurvatureResolverAttribute>
    {

        private RoadCurvitureCalculatorFactory(): base(new CurvatureResolverAttribute())
        {

        }

        static RoadCurvitureCalculatorFactory()
        {
            GetInstance();
        }

        private static RoadCurvitureCalculatorFactory INSTANCE;

        public static RoadCurvitureCalculatorFactory GetInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new RoadCurvitureCalculatorFactory();
            }

            return INSTANCE;
        }

        public override (Type, Func<object>) GetDefaultInstance()
        {
            return (typeof(CircumcircleRoadCircleCurvesResolver), CreateCreator(typeof(CircumcircleRoadCircleCurvesResolver)));
        }

    }
}
