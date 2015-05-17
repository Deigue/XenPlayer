using System;
using System.Windows;
using XenPlayer.Utils;

namespace XenPlayer.Components
{
    public class WeakListenerHandler<TEventArgs, THandler>
         : HandlerBase<TEventArgs, THandler>
        where
            TEventArgs : EventArgs
    {
        WeakReference _listener;

        public WeakListenerHandler(IWeakEventListener listener, Action<THandler> removeAction)
            : this(a => _createDelegate(a), listener, removeAction) { }

        public WeakListenerHandler(Func<Action<Object, TEventArgs>, THandler> createAction, IWeakEventListener listener,
            Action<THandler> removeAction)
            : base(createAction, removeAction)
        {
            Guard.ArgumentNotNull(listener, "listener");
            Listener = listener;
        }

        protected IWeakEventListener Listener
        {
            get
            {
                if (_listener == null || !_listener.IsAlive) return null;
                var targetObj = _listener.Target;
                return (IWeakEventListener)targetObj;
            }
            set
            {
                if (value == null)
                {
                    if (_listener != null) _listener.Target = null;
                    _listener = null;
                }
                else
                {
                    _listener = new WeakReference(value);
                }
            }
        }

        #region Overrides

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose(true);
                Listener = null;
            }
        }

        protected override void Handle(object sender, TEventArgs eventArgs)
        {
            // we use a flag coz if the listener returns false we need to remove it
            bool removeHandlerFlag = true;
            IWeakEventListener listenerObj = Listener;

            // if we recieve a false signal then
            if (listenerObj != null)
                removeHandlerFlag = !listenerObj.ReceiveWeakEvent(typeof(THandler), sender, eventArgs);

            // we remove the handler
            if (removeHandlerFlag) Dispose();
        }

        #endregion

#if DEBUG && WRITETOCONSOLE
        ~WeakListenerHandler()
        {
            Debug.WriteLine("Releasing Weak Listener Handler<E,H> " + this.GetHashCode().ToString());
        }
#endif
    }
}
