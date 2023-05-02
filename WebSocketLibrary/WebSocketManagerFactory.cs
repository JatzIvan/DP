using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    /**
     * Simple factory class that handles all current udp socket "connections"
     * Each "connection" is stored in dictionary so it is easily accessible with ID
     */
    public class WebSocketManagerFactory
    {

        // Connections that
        private static Dictionary<int, AbstractSocket> ActiveConnections { get; set; } = new Dictionary<int, AbstractSocket>();
        
        // Connections that have established first connections (without subscribe)
        private static Dictionary<int, AbstractSocket> PendingConnections { get; set; } = new Dictionary<int, AbstractSocket>();

        // Connections that are not connected to endpoint
        // private static Dictionary<int, UdpSocketClientImplementation> RegisteredConnections { get; set; } = new Dictionary<int, UdpSocketClientImplementation>();

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

        public T CreateConnection<T, G>(string url, string port, List<IObserver<G>> handler) 
            where T : AbstractSocket
            where G : ObserverWrapper
        {
           return this.CreateConnection<T,G>(url, port, new Random().Next(), handler);
        }

        public T CreateConnection<T>(T ws)
        where T : AbstractSocket
        {
            return StartConnection(ws.Id, ws);
        }

        private T StartConnection<T>(int id, T ws) where T : AbstractSocket
        {
            PendingConnections.Add(id, ws);

            Task.Run(ws.EstablishConnection);

            return ws;
        }

        public T CreateConnection<T, G>(string host, string port, int id, List<IObserver<G>> handler)
        where T : AbstractSocket
        where G : ObserverWrapper
        {
            //string trimmed = ReshapeWebSocketUrl(url);

            //T ws = new T(host, port, id);
            T ws = (T)Activator.CreateInstance(typeof(T), new object[] { host, port, id, handler });

            return StartConnection(id, ws);

        }

        /**
         * Create new connection to host and port based on T {SocketType} and G {Message Type}
         */
        public T CreateConnection<T,G>(string host, string port, int id, List<IObserver<G>> handler, int keepAliveTimeout) 
            where T: AbstractSocket
            where G: ObserverWrapper
        {
            //string trimmed = ReshapeWebSocketUrl(url);

            //T ws = new T(host, port, id);
            T ws = (T)Activator.CreateInstance(typeof(T), new object[] { host, port, id, handler, keepAliveTimeout });

            return StartConnection(id, ws);

        }

        /*public bool OpenConnection(AbstractSocket connection, List<IObserver<ObserverWrapper>> handler)
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
        }*/

        public void CloseConnection(AbstractSocket ws)
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

        public AbstractSocket GetConnection(int connectionId)
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

        public Dictionary<int, AbstractSocket> GetPendingConnections()
        {
            return PendingConnections;
        }

        public Dictionary<int, AbstractSocket> GetActiveConnections()
        {
            return ActiveConnections;
        }

        public void ActivatePendingConnection(AbstractSocket connection)
        {
            lock (this)
            {
                PendingConnections.Remove(connection.Id);
                ActiveConnections.Add(connection.Id, connection);

            }
        }

        public void DropActiveConnection(AbstractSocket connection)
        {
            lock (this)
            {
                ActiveConnections.Remove(connection.Id);
                PendingConnections.Add(connection.Id, connection);
                connection.DropConnection();
            }

        }
    }
}
