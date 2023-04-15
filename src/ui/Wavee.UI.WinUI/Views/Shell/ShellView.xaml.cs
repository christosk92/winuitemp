using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Wavee.UI.ViewModels;
using Wavee.UI.WinUI.Navigation;
using Wavee.UI.WinUI.Views.Discover;
using Wavee.UI.WinUI.Views.Discover.Feed;

namespace Wavee.UI.WinUI.Views.Shell
{
    public sealed partial class ShellView : UserControl
    {
        public static NavigationService NavigationService { get; private set; }
        public ShellView()
        {
            this.InitializeComponent();
            NavigationService = new NavigationService(NavigationFrame);
        }

        public NavigationService NavService => NavigationService;
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        private void NavigationView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is NavigationViewItem item)
            {
                var page = item.Tag switch
                {
                    "home" => typeof(HomeView),
                    "feed" => typeof(FeedView),
                    "songs" => typeof(HomeView),
                    "artists" => typeof(HomeView),
                    "albums" => typeof(HomeView),
                    "podcasts" => typeof(HomeView),
                    _ => default
                };

                NavigationService.Navigate(page);
            }
        }

        private void GoBackButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}
