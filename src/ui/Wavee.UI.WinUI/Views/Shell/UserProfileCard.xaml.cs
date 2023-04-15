using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Wavee.UI.User;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Wavee.UI.WinUI.Views.Shell
{
    public sealed partial class UserProfileCard : UserControl
    {
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register(nameof(User),
            typeof(UserViewModel), typeof(UserProfileCard), new PropertyMetadata(default(UserViewModel), ProfileChanged));

        public UserProfileCard()
        {
            this.InitializeComponent();
        }

        public UserViewModel User
        {
            get => (UserViewModel)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        private static void ProfileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (UserProfileCard)d;
            if (e.NewValue is UserViewModel p)
            {
                c.UpdateProfileData();
            }
        }

        private void UpdateProfileData()
        {
            if (!string.IsNullOrEmpty(User.UserSettings.ProfilePictureUrl))
                Prf.ProfilePicture = new BitmapImage(new Uri(User.UserSettings.ProfilePictureUrl));
            else
            {
                Prf.DisplayName = User.UserSettings.ProfilePictureUrl;
            }

            Product.Text = "OFFLINE";

            // Product.Text = User.ServiceType switch
            // {
            //     ServiceType.Local => "OFFLINE",
            //     ServiceType.Spotify => User.Properties["product"].ToUpper(),
            //     _ => throw new ArgumentOutOfRangeException()
            // };
        }
    }
}