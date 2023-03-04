using SumoTraceParser.Models;
using SumoTraceParser.OutputFileTemplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using WebSocketLibrary;

namespace SumoTraceParser
{
    public abstract class AbstractTraceResolver<T> where T: PairWrapper
    {

        protected FcdExport SumoExport;

        protected T WrapperInstance;

        public AbstractTraceResolver(string path)
        {

            using (var reader = new StreamReader(path))
            {
                SumoExport = (FcdExport)new XmlSerializer(typeof(FcdExport)).Deserialize(reader);
            }

            if (SumoExport == null)
            {
                throw new Exception("Parser was unable to open/parse provided file");
            }

        }

        public IEnumerable<IEnumerable<Vehicle>> CreateVehiclePairs(List<Vehicle> vehicles)
        {
            return AbstractCollisionDetector.GetPermutations(vehicles, 2)
                .Where(o => !(WrapperInstance.ResolvedIdPairs.Select(t => t.Item1).ToList().Contains(o.ElementAt(0).Id)
                && WrapperInstance.ResolvedIdPairs.Select(t => t.Item2).ToList().Contains(o.ElementAt(1).Id)));
        }

        // ids to export with tag for filename
        public abstract List<Tuple<string, List<string>>> CreateExportPairs(T VehiclePairs);

        public Timestep CreateTimestep(List<Vehicle> vehicles, double time)
        {
            Timestep step = new Timestep();
            step.Vehicles = vehicles;
            step.Time = time;
            return step;
        }

        public void CreateExportBasedOnConfig(ExportConfig conf, T VehiclePairs)
        {
            Console.WriteLine("\n\n");
            Console.WriteLine("Started Exporting");

            bool exists = System.IO.Directory.Exists(conf.OutputPath + "/sumo_parsed");

            if (!exists)
                System.IO.Directory.CreateDirectory(conf.OutputPath + "/sumo_parsed");

            List<Tuple<string, List<string>>> vehiclesToKeep = CreateExportPairs(VehiclePairs);

            int index = 0;

            foreach(Tuple<string, List<string>> keep in vehiclesToKeep)
            {

                Console.WriteLine("Exporting pair - " + String.Join(",", keep.Item2));

                FcdExport newExport = new FcdExport();
                newExport.Timestemps = new List<Timestep>();

                foreach (Timestep step in SumoExport.Timestemps)
                {

                    List<Vehicle> vehiclesToRender = new List<Vehicle>();

                    foreach(Vehicle vehicle in step.Vehicles)
                    {
                        if (keep.Item2.Contains(vehicle.Id))
                        {
                            vehiclesToRender.Add(vehicle);
                        }
                    }

                    if(vehiclesToRender.Count == 0 && conf.SkipEmptyTimesteps)
                    {
                        continue;
                    }

                    Timestep time = newExport.Timestemps.LastOrDefault();

                    newExport.Timestemps.Add(CreateTimestep(vehiclesToRender, (conf.KeepTimesteps ? step.Time : (time == null ? 0 : (time.Time + conf.TimestepTimeIncrement)))));

                }

                Console.WriteLine("Creating output - " + String.Join(",", keep.Item2));

                switch (conf.Output)
                {
                    case OutputType.xml:
                        new XmlOutputTemplate().GenerateOutputDocument(newExport, conf.OutputPath + "/sumo_parsed/out_" + keep.Item1 + "_" + String.Join("_", keep.Item2) + "__" + index + ".xml");
                        break;
                }

                index++;

            }


        }

        public abstract T CreateVehiclePairs();

    }
}
