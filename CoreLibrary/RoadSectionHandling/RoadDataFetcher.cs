using ApiLibrary.Api;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketLibrary;

namespace CoreLibrary.RoadSectionHandling
{
    public class RoadDataFetcher: IObserver<AreaObserverWrapper>
    {
        private ApiCallsHandler Handler { get; set; }

        private int SocketID { get; set; }

        private List<RoadPointModel> RawFetchedData { get; set; }

        private Dictionary<string, List<RoadPointModel>> RoadSegmentsByRef { get; set; }

        private bool Finished = false;

        private bool Failed = false;

        public RoadDataFetcher()
        {
            this.Handler = ApiCallsHandler.GetHandler();
        }

        /**
         * TODO: Do more generic implementation
         */
        public List<RoadPointModel> GetRoadFromAPI()
        {

            List<RoadPointModel> output = Handler.Get<List<RoadPointModel>>("roads/" + ApplicationConfigurationHandler.TestRoadQuery);

            // When no road data could be fetched, repeat 3 times and then default with empty list
            // TODO: Implement repeat
            if(output == null)
            {
                return new List<RoadPointModel>();
            }

            output.Sort((RoadPointModel model1, RoadPointModel model2) => model1.OsmId.CompareTo(model2.OsmId));

            RawFetchedData = output;

            return output;
        }

        protected Dictionary<string, List<RoadPointModel>> SplitRawDataByRoadRef()
        {
            if(RawFetchedData == null)
            {
                throw new Exception("Missing Road Data");
            }

            return null;

        }

        public List<RoadPointModel> GetSpecifiedRoadSegmentFromAPI()
        {

            List<RoadPointModel> output = Handler.Get<List<RoadPointModel>>("roads/" + ApplicationConfigurationHandler.TestRoadQuery);

            // When no road data could be fetched, repeat 3 times and then default with empty list
            // TODO: Implement repeat
            if (output == null)
            {
                return new List<RoadPointModel>();
            }

            output.Sort((RoadPointModel model1, RoadPointModel model2) => model1.OsmId.CompareTo(model2.OsmId));

            return output;
        }

        public void OnCompleted()
        {
            Console.WriteLine("Completed");
            Finished = true;
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("Error occured during exception handling");
        }

        // Handle Recieved Area info from
        public void OnNext(AreaObserverWrapper value)
        {
            Console.WriteLine("On Next");
            SocketID = value.SocketId;
            Thread.Sleep(2000);
            OnCompleted();
        }
    }
}
