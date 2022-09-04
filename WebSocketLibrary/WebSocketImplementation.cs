using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
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

/*        private static ManualResetEvent connectDone =
    new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);*/

        private UdpClient Client;

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
            /*ep = new IPEndPoint(IPAddress.Parse(host), Int32.Parse(port)); */// endpoint where server is listening
            ep = new IPEndPoint(IPAddress.Parse(host), Int32.Parse(port));
            this.Client = CreateSocket(new IPEndPoint(IPAddress.Any, 1111));
        }

        /**
         *  Figure out some type of configuration
         */
        public bool CreateConnectionWithDataSocket() 
        {

            try
            {
                //Create Subscribe Message (for now it is hardcoded)
                SubscribeMessage firstMsg = new SubscribeMessage();
                firstMsg.Interval = 200;
                firstMsg.Content = SubscribeContent.vehicles;
                firstMsg.ClientPort = 1111;


                // Setup connection with data server
                HandleHandShake(firstMsg);

                // Start listening for data stream
                StartListening();

                return true;
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Try again after 10 seconds");
                return false;
            }
        }

        public void StartListening()
        {
            UdpState state = new UdpState();
            state.client = Client;
            state.endpoint = ep;
            /**
             * Handle Errors
             */

            Client.BeginReceive(new AsyncCallback(Ws_HandleMessage), state);
            
        }

        private Byte[] ConvertMesssageToBytes(object objectToConvert)
        {
            return Encoding.ASCII.GetBytes(GetStringFromObject(objectToConvert));
        }
        
        private UdpClient CreateSocket(IPEndPoint socketEp)
        {
            UdpClient localClient = new UdpClient(socketEp);
            localClient.EnableBroadcast = true;
            return localClient;
        }

        private void HandleHandShake(SubscribeMessage msg)
        {
            Byte[] sendBytes = ConvertMesssageToBytes(msg);

            SendMessage(sendBytes);
        }

        private string GetStringFromObject(object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize);
        }

        /**
         * Function handles incomming messages
         */
        public void Ws_HandleMessage(IAsyncResult ar)
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
            state.client = client;
            state.endpoint = endpoint;
            client.BeginReceive(new AsyncCallback(Ws_HandleMessage), state);
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
            Client.Send(msg, msg.Length, ep);
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
            Client.Close();
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
