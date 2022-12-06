using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    public class ObserverWrapper
    {
        public int SocketId { get; set; }

        public ObserverWrapper(int socketId)
        {
            this.SocketId = socketId;
        }

    }
}
