using CoreLibrary.RoadSectionHandling.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.Model
{
    public class AbstractRoadModel
    {
        public LocationPoint CurrentLocation { get; set; }

        public SegmentCurvitureChain Next { get; set; }
        public SegmentCurvitureChain Previous { get; set; }

        public double RadiusOfCircle { get; set; } = 0;

        public double Angle { get; set; } = 0;

        public double RadiusAccountedForByAngle { get; set; } = 0;

        public double MaxSpeed { get; set; } = Double.MaxValue;

        //Temporary
        public LocationPoint TangentOfPoint { get; set; }

        public double Distance { get; set; }

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

        public double Heading { get; set; }

        public double MaxSpeed { get; set; } = Double.MaxValue;

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
