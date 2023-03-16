using ApiLibrary.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.RoadSectionHandling.RoadParameters
{
    abstract class GenericRoadStateFetcher
    {

        protected float Long;
        protected float Lat;

        protected ApiCallsHandler APIHandler { get; set; }

        public GenericRoadStateFetcher(float lon, float lat) 
        {
            this.APIHandler = ApiCallsHandler.GetHandler();
            this.Long = lon;
            this.Lat = lat;
        }

        public abstract RoadParameters FetchRoadParameters();

        public abstract string GenerateUrl();


    }
}
