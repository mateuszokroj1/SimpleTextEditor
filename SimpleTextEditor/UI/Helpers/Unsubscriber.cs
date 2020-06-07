using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTextEditor.UI.Helpers
{
    internal class Unsubscriber : IDisposable
    {
        private List<IObserver<string>> _observers;
        private IObserver<string> _observer;

        public Unsubscriber(List<IObserver<string>> observers, IObserver<string> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
