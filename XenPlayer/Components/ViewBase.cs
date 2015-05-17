using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace XenPlayer.Components
{
    public class ViewBase : Control
    {
        private static readonly PropertyInfo _childIndexFromChildNamePropertyInfo;
        private static readonly PropertyInfo _templateInternalPropertyInfo;
        private static readonly MethodInfo _styleHelperGetChildMethodInfo;

        public EventHandler TemplateApplied;

        static ViewBase()
        {
            _templateInternalPropertyInfo = typeof(FrameworkElement).GetProperty("TemplateInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            _childIndexFromChildNamePropertyInfo = typeof(FrameworkTemplate).GetProperty("ChildIndexFromChildName", BindingFlags.NonPublic | BindingFlags.Instance);
            _styleHelperGetChildMethodInfo = Type.GetType("System.Windows.StyleHelper, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35").GetMethods(BindingFlags.Static | BindingFlags.NonPublic).FirstOrDefault(el => el.Name == "GetChild" && el.GetParameters()[0].ParameterType == typeof(DependencyObject));
        }

        public ViewBase()
        {
            SetValue(View.IsScopeRootProperty, true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var templateInternal = (FrameworkTemplate)_templateInternalPropertyInfo.GetValue(this, new object[0]);
            var childIndexFromChildName = (HybridDictionary)_childIndexFromChildNamePropertyInfo.GetValue(templateInternal, new object[0]);
            List<FrameworkElement> namedElements = new List<FrameworkElement>();
            foreach (DictionaryEntry de in childIndexFromChildName)
            {
                char firstChar = ((string)de.Key)[0];
                if ((firstChar >= 'A' && firstChar <= 'Z') || (firstChar >= 'a' && firstChar <= 'z'))
                {
                    int childIndex = (int)de.Value;
                    FrameworkElement fe = _styleHelperGetChildMethodInfo.Invoke(null, new object[] { this, childIndex }) as FrameworkElement;
                    if (fe != null)
                    {
                        namedElements.Add(fe);
                    }
                }
            }

            ViewModelBinder.BindProperties(namedElements, DataContext.GetType());
        }
    }
}
