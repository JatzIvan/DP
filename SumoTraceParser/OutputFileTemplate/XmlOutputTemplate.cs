using SumoTraceParser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SumoTraceParser.OutputFileTemplate
{
    class XmlOutputTemplate : AbstractTemplate
    {
        public void GenerateOutputDocument(FcdExport export, string path)
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(FcdExport));

            System.IO.FileStream file = System.IO.File.Create(path);

            writer.Serialize(file, export);
            file.Close();
        }

/*        public Timestep GenerateTimestep(List<Vehicle> vehicles, float time)
        {
            
        }*/
    }
}
