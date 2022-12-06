using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    public class SocketKeepAlive
    {

        public void KeepAliveActiveConnections()
        {

            while (true)
            {
                Dictionary<int, AbstractSocket> activeConnections = WebSocketManagerFactory.GetInstance().GetActiveConnections();

                //Console.WriteLine(activeConnections.Count);

                foreach (KeyValuePair<int, AbstractSocket> entry in activeConnections)
                {

                    KeepAliveMessage msg = entry.Value.GetKeepAliveMessage();

                    if(entry.Value.keepAliveFailedAttempts > 5)
                    {
                        Console.WriteLine("Socket " + entry.Key + " has lost connection");
                        WebSocketManagerFactory.GetInstance().DropActiveConnection(entry.Value);
                    }
                    else
                    {
                        entry.Value.SendMessageWithAck(msg);
                    }
                }

                Thread.Sleep(10000);
            }


        }

    }
}
