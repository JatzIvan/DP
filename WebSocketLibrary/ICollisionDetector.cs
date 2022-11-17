using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    public interface ICollisionDetector
    {

        public void PerformCalculations(ObserverWrapper data);

    }
}
