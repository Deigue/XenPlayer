using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace XenPlayer.Components
{
    public abstract class CommandBase<T> : IActionCommand
    {
        private bool _isActive = true;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    PropertyChanged.Notify(() => IsActive);
                }
            }
        }

        public event EventHandler CanExecuteChanged;
        public event EventHandler<CommandEventArgs> CommandExecuted;
        public event PropertyChangedEventHandler PropertyChanged;

        protected CommandBase()
        {
        }

        protected CommandBase(bool isActive)
        {
            _isActive = isActive;
        }

        public virtual bool CanExecute(T parameter)
        {
            return IsActive && OnCanExecute(parameter);
        }

        public virtual void Execute(T parameter)
        {
            if (CanExecute(parameter))
            {
                OnExecute(parameter);
                OnCommandExecuted(new CommandEventArgs(parameter));
            }
        }


        public void RequeryCanExecute()
        {
            OnRequeryCanExecute();
        }

        bool ICommand.CanExecute(object parameter)
        {
            CheckParameterType(parameter);
            return CanExecute(ParseParameter(parameter, typeof(T)));
        }

        void ICommand.Execute(object parameter)
        {
            CheckParameterType(parameter);
            Execute(ParseParameter(parameter, typeof(T)));
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            RequeryCanExecute();
            return true;
        }

        protected virtual void OnRequeryCanExecute()
        {
            CanExecuteChanged.SafeRaise(this);
        }

        protected void OnCommandExecuted(CommandEventArgs args)
        {
            CommandExecuted.SafeRaise(this, args);
        }

        protected virtual T ParseParameter(object parameter, Type parseAsType)
        {
            if (parameter == null)
            {
                return default(T);
            }

            if (parseAsType.IsEnum)
            {
                return (T)Enum.Parse(parseAsType, Convert.ToString(parameter), true);
            }

            if (parseAsType.IsValueType)
            {
                return (T)Convert.ChangeType(parameter, parseAsType, null);
            }
            
            return (T)parameter;
        }

        protected void CheckParameterType(object parameter)
        {
            if (parameter == null)
            {
                return;
            }

            if (typeof(T).IsValueType)
            {
                return;
            }

            if (parameter is T == false)
            {
                throw new ArgumentException(string.Format("Not the correct type. Actual [{0}] Expected [{1}]", parameter.GetType().FullName, typeof(T).FullName), "parameter");
            }
        }

        protected abstract bool OnCanExecute(T parameter);
        protected abstract void OnExecute(T parameter);
    }
}
