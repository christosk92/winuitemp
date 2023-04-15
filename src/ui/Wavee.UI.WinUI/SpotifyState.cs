using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Wavee.UI.WinUI
{
    public partial class SpotifyState : ObservableObject
    {
        private static SpotifyState _instance;

        [ObservableProperty]
        private bool _isConnected;

        public static SpotifyState Instance => _instance ??= new SpotifyState();
    }
}
