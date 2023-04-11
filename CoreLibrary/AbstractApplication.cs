using ConsoleApp1.Api;
using CoreLibrary.RoadSectionHandling.RoadParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLibrary
{
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

        public abstract void MainLogicStartup();

        public virtual void StartThreads()
        {

            Thread td4 = new Thread(new RoadStateRefresherThread().FetchInformation);
            td4.Start();

            CreatedThreads.Add(td4);
        }

    }
}
