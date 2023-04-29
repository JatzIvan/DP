using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    /**
     * Utility class that starts the application in the intended way
     */
    public class HeadOnCollisionApplicationStarter<T> where T : AbstractApplication
    {

        public static void Run(T appInstance)
        {
            appInstance.LoadConfiguration();
            appInstance.InitializeAPIClient();

            appInstance.MainLogicStartup();

            appInstance.StartThreads();

            Console.Read();
        }

    }
}
