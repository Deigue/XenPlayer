using System;
using System.Windows.Input;

namespace XenPlayer.Components
{
    public interface IReverseCommand : ICommand
    {
        event EventHandler<CommandEventArgs> CommandExecuted;
    }
}
