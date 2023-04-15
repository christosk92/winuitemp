using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.VisualBasic;
using Wavee.UI.User;
using Wavee.UI.WinUI.Helpers;

namespace Wavee.UI.WinUI.Views.Shell
{
    public sealed partial class SidebarControl : NavigationView
    {
        /// <summary>
        /// true if the user is currently resizing the sidebar
        /// </summary>
        private bool dragging;

        private double originalSize = 0;
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register(nameof(User), typeof(UserViewModel), typeof(SidebarControl), new PropertyMetadata(default(UserViewModel)));

        public SidebarControl()
        {
            this.InitializeComponent();
        }

        public UserViewModel User
        {
            get => (UserViewModel)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        private void ResizeElementBorder_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var border = (Border)sender;
            border.ChangeCursor(InputSystemCursor.Create(InputSystemCursorShape.Arrow));
            VisualStateManager.GoToState(border.FindAscendant<SplitView>(), "ResizerNormal", true);
            User.UserSettings.SidebarWidth = OpenPaneLength;
            dragging = false;
        }
        private void ResizeElementBorder_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (DisplayMode != NavigationViewDisplayMode.Expanded)
                return;

            originalSize = IsPaneOpen ? User.UserSettings.SidebarWidth : CompactPaneLength;
            var border = (Border)sender;
            border.ChangeCursor(InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast));
            VisualStateManager.GoToState(border.FindAscendant<SplitView>(), "ResizerPressed", true);
            dragging = true;
        }

        private void Border_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (DisplayMode == NavigationViewDisplayMode.Expanded)
                SetSize(e.Cumulative.Translation.X);
        }

        private void Border_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (dragging)
                return; // keep showing pressed event if currently resizing the sidebar

            var border = (Border)sender;
            border.ChangeCursor(InputSystemCursor.Create(InputSystemCursorShape.Arrow));
            VisualStateManager.GoToState(border.FindAscendant<SplitView>(), "ResizerNormal", true);
        }

        private void Border_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (DisplayMode != NavigationViewDisplayMode.Expanded)
                return;

            var border = (Border)sender;
            border.ChangeCursor(InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast));
            VisualStateManager.GoToState(border.FindAscendant<SplitView>(), "ResizerPointerOver", true);
        }

        private void SetSize(double val, bool closeImmediatelyOnOversize = false)
        {
            if (IsPaneOpen)
            {
                var newSize = originalSize + val;
                var isNewSizeGreaterThanMinimum = newSize >= MinimumSidebarWidth;
                if (newSize <= MaximumSidebarWidth && isNewSizeGreaterThanMinimum)
                    OpenPaneLength = newSize; // passing a negative value will cause an exception

                // if the new size is below the minimum, check whether to toggle the pane collapse the sidebar
                IsPaneOpen = !(!isNewSizeGreaterThanMinimum && (MinimumSidebarWidth + val <= CompactPaneLength || closeImmediatelyOnOversize));
            }
            else
            {
                if (val < MinimumSidebarWidth - CompactPaneLength &&
                    !closeImmediatelyOnOversize)
                    return;

                OpenPaneLength = val + CompactPaneLength; // set open sidebar length to minimum value to keep it smooth
                IsPaneOpen = true;
            }
        }

        /// <summary>
        /// The minimum width of the sidebar in expanded state
        /// </summary>
        private const double MinimumSidebarWidth = 180;

        private const double MaximumSidebarWidth = 500;
    }
}
