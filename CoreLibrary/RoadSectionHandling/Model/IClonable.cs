using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.RoadSectionHandling.Model
{
    internal interface IClonable<T>
    {

        T Clone();

    }
}
