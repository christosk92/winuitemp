using Wavee.UI.ViewModels;

namespace Wavee.UI.WinUI.Views;
public interface INavigablePage
{
    bool ShouldKeepInCache(int depth);
    INavigableViewModel ViewModel { get; }
}