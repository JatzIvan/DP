using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators.CurveCalculators
{
    class AdvancedCurveRoadCollisionCalculator : ICollisionCalculatorImplementation
    {
        public float CalculateTTC()
        {
            throw new NotImplementedException();
        }

        public bool CollisionOccured()
        {
            throw new NotImplementedException();
        }

        public ICollisionCalculatorImplementation.CollisionSeverity GetCollisionSeverity()
        {
            throw new NotImplementedException();
        }

        public CollisionInfo PerformCollisionCalculations(VehicleData vehicle1, VehicleData vehicle2, Dictionary<LocationPoint, AbstractRoadModel> currectRoadModel)
        {

            double Vb = vehicle2.Speed;
            double Va = vehicle1.Speed;
            double alpha;
            double delta;
            double Dab = MapParserUtils.CalculateDistanceBetweenPoints(new LocationPoint(vehicle1.Position.Lon, vehicle1.Position.Lat), 
                new LocationPoint(vehicle2.Position.Lon, vehicle2.Position.Lat));
            int i = 0;
            do
            {

                Vb += vehicle2.Acceleration;
                Va += vehicle1.Acceleration;
                i++;
                delta = -Va + Vb + vehicle2.Acceleration;

                alpha = (Math.Acos(Math.Pow(delta, 2) + Math.Pow(Dab, 2) - Math.Pow(Dab, 2) * delta)) / (2 * delta * Dab);
                

            } while ();


            return null;
        }
    }
}
