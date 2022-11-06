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
using WebSocketLibrary.Models;
using WebSocketSharp.NetCore;

namespace WebSocketLibrary
{
    /**
     * UDP Connections implementation that adds some TCP overhead
     * Because UDP sockets are connectionless, we need to create a custom overhead to connect(handshake), keepalive, acknowledge and close connection
     * 
     */
    public class UdpSocketClientImplementation : AbstractSocket
    { 

        public struct UdpState
        {
            public UdpClient client;
            public IPEndPoint endpoint;
            public int index;
        }

        private UdpClient Client;

        private IPEndPoint EP;

        public UdpSocketClientImplementation(string host, string port, int id)
        {
            if (host is null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (port is null)
            {
                throw new ArgumentNullException(nameof(port));
            }

            this.Id = id;
            EP = new IPEndPoint(IPAddress.Parse(host), Int32.Parse(port)); // endpoint where server is listening
            //IPAddress ip = Dns.GetHostEntry("integration_module").AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            //EP = new IPEndPoint(ip, Int32.Parse(port));
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
        

        // Wait for acknowledgement
        // It is a synchronous wait so thread will wait until it is finished
        // We do not care about the content so we just return bool depending if ack arrived
        public override bool GetACK()
        {
            // Wait for 5 secs
            Client.Client.ReceiveTimeout = 5000;

            Byte[] data = Client.Receive(ref EP);

            string receiveString = Encoding.ASCII.GetString(data);
            AcknowledgeMessage parsedObject = null;
            try
            {
                parsedObject = JsonConvert.DeserializeObject<AcknowledgeMessage>(receiveString);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured while parsing Incomming message");
                Console.WriteLine(e.ToString());
                //TODO setup log with all incidents
            }

            if (parsedObject != null)
            {
                return true;
            }

            return false;
        }

        /**
         * Function handles incomming messages
         */
        public void Ws_HandleMessage(IAsyncResult ar)
        {

            UdpClient client = ((UdpState)(ar.AsyncState)).client;
            IPEndPoint endpoint = ((UdpState)(ar.AsyncState)).endpoint;

            byte[] receiveBytes = new byte[0];
            try
            {
                receiveBytes = client.EndReceive(ar, ref endpoint);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Socket " + Id + " has recieved data");

            //if (receiveBytes.Length >= 4)
            //{
                string receiveString = Encoding.ASCII.GetString(receiveBytes);
                ResolveMessageType(receiveString);
            //}


            UdpState state = new UdpState();
            state.client = client;
            state.endpoint = endpoint;
            client.BeginReceive(new AsyncCallback(Ws_HandleMessage), state);
        }

        public override void SendMessage(Byte[] msg)
        {
            if (checkIfAlive())
            {
                Client.Send(msg, msg.Length, EP);
            }
            
        }
        
        public override void CloseConnection()
        {

            registeredMessageHandlers.ForEach(handler =>
            {
                handler.OnCompleted();
            });
            registeredMessageHandlers.Clear();

            SendCloseMessage();
            Client.Close();

        }                                                      

        // Inform Other side that connection is closing
        private void SendCloseMessage()
        {
            UnsubscribeMessage msg = new UnsubscribeMessage();

            Byte[] sendBytes = ConvertMesssageToBytes(msg);

            SendMessage(sendBytes);
        }

        public void HandleHandShake()
        {
            //Create Subscribe Message (for now it is hardcoded)
            SubscribeMessage firstMsg = GetSubscribeMessage();

            //do
            //{
              //  Console.WriteLine("Trying to Connect, total retries = " + totalWait);
              //  Thread.Sleep(wait);
                Byte[] sendBytes = ConvertMesssageToBytes(firstMsg);
                Client.Send(sendBytes, sendBytes.Length, EP);
            //  wait = 10000;
            //  totalWait++;

            //} while (GetACK());

            //return GetACK();
        }

        public bool checkIfAlive()
        {
            return isAlive;

           /* if (!isAlive)
            {


                HandleHandShake(firstMsg);

            }

            return isAlive;*/
        }
    }
}
