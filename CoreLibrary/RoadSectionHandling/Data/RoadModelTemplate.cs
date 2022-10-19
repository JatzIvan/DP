using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling.Data
{
    interface RoadModelTemplate<T>
    {
        public SegmentCurvitureChain<T> Next { get; set; }

        public SegmentCurvitureChain<T> Previous { get; set; }

        public LocationPoint CurrentLocation { get; set; }
    }

    public class SegmentCurvitureChain<T>
    {
        public CURVITURE_SHARPNESS CurveSharpness { get; set; }

        public double RadiusOfCurvature { get; set; }

        public T Point { get; set; }

        public SegmentCurvitureChain(T point)
        {
            this.Point = point;
        }

    }

    public enum CURVITURE_SHARPNESS
    {
        NO_CURVE,
        LIGHT,
        MEDIUM,
        SHARP
    }
}
