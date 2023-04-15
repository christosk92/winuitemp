using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Wavee.UI.User
{
    public sealed class UserViewModel : ObservableObject
    {
        public UserSettings UserSettings { get; }

        public UserViewModel(Guid userId)
        {
            UserSettings = new UserSettings(userId);
            UserSettings.Initialize();
        }

        public static UserViewModel Guest = new UserViewModel(Guid.Empty);

        public void Deconstruct()
        {
            UserSettings.Deconstruct();
        }
    }

    public partial class UserSettings : ObservableObject
    {
        [ObservableProperty]
        private double _sidebarWidth;

        [ObservableProperty]
        private string? _displayName;

        [ObservableProperty]
        private string? _profilePictureUrl;

        private readonly Guid _userId;
        public UserSettings(Guid userId)
        {
            _userId = userId;
            ReadSettings();
        }


        public void Initialize()
        {
            this.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName
                is nameof(SidebarWidth)
                or nameof(DisplayName)
                or nameof(ProfilePictureUrl))
            {
                WriteSettings();
            }
        }


        public void Deconstruct()
        {
            this.PropertyChanged -= OnPropertyChanged;
        }


        private void ReadSettings()
        {
            if (_userId == Guid.Empty)
            {
                DisplayName = "Guest";
                return;
            }

        }
        private void WriteSettings()
        {
            if (_userId == Guid.Empty)
            {
                return;
            }

        }
    }
}
