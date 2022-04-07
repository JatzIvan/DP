using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using WebSocketLibrary.Models;
using WebSocketSharp.NetCore;

namespace WebSocketLibrary
{
    public class WebSocketImplementation : IObservable<CarUpdateInfoWrapper>
    {

        private WebSocket ws;
        public string Name { get; set; }

        private List<IObserver<CarUpdateInfoWrapper>> registeredMessageHandlers = new List<IObserver<CarUpdateInfoWrapper>>();

        public WebSocketImplementation(Uri uri, string name)
        {
            this.Name = name;
            // Handle connection
            this.ws = new WebSocket(uri.ToString());
            ws.OnMessage += Ws_HandleMessage;
            ws.Connect();
        }
        
        /**
         * Function handles incomming messages
         */
        private void Ws_HandleMessage(object sender, MessageEventArgs args)
        {
            if (args.Data != null)
            {
                CarUpdateInfoWrapper parsedObject = null;
                try
                {
                    parsedObject = JsonConvert.DeserializeObject<CarUpdateInfoWrapper>(args.Data);
                }catch(Exception e)
                {
                    Console.WriteLine("Error occured while parsing Incomming message");
                    Console.WriteLine(e.ToString());
                    //TODO setup log with all incidents
                }

                if(parsedObject != null)
                {
                    foreach(IObserver<CarUpdateInfoWrapper> handler in registeredMessageHandlers)
                    {
                        handler.OnNext(parsedObject);
                    }
                }

            }
        }

        private bool IsAlive(int attempt)
        {
            if (!ws.IsAlive)
            {
                ws.Connect();
            }

            if (!ws.IsAlive && attempt < 10)
            {
                IsAlive(attempt + 1);
            }

            return ws.IsAlive;

        }

        public void SendMessage()
        {

            if (IsAlive(0))
            {
                //ws.SendAsync();
            }

        }

        public void CloseConnection()
        {

            if (ws.IsAlive)
            {
                registeredMessageHandlers.ForEach(handler =>
                {
                    handler.OnCompleted();
                });
                registeredMessageHandlers.Clear();
                ws.CloseAsync();
            }
        }

        public IDisposable Subscribe(IObserver<CarUpdateInfoWrapper> observer)
        {
            if (!registeredMessageHandlers.Contains(observer))
            {
                registeredMessageHandlers.Add(observer);
            }
            return new Unsubscriber<CarUpdateInfoWrapper>(registeredMessageHandlers, observer);
        }

    }
}
