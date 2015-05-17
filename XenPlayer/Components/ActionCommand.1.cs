using System;
using XenPlayer.Components;
using XenPlayer.Utils;

namespace XenPlayer.Components
{
    public class ActionCommand<T> : CommandBase<T>
    {
        private readonly Func<T, bool> _canExecuteHandler;
        private readonly Action<T> _executeHandler;

        public ActionCommand(Action<T> executeHandler)
            : this(executeHandler, null, true)
        {
        }

        public ActionCommand(Action<T> executeHandler, bool isActive)
            : this(executeHandler, null, isActive)
        {
        }

        public ActionCommand(Action<T> executeHandler, Func<T, bool> canExecuteHandler)
            : this(executeHandler, canExecuteHandler, true)
        {
        }

        public ActionCommand(Action<T> executeHandler, Func<T, bool> canExecuteHandler, bool isActive)
            : base(isActive)
        {
            Guard.ArgumentNotNull(executeHandler, "executeHandler");

            _executeHandler = executeHandler;
            _canExecuteHandler = canExecuteHandler;
        }

        protected override bool OnCanExecute(T parameter)
        {
            return _canExecuteHandler == null || _canExecuteHandler(parameter);
        }

        protected override void OnExecute(T parameter)
        {
            _executeHandler(parameter);
        }
    }
}
