using ApiLibrary.Api;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.RoadParameters
{
    public class RoadParametersHolder
    {

        private static RoadParametersHolder INSTANCE;

        private Dictionary<string, RoadParameters> parameters = new Dictionary<string, RoadParameters>();

        private ApiCallsHandler handler { get; set; }

        private RoadParametersHolder()
        {
            this.handler = ApiCallsHandler.GetHandler();
        }

        public static RoadParametersHolder GetInstance()
        {

            if(INSTANCE == null)
            {
                INSTANCE = new RoadParametersHolder();
            }

            return INSTANCE;

        }

        // This method locks parameters variable (it is slow, so use with caution!)
        public void SetParametersForRoadThreadSafe(Dictionary<string, RoadParameters> newParameters )
        {
            lock (parameters)
            {
                foreach(KeyValuePair<string, RoadParameters> param in newParameters)
                {
                    parameters.Add(param.Key, param.Value);
                }
            }
        }

        public RoadParameters GetParametersForRoad(string roadRef, bool refetch)
        {
            if (!parameters.ContainsKey(roadRef) || parameters[roadRef] == null || refetch)
            {

                LocationPoint point = RoadDataFetcher.GetInstance().ResolveRefToLocation(roadRef);

                // TODO: change this, this is just for testing purposes
                RoadParameters output = RoadDataFetcherFactory.GetInstance().GetResolverImplementation(ApplicationConfigurationHandler.RoadStateFetcherImplementation).FetchRoadParameters(point);

                Console.WriteLine("Fetched new road parameters");

                if(output == null)
                {
                    // If fetch fails (for some reason) and we have old value, use the old value
                    return parameters.ContainsKey(roadRef) ? parameters[roadRef] : null;
                }

                if (parameters.ContainsKey(roadRef))
                {
                    parameters[roadRef] = output;
                }
                else
                {
                    parameters.Add(roadRef, output);
                }

            }

            return parameters[roadRef];
        }

    }
}
