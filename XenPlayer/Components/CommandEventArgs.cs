using System;

namespace XenPlayer.Components
{
    public class CommandEventArgs : EventArgs
    {
        public object CommandParameter { get; private set; }

        public CommandEventArgs(object commandParameter)
        {
            CommandParameter = commandParameter;
        }
    }
}
