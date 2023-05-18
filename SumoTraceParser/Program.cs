using System;
using TestingLibrary;

namespace SumoTraceParser
{
    class Program
    {
        static void Main(string[] args)
        {
            /*            TraceResolver resolver = new TraceResolver("F:/C_sharp_projekty/SumoDataParser/sumoTrace_100ms.xml");
                        ExportConfig conf = new ExportConfig();
                        conf.OutputPath = "F:/parser_test";
                        resolver.CreateExportBasedOnConfig(conf, resolver.CreateVehiclePairs());*/

            StressTesting test = new StressTesting("F:/C_sharp_projekty/SumoDataParser/sumoTrace_100ms.xml", "F:/parser_test/stress_test/");
            test.ProcessDump();

        }
    }
}
