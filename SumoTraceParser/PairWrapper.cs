using System;
using System.Collections.Generic;
using System.Text;

namespace SumoTraceParser
{
    public class PairWrapper
    {
        public List<Tuple<string, string>> ResolvedIdPairs { get; set; } = new List<Tuple<string, string>>();

    }
}
