using CoreLibrary.RoadSectionHandling.RoadParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketLibrary.Models;

namespace CoreLibrary.RoadSectionHandling
{

    // TODO: could make more generic, but i see no point in this case
    public class BrakingDistanceCalculatorUtils
    {

        public static double g = 9.81f;

        public static double CalculateBrakingDistance(VehicleData vehicleData, string roadRef)
        {

            //RoadParametersHolder.GetInstance().GetParametersForRoad(roadRef, false).GetFriction();

            double a = g * 0.7;

            double S = Math.Pow(vehicleData.Speed, 2)/(2*a);

            return S;
        }

    }
}
