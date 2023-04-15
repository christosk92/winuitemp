using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Wavee.UI.ViewModels;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Linq;
using Windows.Storage;
using Wavee.UI.WinUI.Helpers;
using Wavee.UI.WinUI.Panels;
using System.Collections.ObjectModel;

namespace Wavee.UI.WinUI.Views.Discover;

public sealed partial class HomeView : HomeViewGeneric
{
    public SpotifyState SpotifyState { get; } = SpotifyState.Instance;

    public HomeView(HomeViewModel homeViewModel) : base(homeViewModel)
    {
        this.InitializeComponent();
    }


    public Visibility HasItemsAndNotBusy(ObservableCollection<HomeRenderItem> i, bool busy)
    {
        return i.Any(a => a.Any()) && !busy ? Visibility.Visible : Visibility.Collapsed;
    }

    public Visibility NotInSpotifyViewHasNoItemsAndIsNotBusy(object? item, ObservableCollection<HomeRenderItem> i, bool isBusy)
    {
        if (item is PivotItem { Tag: "spotify" })
            return Visibility.Collapsed;
        return i.All(c => !c.Any()) && !isBusy ? Visibility.Visible : Visibility.Collapsed;
    }
    public Visibility InSpotifyViewNotConnectedAndNotBusy(object? item, bool connected, bool isBusy)
    {
        if (item is PivotItem { Tag: "spotify" })
            return !connected && !isBusy ? Visibility.Visible : Visibility.Collapsed;
        return Visibility.Collapsed;
    }
    public Visibility IsBusy(bool b)
    {
        return b ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void NavigationView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // if (args.InvokedItemContainer.Tag is string s)
        // {
        //     await ViewModel.LoadItems(s);
        // }
        if (NavigationView.SelectedItem is PivotItem p)
        {
            await ViewModel.LoadItems(p.Tag.ToString());
        }
    }

    private void MainGrid_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Link;
        e.DragUIOverride.Caption = "Drop here to add music";
        e.DragUIOverride.IsGlyphVisible = true;
        e.DragUIOverride.IsContentVisible = true;
        e.DragUIOverride.IsCaptionVisible = true;
    }

    private async void MainGrid_Drop(object sender, DragEventArgs e)
    {
        await ImportHelpers.Import(e, track =>
        {
            if (NavigationView.SelectedItem is PivotItem { Tag: "local" })
                ViewModel.AddTrack(track);
        });
    }

}

public abstract partial class HomeViewGeneric : EasyUserControl<HomeViewModel>
{
    protected HomeViewGeneric(HomeViewModel viewModel) : base(viewModel)
    {
    }

    public override bool ShouldKeepInCache(int depth)
    {
        return depth < 3;
    }
}