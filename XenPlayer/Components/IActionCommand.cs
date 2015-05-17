using System.ComponentModel;
using System.Windows;

namespace XenPlayer.Components
{
    public interface IActionCommand : IReverseCommand, IWeakEventListener, INotifyPropertyChanged
    {
        bool IsActive { get; set; }

        void RequeryCanExecute();
    }
}
