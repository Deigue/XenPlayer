using System;

public static class EventExtensions
{
    public static void SafeRaise<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
    {
        if (eventHandler != null)
        {
            eventHandler(sender, eventArgs);
        }
    }

    public static void SafeRaise(this EventHandler eventHandler, object sender, EventArgs eventArgs)
    {
        if (eventHandler != null)
        {
            eventHandler(sender, eventArgs);
        }
    }

    public static void SafeRaise(this EventHandler eventHandler, object sender)
    {
        if (eventHandler != null)
        {
            eventHandler(sender, EventArgs.Empty);
        }
    }
}
