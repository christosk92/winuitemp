using Wavee.UI.ViewModels;

namespace Wavee.UI.WinUI.Views.Discover.Feed;

public sealed partial class FeedView : FeedViewGeneric
{
    public FeedView(FeedViewModel viewModel) : base(viewModel)
    {
        this.InitializeComponent();
    }
}

public abstract partial class FeedViewGeneric : EasyUserControl<FeedViewModel>
{
    protected FeedViewGeneric(FeedViewModel viewModel) : base(viewModel)
    {
    }
    public override bool ShouldKeepInCache(int depth)
    {
        //only keep in cache if depth is less than 2
        return depth < 2;
    }
}