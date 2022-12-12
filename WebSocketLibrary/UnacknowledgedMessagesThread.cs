using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    // Thread will resend unacknowledged messages
    public class UnacknowledgedMessagesThread
    {

        public void HandleUnresolved()
        {

            while (true)
            {
                Dictionary<int, AbstractSocket> activeConnections = WebSocketManagerFactory.GetInstance().GetActiveConnections();

                foreach (KeyValuePair<int, AbstractSocket> val in activeConnections)
                {
                    val.Value.GetAllUnconfirmedMessages().ForEach(msg =>val.Value.SendMessageWithAck(msg));
                }

                // TODO: choose better value
                Thread.Sleep(200);
            }
        }

    }
}
