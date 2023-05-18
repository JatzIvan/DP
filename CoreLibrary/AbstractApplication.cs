using ApiLibrary.Api;
using CoreLibrary.RoadSectionHandling.RoadParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLibrary
{

    /**
     * Application template
     * Each application should implement all methods declared in this class for correct startup
     */
    public abstract class AbstractApplication
    {


        protected List<Thread> CreatedThreads = new List<Thread>();

        public virtual void InitializeAPIClient()
        {
            ApiHelper.InitializeClient();
        }

        public virtual void LoadConfiguration()
        {
            ApplicationConfigurationHandler.LoadConfiguration();
        }

        /*
         * This method should contain application specific setup during startup
         * Main thread will be locked so this method should provide necessary logic for communication
         */
        public abstract void MainLogicStartup();

        public virtual void StartThreads()
        {

            Thread td4 = new Thread(new RoadStateRefresherThread().FetchInformation);
            td4.Start();

            CreatedThreads.Add(td4);
        }

    }
}
