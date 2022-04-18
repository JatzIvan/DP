using System;
using System.Collections.Generic;

namespace WebSocketLibrary
{
    public class WebSocketManagerFactory
    {

        private static Dictionary<string, UdpSocketClientImplementation> ActiveConnections { get; set; } = new Dictionary<string, UdpSocketClientImplementation>();
        private static WebSocketManagerFactory Instance;

        public static WebSocketManagerFactory GetInstance()
        {
            
            if(Instance == null)
            {
                Instance = new WebSocketManagerFactory();
            }

            return Instance;
        }

        public void CreateConnection(string url, string port)
        {
            this.CreateConnection(url, port, url);

        }

        public UdpSocketClientImplementation CreateConnection(string host, string port, string name)
        {
            //string trimmed = ReshapeWebSocketUrl(url);

            UdpSocketClientImplementation ws = new UdpSocketClientImplementation(host, port, name);

            ActiveConnections.Add(name, ws);

            return ws;

        }

        public void CloseConnection(UdpSocketClientImplementation ws)
        {
            if(ws != null)
            {

                ActiveConnections.Remove(ws.Name);
                ws.CloseConnection();
            }
        }

        public void CloseConnection(string name)
        {
            this.CloseConnection(GetConnection(name));
        }

        public UdpSocketClientImplementation GetConnection(string name)
        {
            if (ActiveConnections.ContainsKey(name))
            {
                return ActiveConnections[name];
            }

            return null;
        }

        // Deprecated
        private string ReshapeWebSocketUrl(string url)
        {
            UriBuilder uri = new UriBuilder(url);
            uri.Scheme = "ws";

            return uri.ToString();
        }

    }
}
