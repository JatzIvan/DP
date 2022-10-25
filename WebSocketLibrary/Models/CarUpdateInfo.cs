using System.Collections.Generic;

namespace WebSocketLibrary.Models
{
    public class CarUpdateInfo: AbstractMessage
    {

        public new string Type { get; set; } = "update_vehicles";

        public List<VehicleData> Vehicles { get; set; }


    }
}
