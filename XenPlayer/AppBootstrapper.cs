using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using Microsoft.Mef.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using XenPlayer.Components;
using XenPlayer.ViewModels;

namespace XenPlayer
{
    public class AppBootstrapper : BootstrapperBase
    {
        private CompositionContainer _container;
        private AggregateCatalog _catalog;

        public AppBootstrapper()
        {
            Initialize();
        }

        /// <summary>
        /// MEF (Microsoft Extensibility Framework) Setup
        /// </summary>
        protected override void Configure()
        {
            _catalog = new AggregateCatalog();
            _container = new CompositionContainer(_catalog);

            _catalog.Catalogs.Add(new AssemblyCatalog(typeof(AppBootstrapper).Assembly));

            var serviceLocator = new MefServiceLocator(_container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            _container.ComposeExportedValue<IWindowManager>(new DefaultWindowManager());
            base.Configure();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            DisplayRootViewFor<ShellViewModel>();
            var rootWindow = Application.Current.Windows.OfType<MetroWindow>().First(w => w.IsActive);
            rootWindow.Title = "XenPlayer";
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var values = _container.GetExportedValues<object>(contract).ToArray();
            if (values.Any())
            {
                return values.First();
            }

            object instance = base.GetInstance(serviceType, key);
            BuildUp(instance);

            return instance;
        }

        protected override void BuildUp(object instance)
        {
            base.BuildUp(instance);
            _container.SatisfyImportsOnce(instance);
        }
    }
}
