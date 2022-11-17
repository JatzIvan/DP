using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    public class ObserverWrapper
    {
        public List<VehicleData> Data { get; set; }

        public int SocketId { get; set; }

        public ObserverWrapper(List<VehicleData> data, int socketId)
        {
            this.Data = data;
            this.SocketId = socketId;
        }

    }
}
