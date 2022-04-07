using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    public interface CollisionDetector
    {

        public void PerformCalculations(List<CarUpdateInfo> data);

    }
}
