using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary.Models
{
    public class WarningMessage: AbstractMessage
    {

        public new string Type { get; set; } = "warning";

        public int VehicleId{ get; set; }

        public double TimeToCollision { get; set; }

        public string CollisionType { get; set; }

        public string CollisionSeverity { get; set; }

    }
}
