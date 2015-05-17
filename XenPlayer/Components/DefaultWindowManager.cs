using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Caliburn.Micro;
using XenPlayer.Components;
using XenPlayer.StyleProperties;

namespace XenPlayer.Components
{
    [Export(typeof(IWindowManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    class DefaultWindowManager : WindowManager
    {
        public override bool? ShowDialog(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            return base.ShowDialog(rootModel, context, settings);
        }

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            var ew = base.EnsureWindow(model, view, isDialog);
            //if (isDialog) return ew;

            // ** PGRMR NOTE.....
            // Setup Window Style
            ew.WindowStyle = WindowStyle.ThreeDBorderWindow;
            ew.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ew.SizeToContent = SizeToContent.Manual;
           

            // ** PRGMR NOTE.....    
            // Extracts the the IntialWindowHeight and InitialWindowWidth from the Style.ShellView.xaml to set 
            // the windows height and width properties.   This will help prevent the TreeView or any other controls
            // to expand or resize automatically.  Window Resizing and GridSplitter controls should be done manually.
            // Example of the Setter  used in ShellView.xaml: 
            //          <Setter Property="styleProperties:WindowAttachedProperties.InitialWindowHeight" Value="700"/>
            //          <Setter Property="styleProperties:WindowAttachedProperties.InitialWindowWidth" Value="1000"/>
            var viewControl = view as Control;
            if (viewControl != null)
            {
                ew.Height = WindowAttachedProperties.GetInitialWindowHeight(viewControl);
                ew.Width = WindowAttachedProperties.GetInitialWindowWidth(viewControl);
            }

            // ** PGRMR NOTE.....
            // Bind the "Title" to the Window which is extracted from the ShellViewModel.cs (interface IHasTitle.cs)
            var hasTitle = model as IHasTitle;
            if (hasTitle != null)
            {
                var titleBinding = new Binding("Title");
                BindingOperations.SetBinding(ew, Window.TitleProperty, titleBinding);
            }
            return ew;
        }
    }
}
