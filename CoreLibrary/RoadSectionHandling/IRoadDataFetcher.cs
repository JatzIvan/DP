using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.RoadSectionHandling
{
    public interface IRoadDataFetcher
    {
        public List<RoadPointModel> GetRoadFromAPI();

        public Dictionary<string, List<RoadPointModel>> GetRoadFromAPIGroupedByRef();

        public List<RoadPointModel> GetRoadDataForRef(string roadRef);

    }
}
