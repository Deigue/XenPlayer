using System;

namespace XenPlayer.Components
{
    public class EventArgs<T> : EventArgs
    {
        public T Arg1 { get; set; }
    }
}
