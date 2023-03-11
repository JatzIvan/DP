using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CoreLibrary.RoadSectionHandling.RoadParameters
{

    // Simple thread that will update road information
    public class RoadStateRefresherThread
    {

        public void FetchInformation()
        {

            while (true)
            {

                Dictionary<string, List<RoadPointModel>> sections = RoadDataFetcher.GetInstance().GetRoadFromAPIGroupedByRef();

                foreach(KeyValuePair<string, List<RoadPointModel>> section in sections)
                {
                    // Fetch Data
                    RoadParametersHolder.GetInstance().GetParametersForRoad(section.Key, true);

                }

                RoadDataManager.GetInstance().RecalculateAllHandlers();

                // Recalculate all the necessary calculations

                // TODO: Wait for 10 minutes for now
                Thread.Sleep(600000);

            }

        }

    }
}
