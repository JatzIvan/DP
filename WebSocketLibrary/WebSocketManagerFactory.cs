using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    /**
     * Simple factory class that handles all current udp socket "connections"
     * Each "connection" is stored in dictionary so it is easily accessible with ID
     */
    public class WebSocketManagerFactory
    {

        private static Dictionary<int, UdpSocketClientImplementation> ActiveConnections { get; set; } = new Dictionary<int, UdpSocketClientImplementation>();
        private static Dictionary<int, UdpSocketClientImplementation> PendingConnections { get; set; } = new Dictionary<int, UdpSocketClientImplementation>();

        private static Dictionary<int, UdpSocketClientImplementation> RegisteredConnections { get; set; } = new Dictionary<int, UdpSocketClientImplementation>();

        private static WebSocketManagerFactory Instance;

        /**
         * Share instances between threads
         */
        public static WebSocketManagerFactory GetInstance()
        {
            
            if(Instance == null)
            {
                Instance = new WebSocketManagerFactory();
            }

            return Instance;
        }

        public UdpSocketClientImplementation CreateConnection(string url, string port)
        {
           return this.CreateConnection(url, port, new Random().Next());
        }


        public UdpSocketClientImplementation CreateConnection(string host, string port, int id)
        {
            //string trimmed = ReshapeWebSocketUrl(url);

            UdpSocketClientImplementation ws = new UdpSocketClientImplementation(host, port, id);

            RegisteredConnections.Add(id, ws);

            return ws;

        }

        public bool OpenConnection(UdpSocketClientImplementation connection, List<IObserver<ObserverWrapper>> handler)
        {

            if(connection == null || !RegisteredConnections.ContainsValue(connection))
            {
                return false;
            }

            handler.ForEach(h =>
            {
                connection.Subscribe(h);
            });

            RegisteredConnections.Remove(connection.Id);
            PendingConnections.Add(connection.Id, connection);

            return connection.CreateConnectionWithDataSocket();

        }


        public bool OpenConnection(int connectionId, List<IObserver<ObserverWrapper>> handler)
        {
            return OpenConnection(RegisteredConnections[connectionId], handler);
        }

        public void CloseConnection(UdpSocketClientImplementation ws)
        {
            lock (this)
            {
                if(ws != null)
                {

                    ActiveConnections.Remove(ws.Id);
                    ws.CloseConnection();
                }
            }
        }

        public void CloseConnection(int connectionId)
        {
            this.CloseConnection(GetConnection(connectionId));
        }

        public UdpSocketClientImplementation GetConnection(int connectionId)
        {

            lock (this)
            {
                if (ActiveConnections.ContainsKey(connectionId))
                {
                    return ActiveConnections[connectionId];
                }

                return null;
            }

        }

        // Deprecated
        private string ReshapeWebSocketUrl(string url)
        {
            UriBuilder uri = new UriBuilder(url);
            uri.Scheme = "ws";

            return uri.ToString();
        }

        public Dictionary<int, UdpSocketClientImplementation> GetPendingConnections()
        {
            return PendingConnections;
        }

        public Dictionary<int, UdpSocketClientImplementation> GetActiveConnections()
        {
            return ActiveConnections;
        }

        public void ActivatePendingConnection(UdpSocketClientImplementation connection)
        {
            lock (this)
            {
                PendingConnections.Remove(connection.Id);
                ActiveConnections.Add(connection.Id, connection);

            }
        }

        public void DropActiveConnection(UdpSocketClientImplementation connection)
        {
            lock (this)
            {
                connection.DropConnection();
                ActiveConnections.Remove(connection.Id);
                PendingConnections.Add(connection.Id, connection);
            }

        }
    }
}
