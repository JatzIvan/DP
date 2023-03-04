using SumoTraceParser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SumoTraceParser.OutputFileTemplate
{
    public interface AbstractTemplate
    {

        /*public Timestep GenerateTimestep(List<Vehicle> vehicles, float time);*/

        public void GenerateOutputDocument(FcdExport export, string path);

    }
}
