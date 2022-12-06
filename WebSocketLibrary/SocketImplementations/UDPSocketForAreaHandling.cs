using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebSocketLibrary.Models;

namespace WebSocketLibrary.SocketImplementations
{

    /**
     * This class needs some refactoring, mostly because there is problem with
     */
    public class UDPSocketForAreaHandling : UdpSocketClientImplementation<AreaObserverWrapper>
    {

        AreaMessage msg = null;

        public UDPSocketForAreaHandling(string host, string port, int id, List<IObserver<AreaObserverWrapper>> observers) : base(host, port, id, observers)
        {

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
                case "area":
                    ResolveAreaMessage(DeserializeObject<AreaMessage>(receiveString));
                    break;
                default:
                    // Stream directly to abstract socket
                    base.ResolveMessageType(receiveString);
                    break;
            }
        }

        protected void ResolveAreaMessage(AreaMessage data)
        {
            if (data != null)
            {

                msg = data;
            }
        }

        private AreaMessage GetArea()
        {

            if (!isAlive)
            {
                return null;
            }

            if(msg == null)
            {
                Console.WriteLine("Requesting Area");
                SendMessage(new RequestAreaMessage());
                return msg;
            }

            return msg;

        }

        public AreaMessage Repeat(Func<AreaMessage> connect)
        {
            while (true)
            {
                AreaMessage val = connect();

                if (val != null)
                {
                    break;
                }
                Thread.Sleep(1000);
            }

            return msg;

        }

        // This is sync so application can correctly continue
        public AreaMessage AreaFetched()
        {
            Task.WaitAll(Task.Run(() =>
            {
                while (!isAlive){}
                return;
            }));
            Task.WaitAll(Task.Run(() => Repeat(GetArea)));
            return msg;

        }
    }
}