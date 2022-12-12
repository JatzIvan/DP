using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebSocketLibrary.Models;
using WebSocketSharp.NetCore;

namespace WebSocketLibrary
{
    /**
     * UDP Connections implementation that adds some TCP overhead
     * Because UDP sockets are connectionless, we need to create a custom overhead to connect(handshake), keepalive, acknowledge and close connection
     * 
     */
    public abstract class UdpSocketClientImplementation<T> : AbstractSocket, IObservable<T> where T: ObserverWrapper
    {

        /**
         * All data observers attached to this socket
         */
        protected List<IObserver<T>> registeredMessageHandlers = new List<IObserver<T>>();

        public struct UdpState
        {
            public UdpClient client;
            public IPEndPoint endpoint;
            public int index;
        }

        private UdpClient Client;

        private IPEndPoint EP;

        public UdpSocketClientImplementation(string host, string port, int id, List<IObserver<T>> observers) : base(host, port, id)
        {
            if (host is null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (port is null)
            {
                throw new ArgumentNullException(nameof(port));
            }

            observers.ForEach(h =>
            {
                Subscribe(h);
            });
            this.Id = id;
            //EP = new IPEndPoint(IPAddress.Parse(host), Int32.Parse(port)); // endpoint where server is listening

            IPAddress ip = Uri.CheckHostName(host).Equals(UriHostNameType.Dns) ? 
                Dns.GetHostEntry(host).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork) :
                IPAddress.Parse(host);
            EP = new IPEndPoint(ip, Int32.Parse(port));
            this.Client = CreateSocket(new IPEndPoint(IPAddress.Any, 0));
            //this.Client.Connect(EP);
            StartListening();
        }

        // Add unique observers to socket
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!registeredMessageHandlers.Contains(observer))
            {
                registeredMessageHandlers.Add(observer);
            }
            return new Unsubscriber<T>(registeredMessageHandlers, observer);
        }

        // Wait until socket is connected
        protected override bool Connect()
        {
            if (!isAlive)
            {
                Console.WriteLine("Trying to connect socket " + Id);
                HandleHandShake();
                //throw new Exception("Not connected yet");
            }

            return isAlive;

        }

        /**
         *  Figure out some type of configuration
         */
        public bool CreateConnectionWithDataSocket() 
        {
            try
            {

                //Create Subscribe Message (for now it is hardcoded)
                //SubscribeMessage firstMsg = new SubscribeMessage();
                //firstMsg.Interval = 200;
                //firstMsg.Content = SubscribeContent.vehicles;



                // Setup connection with data server
                isAlive = checkIfAlive();

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
            state.endpoint = EP;
            /**
             * Handle Errors
             */

            //if (checkIfAlive())
            //{
            Client.BeginReceive(new AsyncCallback(Ws_HandleMessage), state);
            //}
            
        }

        private UdpClient CreateSocket(IPEndPoint socketEp)
        {
            UdpClient localClient = new UdpClient(socketEp);
            // This is required, because when UDP socket looses connection (wtf?), then c# will throw exceptions
            // This does not make much sense
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                localClient.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
            }
            localClient.EnableBroadcast = true;
            return localClient;
        }
        

        /**
         * Function handles incomming messages
         */
        public void Ws_HandleMessage(IAsyncResult ar)
        {

            UdpClient client = ((UdpState)(ar.AsyncState)).client;
            IPEndPoint endpoint = ((UdpState)(ar.AsyncState)).endpoint;

            if(client.Client == null)
            {
                return;
            }

            byte[] receiveBytes = new byte[0];
            try
            {
                receiveBytes = client.EndReceive(ar, ref endpoint);
            }
            catch(Exception e)
            {
                //Console.WriteLine(e);
            }



            //Console.WriteLine("Socket " + Id + " has recieved data");

            //if (receiveBytes.Length >= 4)
            //{
            string receiveString = Encoding.ASCII.GetString(receiveBytes);
                ResolveMessageType(receiveString);
            //}


            UdpState state = new UdpState();
            state.client = client;
            state.endpoint = endpoint;

            if (client == null || client.Client == null)
            {
                return;
            }

            try
            {
                client.BeginReceive(new AsyncCallback(Ws_HandleMessage), state);

            }
            catch (Exception e)
            {
                Console.WriteLine("Socket was closed, stopping receive");
            }

        }

        public override AbstractMessage SendMessage(AbstractMessage msg)
        {
            msg.Index = GetMessageIndex();
            if (checkIfAlive())
            {
                Byte[] msgInBytes = ConvertMesssageToBytes(msg);
                Client.Send(msgInBytes, msgInBytes.Length, EP);
            }

            return msg;

        }
        
        public override void CloseConnection()
        {

            registeredMessageHandlers.ForEach(handler =>
            {
                handler.OnCompleted();
            });
            registeredMessageHandlers.Clear();

            SendCloseMessage();
            //Client.Client.Shutdown(SocketShutdown.Both);
            Client.Close();

        }                                                      

        // Inform Other side that connection is closing
        private void SendCloseMessage()
        {
            UnsubscribeMessage msg = new UnsubscribeMessage();

            //Byte[] sendBytes = ConvertMesssageToBytes(msg);

            SendMessage(msg);
        }

        public void HandleHandShake()
        {
            //Create Subscribe Message (for now it is hardcoded)
            ConnectMessage firstMsg = GetConnectMessage();

            AddToMessageQueue(firstMsg.Index, firstMsg);
            Byte[] sendBytes = ConvertMesssageToBytes(firstMsg);
            Client.Send(sendBytes, sendBytes.Length, EP);

        }

        public bool checkIfAlive()
        {
            return isAlive;
        }

        protected override void HandleCustomAckMessageLogic(AbstractMessage msg)
        {
            return;
        }
    }
}
