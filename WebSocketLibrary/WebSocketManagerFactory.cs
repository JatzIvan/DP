using System;
using System.Collections.Generic;

namespace WebSocketLibrary
{
    public class WebSocketManagerFactory
    {

        private static Dictionary<string, WebSocketImplementation> ActiveConnections { get; set; } = new Dictionary<string, WebSocketImplementation>();
        private static WebSocketManagerFactory Instance;

        public static WebSocketManagerFactory GetInstance()
        {
            
            if(Instance == null)
            {
                Instance = new WebSocketManagerFactory();
            }

            return Instance;
        }

        public void CreateConnection(string url)
        {
            this.CreateConnection(url, url);

        }

        public WebSocketImplementation CreateConnection(string url, string name)
        {
            string trimmed = ReshapeWebSocketUrl(url);

            WebSocketImplementation ws = new WebSocketImplementation(new Uri(trimmed), name);

            ActiveConnections.Add(name, ws);

            return ws;

        }

        public void CloseConnection(WebSocketImplementation ws)
        {
            if(ws != null)
            {

                ActiveConnections.Remove(ws.Name);
                ws.CloseConnection();
            }
        }

        public void CloseConnection(string name)
        {
            this.CloseConnection(getConnection(name));
        }

        public WebSocketImplementation getConnection(string name)
        {
            if (ActiveConnections.ContainsKey(name))
            {
                return ActiveConnections[name];
            }

            return null;
        }

        private string ReshapeWebSocketUrl(string url)
        {
            UriBuilder uri = new UriBuilder(url);
            uri.Scheme = "ws";

            return uri.ToString();
        }

    }
}
