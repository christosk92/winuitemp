using System.Diagnostics;
using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;
using Wavee.Models;
using Wavee.UI.Tracks.Models;

namespace Wavee.UI.ViewModels
{
    public partial class AudioItemViewModel : ObservableObject, ISelectable
    {
        [ObservableProperty]
        private bool _isSelected;
        public AudioItemViewModel(LocalTrack track)
        {
            // using var tag = TagLib.File.Create(track.Path);
            // var picture = tag.Tag.Pictures.FirstOrDefault();
            // if (picture != null)
            // {
            //     Image = picture.Data.Data;
            // }
            // else
            // {
            //     Image = null;
            // }

            Title = track.Title;
            Subtitle = track.Performers.Select(x => new DescriptionItem(x, x)).ToArray();
            var imageId = track.ImageId;
            if (imageId is not null)
            {
                var image = ImageCache.Instance.Get(imageId.Value);
                if (image is not null)
                {
                    Image = image;
                }
            }
        }

        public string Title { get; }
        public DescriptionItem[] Subtitle { get; }
        public string Image { get; }
        public bool IsArtist { get; }
    }

    public interface ISelectable
    {
        bool IsSelected { get; set; }
    }
}
