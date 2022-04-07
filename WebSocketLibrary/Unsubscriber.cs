using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketLibrary
{
    public class Unsubscriber<CarUpdateInfo> : IDisposable
    {
        private List<IObserver<CarUpdateInfo>> _observers;
        private IObserver<CarUpdateInfo> _observer;

        internal Unsubscriber(List<IObserver<CarUpdateInfo>> observers, IObserver<CarUpdateInfo> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
