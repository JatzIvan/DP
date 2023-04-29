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

        public UdpSocketForCarConnection(string host, string port, int id, List<IObserver<VehicleObserverWrapper>> observers, float interval) : base(host, port, id, observers)
        {
            this.Interval = interval;

        }

        public UdpSocketForCarConnection(string host, string port, int id, List<IObserver<VehicleObserverWrapper>> observers, int keepAliveTimeout, float interval) : base(host, port, id, observers, keepAliveTimeout)
        {
            this.Interval = interval;

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
                //msg.Index = GetMessageIndex();
                msg.Interval = Interval;
                msg.Content = SubscribeContent.vehicles;
                //msg.Road = "503";
                //AddToMessageQueue(msg.Index, msg);
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

        public override void ActivateConnection()
        {

            base.ActivateConnection();
            Task.Run(() => Do(IsSubscribed));
            // Send Subscribe message
            // TODO: This is probably not the best idea, think this through
            /*while (!Subscribed && isAlive)
            {
                SendMessage(GetSubscribeMessage());
                Thread.Sleep(200);
            }*/
        }

    }

    public class SubscribeDataWrapper
    {
        public string RoadRef { get; set; }
    }
}
