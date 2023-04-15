using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nito.AsyncEx;
using Wavee.UI.Tracks;
using Wavee.UI.Tracks.Models;

namespace Wavee.UI.ViewModels;

public partial class HomeViewModel : ObservableObject, INavigableViewModel
{
    private readonly AsyncLock _lock = new();
    private static string? _lastView;

    [ObservableProperty]
    private bool _isBusy;
    private CancellationTokenSource? _cts;

    public ObservableCollection<HomeRenderItem> Items { get; }
        = new ObservableCollection<HomeRenderItem>();

    public async Task LoadItems(string view)
    {
        _lastView = view;
        try
        {
            if (_cts is not null)
                await _cts.CancelAsync();
        }
        catch (ObjectDisposedException)
        {
            // Ignore
        }
        finally
        {
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
        }
        //view: {local, spotify}

        try
        {
            using (await _lock.LockAsync(_cts.Token))
            {
                IsBusy = true;
                Items.Clear();

                var task = view switch
                {
                    "local" => LoadLocalItems(_cts.Token),
                    "spotify" => LoadSpotifyItems(_cts.Token),
                    _ => throw new ArgumentException("Invalid view")
                };

                var items = await task;
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
            Debug.WriteLine("[FetchHome] Operation cancelled");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<IEnumerable<HomeRenderItem>> LoadLocalItems(CancellationToken cancellationToken)
    {
        var tracks = await LocalTracksDb.Instance.GetTracks();

        var group = HomeRenderItem.TracksGrid;
        foreach (var track in tracks)
        {
            group.Add(new AudioItemViewModel(track));
        }
        return new[] { group };
    }
    private static async Task<IEnumerable<HomeRenderItem>> LoadSpotifyItems(CancellationToken cancellationToken)
    {
        return Enumerable.Empty<HomeRenderItem>();
    }

    public void AddTrack(LocalTrack track)
    {
        using (_lock.Lock())
        {
            var group = Items.FirstOrDefault(x => x.Render is HomeRenderItemType.Grid);
            if (group is null)
            {
                group = HomeRenderItem.TracksGrid;
                Items.Add(group);
            }

            group.Insert(0, new AudioItemViewModel(track));
        }
    }

    public async void OnNavigatedTo(object? parameter)
    {
        _lastView ??= "local";
        await LoadItems(_lastView);
    }

}

public class HomeRenderItem : ObservableCollection<AudioItemViewModel>
{
    public required HomeRenderItemType Render { get; init; }
    public required string Title { get; init; }

    public static HomeRenderItem TracksGrid => new()
    {
        Render = HomeRenderItemType.Grid,
        Title = "Tracks"
    };
}

public enum HomeRenderItemType
{
    HorizontalStack,
    Grid
}