using System;
using System.Diagnostics;
using System.Linq.Expressions;
using XenPlayer.Utils;

namespace XenPlayer.Components
{
    public abstract class HandlerBase<TEventArgs, THandler> : IDisposable where TEventArgs : EventArgs
    {
        protected static readonly Func<Action<Object, TEventArgs>, THandler> _createDelegate;

        private Func<Action<Object, TEventArgs>, THandler> _createAction;
        private Action<THandler> _removeAction;
        private THandler _eventHandler;
        private bool _isDisposed;

        internal Func<Event<TEventArgs>, bool> Predicate { get; set; }
        internal Action<HandlerBase<TEventArgs, THandler>, Event<TEventArgs>> PreHandle { get; set; }
        internal Action<HandlerBase<TEventArgs, THandler>, Event<TEventArgs>> PostHandle { get; set; }

        protected Func<Action<Object, TEventArgs>, THandler> CreateAction
        {
            get { return _createAction; }
            set { _createAction = value; }
        }

        protected Action<THandler> RemoveAction
        {
            get { return _removeAction; }
            set { _removeAction = value; }
        }

        protected THandler EventHandler
        {
            get { return _eventHandler; }
            set { _eventHandler = value; }
        }

        static HandlerBase()
        {
            Expression<Func<Action<Object, TEventArgs>, THandler>> createExpr = a => (THandler)(Object)Delegate.CreateDelegate(typeof(THandler), a.Target, a.Method);
            _createDelegate = createExpr.Compile();
        }

        protected HandlerBase(Action<THandler> removeAction) : this(a => _createDelegate(a), removeAction)
        {
        }

        protected HandlerBase(Func<Action<Object, TEventArgs>, THandler> createAction, Action<THandler> removeAction)
        {
            Guard.ArgumentNotNull(createAction, "createAction");
            _createAction = createAction;
            _removeAction = removeAction;
        }

        protected abstract void Handle(Object sender, TEventArgs eventArgs);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_removeAction != null)
                {
                    _removeAction(_eventHandler);
                }

                Predicate = null;
                PreHandle = null;
                PostHandle = null;
                _createAction = null;
                _removeAction = null;
                _eventHandler = default(THandler);
                _isDisposed = true;
            }
        }

        public static implicit operator THandler(HandlerBase<TEventArgs, THandler> handler)
        {
            handler.EventHandler = handler.CreateAction((s, e) =>
            {
                Debug.Assert(handler != null, "Handler should have been detached, as handler is null.");
                if (handler == null)
                {
                    return;
                }

                var handlerBase = handler;
                var @event = new Event<TEventArgs>(s, e);

                //if (handler.Predicate != null && !handler.Predicate(_event)) return;
                var preHandler = handlerBase.PreHandle;
                if (!handlerBase._isDisposed && preHandler != null)
                {
                    preHandler(handlerBase, @event);
                }

                var predicate = handlerBase.Predicate;
                if (!handlerBase._isDisposed && (predicate == null || predicate(@event)))
                {
                    handlerBase.Handle(s, e);
                }

                var postHandler = handlerBase.PostHandle;
                if (!handlerBase._isDisposed && postHandler != null)
                {
                    postHandler(handlerBase, @event);
                }

                // we check if it is disposed, if so then we set the handler to null 
                if (handlerBase._isDisposed) {
                    handler = null;        // this should remove the lifting
                }
            });

            // we return
            return handler.EventHandler;
        }
    }
}
