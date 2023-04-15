using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Wavee.UI.ViewModels;

namespace Wavee.UI.WinUI.TemplateSelectors
{
    internal sealed class HomeViewRenderItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GridTemplate { get; set; } = default!;
        public DataTemplate HorizontalStack { get; set; } = default!;

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return item switch
            {
                HomeRenderItem h => h.Render switch
                {
                    HomeRenderItemType.Grid => GridTemplate,
                    HomeRenderItemType.HorizontalStack => HorizontalStack,
                    _ => base.SelectTemplateCore(item, container)
                },
                _ => base.SelectTemplateCore(item, container)
            };
        }
    }
}
