using System;

namespace XenPlayer.Components
{
    public class Event<T> where T : EventArgs
    {
        private readonly object _sender;
        private readonly T _eventArgs;

        public Event(object sender, T eventArgs)
        {
            _sender = sender;
            _eventArgs = eventArgs;
        }

        public object Sender
        {
            get { return _sender; }
        }

        public T EventArgs
        {
            get { return _eventArgs; }
        }
    }
}
