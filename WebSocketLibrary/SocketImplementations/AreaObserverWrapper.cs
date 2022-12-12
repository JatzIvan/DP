using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
  public class AreaObserverWrapper : ObserverWrapper
    {

        public AreaMessage Data { get; set; }

        public AreaObserverWrapper(AreaMessage data, int socketId) : base(socketId)
        {

            this.Data = data;

        }
    }
}
