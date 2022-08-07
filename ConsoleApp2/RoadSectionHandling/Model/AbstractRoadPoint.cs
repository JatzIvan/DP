using ConsoleApp2.RoadSectionHandling.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling.Model
{
    public class AbstractRoadModel
    {
        public LocationPoint CurrentLocation { get; set; }

        public SegmentCurvitureChain Next { get; set; }
        public SegmentCurvitureChain Previous { get; set; }

        public double RadiusOfCircle { get; set; } = 0;

        //Temporary
        public LocationPoint TangentOfPoint { get; set; }

        public AbstractRoadModel(LocationPoint point)
        {
            this.CurrentLocation = point;
        }

        public T GetResult<T>() where T : AbstractRoadModel
        {
            return (T)this;
        }

    }

    public class SegmentCurvitureChain
    {
        public CURVITURE_SHARPNESS CurveSharpness { get; set; }

        public double RadiusOfCurvature { get; set; }

        public AbstractRoadModel Point { get; set; }

        public SegmentCurvitureChain(AbstractRoadModel point)
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
