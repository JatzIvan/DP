using System.Collections;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    public class VehicleObserverWrapper : ObserverWrapper
    {
        public CarUpdateInfo Data { get; set; }
        public VehicleObserverWrapper(CarUpdateInfo data, int socketId) : base(socketId)
        {
            this.Data = data;
        }

    }
}