using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XenPlayer.Utils;

namespace XenPlayer.StyleProperties
{
    public static class EditableViewAttachedProperties
    {
        public static readonly DependencyProperty ViewSavedStateProperty = DependencyProperty.RegisterAttached("ViewSavedState", typeof(ViewSaveState), typeof(EditableViewAttachedProperties), new PropertyMetadata(ViewSaveState.None));

        public static void SetViewSavedState(DependencyObject element, ViewSaveState value)
        {
            element.SetValue(ViewSavedStateProperty, value);
        }

        public static ViewSaveState GetViewSavedState(DependencyObject element)
        {
            return (ViewSaveState)element.GetValue(ViewSavedStateProperty);
        }
    }
}
