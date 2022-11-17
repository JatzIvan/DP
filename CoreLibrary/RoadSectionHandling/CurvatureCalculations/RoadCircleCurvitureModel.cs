using CoreLibrary.RoadSectionHandling.Model;
using Npgsql;
using RoadSectionHandler;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.CurvatureCalculations
{
    public class RoadCircleCurvitureModel : AbstractRoadModel, IDatabaseModel<RoadCircleCurvitureModel>
    {

        public RoadCircleCurvitureModel(LocationPoint point) : base(point)
        { }

        public RoadCircleCurvitureModel MapReaderToObject(NpgsqlDataReader reader)
        {
            throw new NotImplementedException();
        }

    }
}
