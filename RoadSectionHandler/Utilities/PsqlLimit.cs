using System;
using System.Collections.Generic;
using System.Text;

namespace RoadSectionHandler.Utilities
{
    public class PsqlLimit
    {
        public int Limit { get; set; } = -1;
        public int Offset { get; set; } = -1;

        public PsqlLimit(int limit, int offset)
        {

            this.Limit = limit > 0 ? limit : -1;
            this.Offset = offset > 0 ? offset : -1;
        }

        public PsqlLimit(int limit)
        {
            this.Limit = limit;
        }

        public string ResolveLimit()
        {

            string returnResult = "";

            if(Limit != -1)
            {
                returnResult += "LIMIT " + Limit + " ";
            }
            if(Offset != -1)
            {
                returnResult += "OFFSET " + Offset;
            }

            return returnResult;
        }

    }
}
