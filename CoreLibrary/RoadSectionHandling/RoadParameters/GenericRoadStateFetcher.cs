using ApiLibrary.Api;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.RoadSectionHandling.RoadParameters
{
    public abstract class GenericRoadStateFetcher
    {
        protected ApiCallsHandler APIHandler { get; set; }

        public GenericRoadStateFetcher() 
        {
            this.APIHandler = ApiCallsHandler.GetHandler();
        }

        public abstract RoadParameters FetchRoadParameters(LocationPoint point);

        public abstract string GenerateUrl(LocationPoint point);


    }
}
