using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    public interface IMessageHandler<T> where T: ObserverWrapper
    {

        public void PerformActions(T data);

    }
}
