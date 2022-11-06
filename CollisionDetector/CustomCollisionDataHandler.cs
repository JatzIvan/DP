using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketLibrary;
using WebSocketLibrary.Models;

namespace CollisionDetector
{
    class CustomCollisionDataHandler : AbstractCollisionDetector
    {

        public CustomCollisionDataHandler(Dictionary<LocationPoint, AbstractRoadModel> currectRoadModel) : base(currectRoadModel)
        {

        }

        public override void PerformCalculations(List<VehicleData> data)
        {
            IEnumerable<IEnumerable<VehicleData>> pairsToCalc = CreateVehiclePairs(data);

            // Try threading or something, right now I need to ensure that this concept can work
            foreach (var pair in pairsToCalc)
            {
                ICollisionCalculatorImplementation calcMethod = ResolveCollisionCalculatorBasedOnCurvature(pair.ElementAt(0), pair.ElementAt(1));
                if(calcMethod != null)
                {
                    calcMethod.PerformCollisionCalculations(pair.ElementAt(0), pair.ElementAt(1), currectRoadModel);
                }
            }

        }
    }
}
