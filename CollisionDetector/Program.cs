using ConsoleApp1.Api;
using CoreLibrary;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.CollisionCalculators.StraightCalculators;
using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.RoadParameters;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WebSocketLibrary;
using WebSocketLibrary.Models;
using WebSocketLibrary.SocketImplementations;
using static System.Net.Mime.MediaTypeNames;

namespace CollisionDetector
{
    class Program
    {

        static void Main(string[] args)
        {
            HeadOnCollisionApplicationStarter<SimpleHeadOnApplication>.Run(new SimpleHeadOnApplication());
        }

    }
}
