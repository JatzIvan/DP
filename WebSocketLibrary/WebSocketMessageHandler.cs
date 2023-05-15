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
        private IMessageHandler<T> CollisionDetector;

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
            Console.WriteLine("Observer stopped calculating");
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

            Task task = Task.Run(() => CollisionDetector.PerformActions(value));

        }

    }
}
