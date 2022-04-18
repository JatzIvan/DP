using Npgsql;
using RoadSectionHandler;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling.Model
{
    public class RoadCurvitureModel : IDatabaseModel<RoadCurvitureModel>
    {

        public LocationPoint CurrentLocation { get; set; }

        public SegmentCurvitureChain Next { get; set; }

        public SegmentCurvitureChain Previous { get; set; }

        public double RadiusOfCircle { get; set; } = 0;

/*        // For sharp turn, radius is small. Translate to curviture for better representation
        public double GetCurviture()
        {
            return RadiusOfCurviture == 0 ? 0 : (1 / RadiusOfCurviture);
        }*/

        public RoadCurvitureModel MapReaderToObject(NpgsqlDataReader reader)
        {
            throw new NotImplementedException();
        }

        public RoadCurvitureModel(LocationPoint currentLocation)
        {
            this.CurrentLocation = currentLocation;
        }

    }

    public class SegmentCurvitureChain
    {
        public CURVITURE_SHARPNESS CurveSharpness { get; set; }

        public double RadiusOfCurvature { get; set; }

        public RoadCurvitureModel Point { get; set; }

        public SegmentCurvitureChain (RoadCurvitureModel point)
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
