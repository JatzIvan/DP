using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary.RoadSectionHandling
{
    public class AbstractCalculatorAttribute: Attribute
    {

        public string Type { get; set; }

        public AbstractCalculatorAttribute(string type)
        {
            this.Type = type;
        }

    }
}
