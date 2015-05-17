using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Threading;
using XenPlayer.Components;
using XenPlayer.Utils;

namespace XenPlayer.Extensions
{
    [Obsolete]
    internal static class HandlerExtensions
    {
        private const string HANDLECOUNT_OUTOFRANGE = "Handle Exactly count cannot be less than 1";
        private const string HANDLER_PERIODLESSTHANZERO = "Handler's Period cannot be less than or equal to zero timespan";

        // When applies if the predicate is true

        public static HandlerBase<TEventArgs, THandler> When<TEventArgs, THandler>(this HandlerBase<TEventArgs, THandler> handler, Func<Event<TEventArgs>, bool> predicate)
            where
                TEventArgs : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentNotNull(predicate, "predicate");

            if (handler.Predicate != null)
            {
                var existingPredicate = handler.Predicate;
                handler.Predicate = e => existingPredicate(e) && predicate(e);
            }
            else
            {
                handler.Predicate = predicate;
            }

            return handler;
        }

        public static T When<T, TEventArgs, THandler>(this T handler, Func<Event<TEventArgs>, bool> predicate) where T : HandlerBase<TEventArgs, THandler>
                                                                                                               where TEventArgs : EventArgs
        {
            When(handler, predicate);
            return handler;
        }

        // Skip is the oppose of when

        public static HandlerBase<TEventArgs, THandler> SkipWhen<TEventArgs, THandler>(this HandlerBase<TEventArgs, THandler> handler, Func<Event<TEventArgs>, bool> predicate)
            where TEventArgs : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentNotNull(predicate, "predicate");

            if (handler.Predicate != null)
            {
                var existingPredicate = handler.Predicate;
                handler.Predicate = e => existingPredicate(e) && !predicate(e);
            }
            else
            {
                handler.Predicate = e => !predicate(e);
            }

