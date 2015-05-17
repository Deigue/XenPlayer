using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XenPlayer.StyleProperties
{
    public static class WindowAttachedProperties
    {
        public static readonly DependencyProperty InitialWindowWidthProperty = DependencyProperty.RegisterAttached(
            "InitialWindowWidth", typeof (double), typeof (WindowAttachedProperties), new PropertyMetadata(default(double)));

        public static void SetInitialWindowWidth(DependencyObject element, double value)
        {
            element.SetValue(InitialWindowWidthProperty, value);
        }

        public static double GetInitialWindowWidth(DependencyObject element)
        {
            return (double) element.GetValue(InitialWindowWidthProperty);
        }

        public static readonly DependencyProperty InitialWindowHeightProperty = DependencyProperty.RegisterAttached(
            "InitialWindowHeight", typeof (double), typeof (WindowAttachedProperties), new PropertyMetadata(default(double)));

        public static void SetInitialWindowHeight(DependencyObject element, double value)
        {
            element.SetValue(InitialWindowHeightProperty, value);
        }

        public static double GetInitialWindowHeight(DependencyObject element)
        {
            return (double) element.GetValue(InitialWindowHeightProperty);
        }
    }
}
