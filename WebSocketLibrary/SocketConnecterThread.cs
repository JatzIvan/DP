using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WebSocketLibrary
{
    /**
     * Define methods that will be executed in separate thread
     * Thread will handle all pending connections (it will send subscribe until acknowledgement was recieved)
     */
    /*public class SocketConnecterThread
    {

        public void HandlePending()
        {

            while (true)
            {
                Dictionary<int, UdpSocketClientImplementation>  pending = WebSocketManagerFactory.GetInstance().GetPendingConnections();

                foreach(KeyValuePair<int, UdpSocketClientImplementation> val in pending)
                {
                    bool active = val.Value.checkIfAlive();
                    if (active)
                    {
                        WebSocketManagerFactory.GetInstance().ActivatePendingConnection(val.Value);
                    }
                    else
                    {
                        Console.WriteLine("Trying to connect socket " + val.Key);
                        val.Value.HandleHandShake();
                    }

                }

                Thread.Sleep(2000);
            }
        }

    }*/
}