            return handler;
        }

        public static T SkipWhen<T, TEventArgs, THandler>(this T handler, Func<Event<TEventArgs>, bool> predicate)
            where T : HandlerBase<TEventArgs, THandler>
            where TEventArgs : EventArgs
        {
            SkipWhen(handler, predicate);
            return handler;
        }

        // Handle Once, allows the handled to be called once and thereafer it will be disposed off

        public static HandlerBase<TEventArgs, THandler> HandleOnce<TEventArgs, THandler>(this HandlerBase<TEventArgs, THandler> handler)
            where TEventArgs : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");

            if (handler.PostHandle != null)
            {
                var existingHandle = handler.PostHandle;
                handler.PostHandle = (h, e) =>
                {
                    existingHandle(h, e);
                    h.Dispose();
                };
            }
            else
            {
                handler.PostHandle = (h, e) => h.Dispose();
            }

            return handler;
        }

        public static T HandleOnce<T, TEventArgs, THandler>(this T handler)
            where T : HandlerBase<TEventArgs, THandler>
            where TEventArgs : EventArgs
        {
            handler.HandleOnce();
            return handler;
        }

        // Handle exactly - allows the handle to be called an exact number of times

        public static HandlerBase<TEventArgs, THandler> HandleExactly<TEventArgs, THandler>(this HandlerBase<TEventArgs, THandler> handler, int count)
            where
                TEventArgs : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentOutOfRange((count < 1), "count", HANDLECOUNT_OUTOFRANGE);

            int counter = count - 1;
            if (handler.PostHandle != null)
            {
                var existingHandle = handler.PostHandle;
                handler.PostHandle = (h, e) =>
                {
                    existingHandle(h, e);
                    if (counter == 0)
                    {
                        h.Dispose();
                    }
                    else
                    {
                        counter -= 1;
                    }
                };
            }
            else
            {
                handler.PostHandle = (h, e) =>
                {
                    if (counter == 0)
                    {
                        h.Dispose();
                    }
                    else
                    {
                        counter -= 1;
                    }
                };
            }
            return handler;
        }

        public static T HandleExactly<T, TEventArgs, THandler>(this T handler, int count)
            where T : HandlerBase<TEventArgs, THandler>
            where TEventArgs : EventArgs
        {
            HandleExactly(handler, count);
            return handler;
        }

        // While - it will handle while the condition is true, once false - note this checked pre-handling

        public static HandlerBase<TEventArgs, THandler> While<TEventArgs, THandler>(this HandlerBase<TEventArgs, THandler> handler, Func<Event<TEventArgs>, bool> condition)
            where TEventArgs : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");

            if (handler.PreHandle != null)
            {
                var existingHandle = handler.PreHandle;
                handler.PreHandle = (h, e) =>
                {
                    existingHandle(h, e);
                    if (!condition(e)) h.Dispose();
                };
            }
            else
            {
                handler.PreHandle = (h, e) =>
                {
                    if (!condition(e))
                    {
                        h.Dispose();
                    }
                };
            }
            return handler;
        }

        public static T While<T, TEventArgs, THandler>(this T handler, Func<Event<TEventArgs>, bool> condition)
            where T : HandlerBase<TEventArgs, THandler>
            where TEventArgs : EventArgs
        {
            While(handler, condition);
            return handler;
        }

        // Until - it will handle till the condition is true, note this 

        public static HandlerBase<TEventArgs, THandler> Until<TEventArgs, THandler>(this HandlerBase<TEventArgs, THandler> handler, Func<Event<TEventArgs>, bool> condition)
            where
                TEventArgs : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");

            if (handler.PostHandle != null)
            {
                var existingHandle = handler.PostHandle;
                handler.PostHandle = (h, e) =>
                {
                    existingHandle(h, e);
                    if (!condition(e)) h.Dispose();
                };
            }
            else
            {
                handler.PostHandle = (h, e) =>
                {
                    if (!condition(e))
                    {
                        h.Dispose();
                    }
                };
            }
            return handler;
        }

        public static T Until<T, TEventArgs, THandler>(this T handler, Func<Event<TEventArgs>, bool> condition)
            where T : HandlerBase<TEventArgs, THandler>
            where TEventArgs : EventArgs
        {
            Until(handler, condition);
            return handler;
        }

        // For - specifically for property changed event args

        public static HandlerBase<PropertyChangedEventArgs, PropertyChangedEventHandler> HandleFor(this HandlerBase<PropertyChangedEventArgs, PropertyChangedEventHandler> handler, Expression<Func<Object>> propertySelector)
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentNotNull(propertySelector, "propertySelector");

            // get the property name
            var propertyName = PropertyChangedExtensions.GetPropertyName(propertySelector);

            if (handler.Predicate != null)
            {
                var existingPredicate = handler.Predicate;
                handler.Predicate = e => existingPredicate(e) && (string.Equals(e.EventArgs.PropertyName, propertyName, StringComparison.InvariantCulture));
            }
            else
            {
                handler.Predicate = e => string.Equals(e.EventArgs.PropertyName, propertyName, StringComparison.InvariantCulture);
            }
            return handler;
        }

        public static T HandleFor<T>(this object handler, Expression<Func<Object>> propertySelector)
        {
            HandleFor((HandlerBase<PropertyChangedEventArgs, PropertyChangedEventHandler>)handler, propertySelector);
            return (T)handler;
        }

        // Throttle     

        public static HandlerBase<TEventArgs, THandler> Throttle<TEventArgs, THandler>(this HandlerBase<TEventArgs, THandler> handler, TimeSpan period)
            where TEventArgs : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentOutOfRange((period <= TimeSpan.Zero), "period", HANDLER_PERIODLESSTHANZERO);

            var lastAllow = DateTime.MinValue;

            if (handler.Predicate != null)
            {
                var existingPredicate = handler.Predicate;
                handler.Predicate = e =>
                {
                    var allow = DateTime.Now.Subtract(lastAllow) > period;
                    if (allow)
                    {
                        lastAllow = DateTime.Now;
                    }
                    return existingPredicate(e) && allow;
                };
            }
            else
            {
                handler.Predicate = e =>
                {
                    var allow = DateTime.Now.Subtract(lastAllow) > period;
                    if (allow)
                    {
                        lastAllow = DateTime.Now;
                    }
                    return allow;
                };
            }
            return handler;
        }

        public static T Throttle<T, TEventArgs, THandler>(this T handler, TimeSpan period)
            where T : HandlerBase<TEventArgs, THandler>
            where TEventArgs : EventArgs
        {
            Throttle(handler, period);
            return handler;
        }

        // Timeout     

        public static HandlerBase<TEventArgs, THandler> Timeout<TEventArgs, THandler>(this HandlerBase<TEventArgs, THandler> handler, TimeSpan period)
            where TEventArgs : EventArgs
        {
            return Timeout(handler, period, null);
        }

        public static HandlerBase<TEventArgs, THandler> Timeout<TEventArgs, THandler>(this HandlerBase<TEventArgs, THandler> handler, TimeSpan period, Action onTimeout)
            where TEventArgs : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentOutOfRange((period <= TimeSpan.Zero), "period", HANDLER_PERIODLESSTHANZERO);

            // if the timer finishes before the first event, it will dispose the handler
            var timer = new DispatcherTimer { Interval = period };
            timer.Tick += (s, e) =>
            {
                if (handler != null)
                {
                    handler.Dispose();
                }
                handler = null;
                if (onTimeout != null)
                {
                    onTimeout();
                }
                onTimeout = null;
            };

            if (handler.PreHandle != null)
            {
                var existingHandle = handler.PreHandle;
                handler.PreHandle = (h, e) =>
                {
                    existingHandle(h, e);
                    if (timer != null)
                    {
                        timer.Stop();
                        timer = null;
                        onTimeout = null;
                    }
                };
            }
            else
            {
                handler.PreHandle = (h, e) =>
                {
                    if (timer != null)
                    {
                        timer.Stop();
                        timer = null;
                    }
                };
            }

            timer.Start();
            return handler;
        }

        public static T Timeout<T, TEventArgs, THandler>(this T handler, TimeSpan period)
            where T : HandlerBase<TEventArgs, THandler>
            where TEventArgs : EventArgs
        {
            Timeout(handler, period);
            return handler;
        }

        public static T Timeout<T, TEventArgs, THandler>(this T handler, TimeSpan period, Action onTimeout)
            where T : HandlerBase<TEventArgs, THandler>
            where TEventArgs : EventArgs
        {
            Timeout(handler, period, onTimeout);
            return handler;
        }

        // Recurring Timeout     

        public static HandlerBase<TEventArgs, THandler> RecurringTimeout<TEventArgs, THandler>(this HandlerBase<TEventArgs, THandler> handler, TimeSpan period)
            where TEventArgs : EventArgs
        {
            return RecurringTimeout(handler, period, null);
        }

        public static HandlerBase<TEventArgs, THandler> RecurringTimeout<TEventArgs, THandler>(this HandlerBase<TEventArgs, THandler> handler, TimeSpan period, Action onTimeout)
            where TEventArgs : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentOutOfRange((period <= TimeSpan.Zero), "period", HANDLER_PERIODLESSTHANZERO);

            // if the timer finishes before the first event, it will dispose the handler
            var timer = new DispatcherTimer
            {
                Interval = period
            };

            timer.Tick += (s, e) =>
            {
                if (handler != null) handler.Dispose();
                handler = null;
                if (onTimeout != null) onTimeout();
                onTimeout = null;
            };

            if (handler.PreHandle != null)
            {
                var existingHandle = handler.PreHandle;
                handler.PreHandle = (h, e) =>
                {
                    existingHandle(h, e);
                    timer.Stop();
                    timer.Start();
                };
            }
            else
            {
                handler.PreHandle = (h, e) =>
                {
                    timer.Stop();
                    timer.Start();
                };
            }

            timer.Start();
            return handler;
        }

        public static T RecurringTimeout<T, TEventArgs, THandler>(this T handler, TimeSpan period)
            where T : HandlerBase<TEventArgs, THandler>
            where TEventArgs : EventArgs
        {
            RecurringTimeout(handler, period);
            return handler;
        }

        public static T RecurringTimeout<T, TEventArgs, THandler>(this T handler, TimeSpan period, Action onTimeout)
            where T : HandlerBase<TEventArgs, THandler>
            where TEventArgs : EventArgs
        {
            RecurringTimeout(handler, period, onTimeout);
            return handler;
        }
    }
}
