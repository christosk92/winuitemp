using Microsoft.UI.Xaml;
using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Wavee.UI.ViewModels;
using Wavee.UI.WinUI.Views.Discover;
using Wavee.UI.WinUI.Views.Discover.Feed;

namespace Wavee.UI.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            var services = ConfigureServices();
            Ioc.Default.ConfigureServices(services);
        }

        private IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<HomeView>();
            serviceCollection.AddTransient<HomeViewModel>();

            serviceCollection.AddTransient<FeedView>();
            serviceCollection.AddTransient<FeedViewModel>();

            serviceCollection.AddTransient<IApplicationDataFolderProvider, WinUIAppDataFolderProvider>();
            serviceCollection.AddTransient<ImageCache>();

            return serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;
    }

    internal class WinUIAppDataFolderProvider : IApplicationDataFolderProvider
    {
        public string Path => Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;
    }
}
