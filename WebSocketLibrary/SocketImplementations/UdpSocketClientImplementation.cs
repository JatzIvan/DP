using CoreLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
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

        private ConcurrentQueue<AbstractMessage> messagesQueue;

        private Thread sendingThread;
        private Thread keepAliveThread;

        public struct UdpState
        {
            public UdpClient client;
            public IPEndPoint endpoint;
            public int index;
        }

        private UdpClient Client;

        private IPEndPoint EP;

        private int keepAliveTimeout;

        public UdpSocketClientImplementation(string host, string port, int id, List<IObserver<T>> observers)
            : this(host, port, id, observers, 10000)
        {

        }

        public UdpSocketClientImplementation(string host, string port, int id, List<IObserver<T>> observers, int keepAliveTimeout) : base(host, port, id)
        {
            if (host is null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (port is null)
            {
                throw new ArgumentNullException(nameof(port));
            }

            if(keepAliveTimeout <= 0)
            {
                keepAliveTimeout = 10000;
            }

            messagesQueue = new ConcurrentQueue<AbstractMessage>();

            observers.ForEach(h =>
            {
                Subscribe(h);
            });
            this.Id = id;
            this.keepAliveTimeout = keepAliveTimeout;
            //EP = new IPEndPoint(IPAddress.Parse(host), Int32.Parse(port)); // endpoint where server is listening

            // Resolve docker address when needed
            IPAddress ip = Uri.CheckHostName(host).Equals(UriHostNameType.Dns) ? 
                Dns.GetHostEntry(host).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork) :
                IPAddress.Parse(host);
            EP = new IPEndPoint(ip, Int32.Parse(port));
            this.Client = CreateSocket(new IPEndPoint(IPAddress.Any, 0));
            //this.Client.Connect(EP);
            StartListening();

            sendingThread = new Thread(SendMessages);
            sendingThread.Start();

            keepAliveThread = new Thread(KeepAlive);
            keepAliveThread.Start();

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
                Logger.GetLogger().WriteLine("Trying to connect socket " + Id);
                HandleHandShake();
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
                // Setup connection with data server
                isAlive = checkIfAlive();

                // Start listening for data stream
                StartListening();

                return true;
            }
            catch (SocketException e)
            {
                Logger.GetLogger().WriteLine(e.ToString());
                Logger.GetLogger().WriteLine("Try again after 10 seconds");
                return false;
            }
        }

        public void StartListening()
        {
            UdpState state = new UdpState();
            state.client = Client;
            state.endpoint = EP;


            Client.BeginReceive(new AsyncCallback(Ws_HandleMessage), state);
            
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
            catch (Exception)
            {
                //Logger.GetLogger().WriteLine(e);
            }

            string receiveString = Encoding.ASCII.GetString(receiveBytes);
                ResolveMessageType(receiveString);


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
            catch (Exception)
            {
                Logger.GetLogger().WriteLine("Socket was closed, stopping receive");
            }

        }

        /**
         * Save message into queue and send when possible.
         * This is necessary because there are multiple threads that send messages
         */
        public override AbstractMessage SendMessage(AbstractMessage msg)
        {
            msg.Index = GetMessageIndex();
            messagesQueue.Enqueue(msg);

            return msg;

        }
        
        public override void CloseConnection()
        {

            SendCloseMessage();

        }

        public override void ConnectionCleanup()
        {
            base.ConnectionCleanup();
            registeredMessageHandlers.ForEach(handler =>
            {
                handler.OnCompleted();
            });
            registeredMessageHandlers.Clear();
            Client.Close();
            // Close thread for sending messages
            Logger.GetLogger().WriteLine("Closing thread");
            sendingThread.Interrupt();
            keepAliveThread.Interrupt();
        }

        // Inform Other side that connection is closing
        private void SendCloseMessage()
        {
            UnsubscribeMessage msg = new UnsubscribeMessage();

            //Byte[] sendBytes = ConvertMesssageToBytes(msg);

            SendMessageWithAck(msg);
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

        private void SendMessages()
        {
            try
            {
                while (true)
                {
                    while (messagesQueue.TryDequeue(out AbstractMessage message))
                    {
                        if (checkIfAlive())
                        {
                            Byte[] msgInBytes = ConvertMesssageToBytes(message);
                            Client.Send(msgInBytes, msgInBytes.Length, EP);
                        }
                    }
                    Thread.Sleep(10);
                }
            }catch (Exception) { 
            }
        }

        public void KeepAlive()
        {
            try
            {
                while (true)
                {
                    if (checkIfAlive())
                    {
                        KeepAliveMessage msg = this.GetKeepAliveMessage();

                        if (this.keepAliveFailedAttempts > 5)
                        {
                            Logger.GetLogger().WriteLine("Socket " + this.Id + " has lost connection");
                            WebSocketManagerFactory.GetInstance().DropActiveConnection(this);
                        }
                        else
                        {
                            Logger.GetLogger().WriteLine("Sending Keepalive message");
                            this.SendMessageWithAck(msg);
                        }
                    }

                    Thread.Sleep(keepAliveTimeout);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
