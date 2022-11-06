using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    /**
     * Socket blueprint, that contains all the necessary variables and methods to construct correct custom sockets
     */
    public abstract class AbstractSocket : IObservable<List<VehicleData>>
    {
        /**
         * All data observers attached to this socket
         */ 
        protected List<IObserver<List<VehicleData>>> registeredMessageHandlers = new List<IObserver<List<VehicleData>>>();
        protected bool isAlive = false;
        // Messages that need to be acknowledged
        protected Dictionary<int, AbstractMessage> messageQueue = new Dictionary<int, AbstractMessage>();
        public int Id { get; set; }
        public int keepAliveFailedAttempts = 0;

        // Return keep alive message. If message already present in queue (not acknowledged), then we have most likely lost connection.
        public KeepAliveMessage GetKeepAliveMessage()
        {

            Console.WriteLine("Num of messages " + messageQueue.Count);
            
            KeepAliveMessage msg = (KeepAliveMessage) messageQueue.Values.FirstOrDefault(a => typeof(KeepAliveMessage) == a.GetType());

            if (msg == null)
            {
                msg = new KeepAliveMessage();
                msg.Index = new Random().Next();
                AddToMessageQueue(msg.Index, msg);
            }
            else
            {
                keepAliveFailedAttempts++;
            }

            return msg;
        }

        /**
         * Method returns all messages that need to be (and were not) acknowledged
         */ 
        public List<AbstractMessage> GetAllUnconfirmedMessages()
        {
            List<AbstractMessage> msg = messageQueue.Values.Where(a => typeof(KeepAliveMessage) == a.GetType()).ToList();

            return msg;

        }

        public SubscribeMessage GetSubscribeMessage()
        {
            SubscribeMessage msg = (SubscribeMessage)messageQueue.Values.FirstOrDefault(a => typeof(SubscribeMessage) == a.GetType());

            if (msg == null)
            {
                msg = new SubscribeMessage();
                msg.Index = new Random().Next();
                msg.Interval = 200;
                msg.Content = SubscribeContent.vehicles;
                AddToMessageQueue(msg.Index, msg);
            }

            return msg;
        }

        public void AddToMessageQueue(int index, AbstractMessage msg)
        {
            messageQueue.Add(index, msg);
        }

        // Add unique observers to socket
        public IDisposable Subscribe(IObserver<List<VehicleData>> observer)
        {
            if (!registeredMessageHandlers.Contains(observer))
            {
                registeredMessageHandlers.Add(observer);
            }
            return new Unsubscriber<List<VehicleData>>(registeredMessageHandlers, observer);
        }

        public static string GetStringFromObject(object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize);
        }

        public static Byte[] ConvertMesssageToBytes(object objectToConvert)
        {
            return Encoding.ASCII.GetBytes(GetStringFromObject(objectToConvert));
        }

        private T DeserializeObject<T>(string msg)
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

        protected void ResolveMessageType(string receiveString)
        {

            AbstractMessage parsedObject = DeserializeObject<AbstractMessage>(receiveString);

            if(parsedObject == null)
            {
                return;
            }

            switch (parsedObject.Type)
            {
                case "acknowledge":
                    ResolveAckMessage(DeserializeObject<AcknowledgeMessage>(receiveString));
                    break;
                case "update_vehicles":
                    ResolveDataMessage(DeserializeObject<CarUpdateInfo>(receiveString));
                    break;
                default:
                    Console.WriteLine("Unknown message, ignoring");
                    break;
            }
        }

        // Remove message from queue that was acknowledged
        protected void ResolveAckMessage(AcknowledgeMessage msg)
        {

            if (!messageQueue.ContainsKey(msg.AcknowledgingIndex))
            {
                return;
            }

            if (messageQueue[msg.AcknowledgingIndex].GetType().Equals(typeof(SubscribeMessage)))
            {
                Console.WriteLine("Socket " + Id + " has established a connection");
                isAlive = true;
            }

            messageQueue.Remove(msg.AcknowledgingIndex); 
        }

        protected void ResolveDataMessage(CarUpdateInfo data)
        {
            if (data != null)
            {
                Stopwatch sw = Stopwatch.StartNew();
                foreach (IObserver<List<VehicleData>> handler in registeredMessageHandlers)
                {
                    handler.OnNext(data.Vehicles);
                }

                Console.WriteLine(sw.ElapsedMilliseconds);
            }
        }

        public void DropConnection()
        {
            this.isAlive = false;
            this.keepAliveFailedAttempts = 0;
            messageQueue.Clear();

        }
        
        public abstract void SendMessage(Byte[] msg);

        public abstract void CloseConnection();

        public abstract bool GetACK();
    }
}
