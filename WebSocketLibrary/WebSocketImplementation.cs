using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using WebSocketLibrary.Models;
using WebSocketSharp.NetCore;

namespace WebSocketLibrary
{
    public class UdpSocketClientImplementation : IObservable<CarUpdateInfoWrapper>
    {

        public struct UdpState
        {
            public UdpClient client;
            public IPEndPoint endpoint;
        }

        private UdpClient ws;

        private IPEndPoint ep;
        
        public string Name { get; set; }

        private List<IObserver<CarUpdateInfoWrapper>> registeredMessageHandlers = new List<IObserver<CarUpdateInfoWrapper>>();

        public UdpSocketClientImplementation(string host, string port, string name)
        {
            if (host is null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (port is null)
            {
                throw new ArgumentNullException(nameof(port));
            }

            this.Name = name;
            // Handle connection
            ep = new IPEndPoint(IPAddress.Parse(host), Int32.Parse(port)); // endpoint where server is listening
            this.ws = new UdpClient();

            UdpState state = new UdpState();
            state.client = ws;
            state.endpoint = ep;

            Console.WriteLine(ws.Client.Connected);
            Console.WriteLine(ws.Client);

            HandleHandShake();

            ws.BeginReceive(new AsyncCallback(Ws_HandleMessage), state);

            //ws.OnMessage += Ws_HandleMessage;
            //ws.Connect();
        }

        private Byte[] ConvertMesssageToBytes(object objectToConvert)
        {
            return Encoding.ASCII.GetBytes(GetStringFromObject(objectToConvert));
        }
        
        private void HandleHandShake()
        {

            SubscribeMessage firstMsg = new SubscribeMessage();
            firstMsg.Interval = 200;
            firstMsg.Content = SubscribeContent.vehicles;

            Byte[] sendBytes = ConvertMesssageToBytes(firstMsg);

            SendMessage(sendBytes);
        }

        private string GetStringFromObject(object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize);
        }

        /**
         * Function handles incomming messages
         */
        private void Ws_HandleMessage(IAsyncResult ar)
        {
            Console.WriteLine("HELLO");
            UdpClient client = ((UdpState)(ar.AsyncState)).client;
            IPEndPoint endpoint = ((UdpState)(ar.AsyncState)).endpoint;


            byte[] receiveBytes = client.EndReceive(ar, ref endpoint);

            if (receiveBytes.Length >= 4)
            {
                string receiveString = Encoding.ASCII.GetString(receiveBytes);
                CarUpdateInfoWrapper parsedObject = null;
                try
                {
                    parsedObject = JsonConvert.DeserializeObject<CarUpdateInfoWrapper>(receiveString);
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


            UdpState state = new UdpState();
            state.client = ws;
            state.endpoint = ep;
            ws.BeginReceive(new AsyncCallback(Ws_HandleMessage), state);
        }

       /* private bool IsAlive(int attempt)
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

        }*/

        public void SendMessage(Byte[] msg)
        {

            ws.Connect(ep.Address, ep.Port);
            ws.Send(msg, msg.Length);
            //ws.Close();

            /*if (IsAlive(0))
            {
                //ws.SendAsync();
            }*/

        }

        public void CloseConnection()
        {

            //if (ws.IsAlive)
            //{
            registeredMessageHandlers.ForEach(handler =>
            {
                handler.OnCompleted();
                });
            registeredMessageHandlers.Clear();
            ws.Close();
            //}
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
