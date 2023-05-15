using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    public class UdpSocketForCarConnection : UdpSocketClientImplementation<VehicleObserverWrapper>
    {

        private SubscribeDataWrapper ConnectionData;
        private bool Subscribed = false;
        private float Interval;
        private DateTime LastReceivedMessageTime;
        private int KeepAliveTimetout;
        private Thread SubscriptionAliveThread;

        public UdpSocketForCarConnection(string host, string port, int id, List<IObserver<VehicleObserverWrapper>> observers, float interval) : this(host, port, id, observers, 6000, interval)
        {
        }

        public UdpSocketForCarConnection(string host, string port, int id, List<IObserver<VehicleObserverWrapper>> observers, int keepAliveTimeout, float interval) : base(host, port, id, observers, keepAliveTimeout)
        {
            this.Interval = interval;
            this.KeepAliveTimetout = keepAliveTimeout;
            SubscriptionAliveThread = new Thread(CheckLastMessageTime);
            SubscriptionAliveThread.Start();
        }

        protected override void ResolveMessageType(string receiveString)
        {
            AbstractMessage parsedObject = DeserializeObject<AbstractMessage>(receiveString);

            if (parsedObject == null)
            {
                return;
            }


            switch (parsedObject.Type)
            {
                case "update_vehicles":
                    LastReceivedMessageTime = DateTime.Now;
                    ResolveDataMessage(DeserializeObject<CarUpdateInfo>(receiveString));
                    break;
                default:
                    // Stream directly to abstract socket
                    base.ResolveMessageType(receiveString);
                    break;
            }
        }

        protected void ResolveDataMessage(CarUpdateInfo data)
        {
            if (data != null)
            {

                foreach (IObserver<VehicleObserverWrapper> handler in registeredMessageHandlers)
                {
                    handler.OnNext(new VehicleObserverWrapper(data, Id));

                }
            }
        }

        public SubscribeMessage GetSubscribeMessage()
        {
            SubscribeMessage msg = (SubscribeMessage)messageQueue.Values.FirstOrDefault(a => typeof(SubscribeMessage) == a.GetType());

            if (msg == null)
            {
                msg = new SubscribeMessage();
                msg.Interval = Interval;
                msg.Content = SubscribeContent.vehicles;
            }

            return msg;
        }

        // Handle socket subscription
        protected override void HandleCustomAckMessageLogic(AbstractMessage msg)
        {

            if (msg.GetType().Equals(typeof(SubscribeMessage)))
            {
                Console.WriteLine("Socket " + Id + " starts recieving data");
                Subscribed = true;
                LastReceivedMessageTime = DateTime.Now;
            }

            if (msg.GetType().Equals(typeof(UnsubscribeMessage)))
            {
                Console.WriteLine("Socket " + Id + " was unsubscribed");
                ConnectionCleanup();
            }

        }

        public override void DropConnection()
        {
            Subscribed = false;
            base.DropConnection();

        }

        public bool IsSubscribed()
        {

            if (!isAlive)
            {
                return false;
            }

            if (!Subscribed)
            {
                SendMessageWithAck(GetSubscribeMessage());
                //throw new Exception();
            }

            return Subscribed;
        }

        public void Subscribe()
        {
            Task.Run(() => Do(IsSubscribed));
        }

        public override void ActivateConnection()
        {

            base.ActivateConnection();
            Subscribe();
        }

        private void CheckLastMessageTime()
        {
            try
            {
                while (true)
                {

                    if (Subscribed)
                    {

                        if((DateTime.Now - LastReceivedMessageTime).TotalMilliseconds > 5 * KeepAliveTimetout)
                        {
                            Console.WriteLine("Socket " + this.Id + " lost vehicle update subscription");
                            Subscribed = false;
                            Subscribe();
                        }

                    }
                    //}

                    Thread.Sleep(KeepAliveTimetout);
                }
            }
            catch (Exception)
            {
            }
        }

    }

    public class SubscribeDataWrapper
    {
        public string RoadRef { get; set; }
    }
}
