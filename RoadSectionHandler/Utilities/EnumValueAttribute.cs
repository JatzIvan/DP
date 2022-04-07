using System;
using System.Collections.Generic;
using System.Text;

namespace RoadSectionHandler.Utilities
{
    public class EnumValueAttribute : Attribute
    {
        public string StringValue { get; protected set; }
        
        
        public EnumValueAttribute(string value)
        {
            this.StringValue = value;
        }



    }


}
