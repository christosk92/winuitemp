using Microsoft.UI.Xaml.Controls;
using Wavee.UI.ViewModels;

namespace Wavee.UI.WinUI.Views
{
    public abstract partial class EasyUserControl<T> : UserControl, INavigablePage where T : INavigableViewModel
    {
        protected EasyUserControl(T viewModel)
        {
            ViewModel = viewModel;
        }

        public virtual bool ShouldKeepInCache(int depth)
        {
            return false;
        }

        public T ViewModel { get; }
        INavigableViewModel INavigablePage.ViewModel => ViewModel;
    }
}
