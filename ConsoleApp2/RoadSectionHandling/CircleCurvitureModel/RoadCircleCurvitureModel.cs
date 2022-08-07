using ConsoleApp2.RoadSectionHandling.Model;
using Npgsql;
using RoadSectionHandler;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling.CircleCurvitureModel
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
