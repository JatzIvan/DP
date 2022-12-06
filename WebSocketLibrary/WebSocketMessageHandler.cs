using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketLibrary.Models;

namespace WebSocketLibrary
{
    public class WebSocketMessageHandler<T> : IObserver<T> where T: ObserverWrapper
    {

        private string Name;
        private IDisposable Unsubscriber;
        private IMessageHandler<T> CollisionDetector;

        // TODO: add calculation templates
        public WebSocketMessageHandler(string name, IMessageHandler<T> collisionDetector)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Handler is missing a name");
            }

            this.CollisionDetector = collisionDetector;
            this.Name = name;
        }

        /**
         * Handle socket finish without application crash
         */
        public virtual void OnCompleted()
        {
            throw new NotImplementedException();
        }

        /**
         * Handle errors without application crash
         */
        public virtual void OnError(Exception error)
        {
            Console.WriteLine("Error occured during data handling");
        }

        public virtual void OnNext(T value)
        {

            Stopwatch sw = Stopwatch.StartNew();
            Task task = Task.Run(() => CollisionDetector.PerformActions(value));
            //Thread td = new Thread(() => CollisionDetector.PerformCalculations(value));
            //td.Start();
            //Console.WriteLine("Latitude: " + value.RecievedData[0].Lat + " ,Longitude:" + value.RecievedData[0].Lon + " ,Velocity:" + value.RecievedData[0].Vel + " ,Orientation:" + value.RecievedData[0].Orientation);
            //CollisionDetector.PerformCalculations(value);
            //Console.WriteLine("Time elapsed for thread creation " + sw.ElapsedMilliseconds);
        }
        /*public virtual void Subscribe(AbstractSocket provider)
        {
            Unsubscriber = provider.Subscribe(this);
        }*/

        /*public virtual void Unsubscribe()
        {
            Unsubscriber.Dispose();
        }
        */
    }
}
