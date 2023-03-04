using System;
using System.Collections.Generic;
using System.Text;

namespace SumoTraceParser
{
    public class ExportConfig
    {

        public string OutputPath { get; set; }

        public bool SkipEmptyTimesteps { get; set; } = true;

        public bool KeepTimesteps { get; set; } = false;

        // s
        public float TimestepTimeIncrement { get; set; } = 0.1f;

        public CouplingType Type { get; set; } = CouplingType.SinglePair;

        public OutputType Output { get; set; } = OutputType.xml;

        public int CustomNumber { get; set; }
    }

    public enum CouplingType
    {
        SinglePair,
        CustomNumberOfPairs
    }

    public enum OutputType
    {
        xml
    }
}
