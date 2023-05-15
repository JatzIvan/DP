using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.RoadSectionHandling
{

    /**
     * Manager to create and store RoadDataHandler objects
     * Use this for correct road data recalc after new weather data was fetched
     */
    public class RoadDataManager
    {

        private static RoadDataManager INSTANCE;

        private List<RoadDataHandler> dataHandlers = new List<RoadDataHandler>();

        private RoadDataManager() { }

        public static RoadDataManager GetInstance()
        {

            if(INSTANCE == null)
            {
                INSTANCE = new RoadDataManager();
            }

            return INSTANCE;
            
        }

        public RoadDataHandler AddDataHandler(string sectionName, string sectionRef)
        {
            return AddDataHandler(new RoadDataHandler(sectionName, sectionRef));
        }

        public RoadDataHandler AddDataHandler(RoadDataHandler handler)
        {

            lock (dataHandlers)
            {
                dataHandlers.Add(handler);
            }

            return handler;
        }

        public void RemoveDataHandler(RoadDataHandler handler)
        {
            lock (dataHandlers)
            {
                dataHandlers.Remove(handler);
            }
        }

        public RoadDataHandler GetDataHandler(string sectionRef)
        {
            lock (dataHandlers)
            {
                return dataHandlers.Where(handler => handler.SectionRef.Equals(sectionRef)).FirstOrDefault();
            }
        }

        public void RecalculateAllHandlers()
        {
            foreach(RoadDataHandler handler in dataHandlers)
            {
                handler.RecalculateRoadModelBasedOnCurrentRoadState();
            }
        }

    }
}
