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

                // TODO: Wait for 10 minutes for now
                Thread.Sleep(600000);

                Dictionary<string, List<RoadPointModel>> sections = RoadDataFetcher.GetInstance().GetRoadFromAPIGroupedByAttr();

                foreach(KeyValuePair<string, List<RoadPointModel>> section in sections)
                {
                    // Fetch Data
                    RoadParametersHolder.GetInstance().GetParametersForRoad(section.Key, true);

                }

                // Recalculate all the necessary calculations
                RoadDataManager.GetInstance().RecalculateAllHandlers();


            }

        }

    }
}
