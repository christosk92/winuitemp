using CommunityToolkit.Mvvm.ComponentModel;
using Wavee.UI.User;

namespace Wavee.UI.ViewModels
{
    public sealed partial class ShellViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserViewModel _user;

        public ShellViewModel()
        {
            _user = UserViewModel.Guest;
        }
    }
}
