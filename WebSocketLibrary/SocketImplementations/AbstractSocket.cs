using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    /**
     * Socket blueprint, that contains all the necessary variables and methods to construct correct custom sockets
     */
    public abstract class AbstractSocket
    {

        protected bool isAlive = false;
        // Messages that need to be acknowledged
        protected Dictionary<int, AbstractMessage> messageQueue = new Dictionary<int, AbstractMessage>();
        public int Id { get; set; }
        public int keepAliveFailedAttempts = 0;

        private int MessageIndex = 0;

        public AbstractSocket(string host, string port, int id)
        {

        }

        /**
         * Return keep alive message. If message already present in queue (not acknowledged), then we have most likely lost connection.
         */
        public KeepAliveMessage GetKeepAliveMessage()
        {

            // Console.WriteLine("Num of messages " + messageQueue.Count);

            KeepAliveMessage msg = (KeepAliveMessage)messageQueue.Values.FirstOrDefault(a => typeof(KeepAliveMessage) == a.GetType());

            if (msg == null)
            {
                msg = new KeepAliveMessage();
                keepAliveFailedAttempts = 0;
                //msg.Index = GetMessageIndex();
                //AddToMessageQueue(msg.Index, msg);
            }
            else
            {
                keepAliveFailedAttempts++;
            }

            return msg;
        }

        /**
         * Method returns all messages that need to be (and were not) acknowledged
         * This method will skip KeepAlive And ConnectMessages.
         */
        public List<AbstractMessage> GetAllUnconfirmedMessages()
        {
            List<AbstractMessage> msg = messageQueue.Values.Where(a => typeof(KeepAliveMessage) != a.GetType()
            && typeof(ConnectMessage) != a.GetType()).ToList();

            return msg;

        }

        /**
         * Create or get connect message from queue
         */
        public ConnectMessage GetConnectMessage()
        {
            ConnectMessage msg = (ConnectMessage)messageQueue.Values.FirstOrDefault(a => typeof(ConnectMessage) == a.GetType());

            if (msg == null)
            {
                msg = new ConnectMessage();
            }

            return msg;
        }

        /**
         * Add message to queue to be acknowledged
         */
        public void AddToMessageQueue(int index, AbstractMessage msg)
        {
            if (!messageQueue.ContainsKey(index))
            {
                messageQueue.Add(index, msg);
            }
        }

        public static string GetStringFromObject(object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize);
        }

        public static Byte[] ConvertMesssageToBytes(object objectToConvert)
        {
            return Encoding.ASCII.GetBytes(GetStringFromObject(objectToConvert));
        }

        protected T DeserializeObject<T>(string msg)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured while parsing Incomming message");
                Console.WriteLine(e.ToString());
                //TODO setup log with all incidents
            }

            return default(T);

        }

        /**
         * Method determines which type of message was recieved and handles it accordingly
         */
        protected virtual void ResolveMessageType(string receiveString)
        {

            AbstractMessage parsedObject = DeserializeObject<AbstractMessage>(receiveString);

            if (parsedObject == null)
            {
                return;
            }

            //Console.WriteLine(parsedObject.Type);

            switch (parsedObject.Type)
            {
                case "acknowledge":
                    ResolveAckMessage(DeserializeObject<AcknowledgeMessage>(receiveString));
                    break;
                default:
                    Console.WriteLine("Unknown message, ignoring");
                    break;
            }
        }

        /**
         * Remove message from queue that was acknowledged
         */
        protected void ResolveAckMessage(AcknowledgeMessage msg)
        {

            if (!messageQueue.ContainsKey(msg.AcknowledgingIndex))
            {
                return;
            }

            Console.WriteLine("Received ack");

            AbstractMessage queueMessage = messageQueue[msg.AcknowledgingIndex];
            messageQueue.Remove(msg.AcknowledgingIndex);

            if (queueMessage.GetType().Equals(typeof(ConnectMessage)))
            {
                Console.WriteLine("Socket " + Id + " has established a connection");
                ActivateConnection();
            }

            //Handle Custom Logic if necessary
            HandleCustomAckMessageLogic(queueMessage);

        }

        protected abstract void HandleCustomAckMessageLogic(AbstractMessage msg);

        /**
         * Connection is dropped when keep alive messages are not ack
         */
        public virtual void DropConnection()
        {
            this.isAlive = false;
            this.keepAliveFailedAttempts = 0;
            messageQueue.Clear();
            // Try to establish connection again
            Task.Run(this.EstablishConnection);
        }

        public int GetMessageIndex()
        {
            lock (this)
            {
                return MessageIndex++;
            }
        }

        public virtual void ActivateConnection()
        {
            WebSocketManagerFactory.GetInstance().ActivatePendingConnection(this);
            isAlive = true;
            MessageIndex = 1;
        }

        protected abstract bool Connect();

        public bool Do(Func<bool> connect)
        {
            while (true)
            {
                bool val = connect();

                if (val)
                {
                    break;
                }
                Thread.Sleep(1000);
            }

            return true;

        }

        public async Task<bool> EstablishConnection()
        {

            if (isAlive)
            {
                return true;
            }

            return await Task.Run(() => Do(Connect));
        }

        public abstract AbstractMessage SendMessage(AbstractMessage msg);

        public AbstractMessage SendMessageWithAck(AbstractMessage msg)
        {

            // Remap message from queue
            if (messageQueue.ContainsKey(msg.Index))
            {
                messageQueue.Remove(msg.Index);
            }

            AbstractMessage sendMessage = SendMessage(msg);

            AddToMessageQueue(sendMessage.Index, sendMessage);

            return sendMessage;
        }

        public abstract void CloseConnection();

        public virtual void ConnectionCleanup()
        {
            isAlive = false;
        }
    }
}
