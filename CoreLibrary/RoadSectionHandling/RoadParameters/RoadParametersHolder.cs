using ApiLibrary.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling.RoadParameters
{
    public class RoadParametersHolder
    {

        private static RoadParametersHolder INSTANCE;

        private Dictionary<string, RoadParameters> parameters;

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

        // TODO: this is just a placeholder, change when the specification is available
        private string createRequest(string roadRef)
        {
            return $"road/{roadRef}/state";
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
                RoadParameters output = handler.Get<RoadParameters>(createRequest(roadRef));

                if(output == null)
                {
                    // If fetch fails (for some reason) and we have old value, use the old value
                    return parameters.ContainsKey(roadRef) ? parameters[roadRef] : null;
                }

                parameters.Add(roadRef, output);

            }

            return parameters[roadRef];
        }

    }
}
