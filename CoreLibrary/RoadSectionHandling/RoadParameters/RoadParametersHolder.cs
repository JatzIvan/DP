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

        private Dictionary<object, RoadParameters> parameters = new Dictionary<object, RoadParameters>();

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

        public RoadParameters GetParametersForRoad(string attr, bool refetch)
        {
            if (!parameters.ContainsKey(attr) || parameters[attr] == null || refetch)
            {

                LocationPoint point = RoadDataFetcher.GetInstance().ResolveAttrToLocation(attr);

                // TODO: change this, this is just for testing purposes
                RoadParameters output = RoadDataFetcherFactory.GetInstance().GetResolverImplementation(ApplicationConfigurationHandler.RoadStateFetcherImplementation).FetchRoadParameters(point);

                Console.WriteLine("Fetched new road parameters");

                if(output == null)
                {
                    // If fetch fails (for some reason) and we have old value, use the old value
                    return parameters.ContainsKey(attr) ? parameters[attr] : null;
                }

                if (parameters.ContainsKey(attr))
                {
                    parameters[attr] = output;
                }
                else
                {
                    parameters.Add(attr, output);
                }

            }

            return parameters[attr];
        }

    }
}
