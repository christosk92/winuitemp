using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Behaviors;
using Wavee.Models;
using CommunityToolkit.WinUI.UI.Animations;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Wavee.UI.WinUI.Controls
{
    public sealed partial class AudioItemControl : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(AudioItemControl), new PropertyMetadata(default(string?)));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(string),
            typeof(AudioItemControl), new PropertyMetadata(default(string)));

        // private static async void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        // {
        //     var control = (AudioItemControl)d;
        //     if (e.NewValue is byte[] vector)
        //     {
        //         await control.UpdateImage(vector);
        //     }
        // }


        public static readonly DependencyProperty IsArtistProperty = DependencyProperty.Register(nameof(IsArtist), typeof(bool), typeof(AudioItemControl), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(nameof(Subtitle), typeof(DescriptionItem[]), typeof(AudioItemControl), new PropertyMetadata(default(DescriptionItem[])));
        public static readonly DependencyProperty PointerEnteredValProperty =
            DependencyProperty.Register(nameof(PointerEnteredVal), typeof(bool), typeof(AudioItemControl), new PropertyMetadata(default(bool), PropertyChangedCallback));

        public AudioItemControl()
        {
            this.InitializeComponent();
        }

        public string? Title
        {
            get => (string?)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string? Image
        {
            get => (string?)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public bool IsArtist
        {
            get => (bool)GetValue(IsArtistProperty);
            set => SetValue(IsArtistProperty, value);
        }

        public DescriptionItem[] Subtitle
        {
            get => (DescriptionItem[])GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public bool PointerEnteredVal
        {
            get => (bool)GetValue(PointerEnteredValProperty);
            set => SetValue(PointerEnteredValProperty, value);
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (AudioItemControl)d;
            if (e.NewValue is bool b)
            {
                control.HandleAnimations(b);
            }

        }

        private AnimationBuilder _blur;
        private AnimationBuilder _unblur;

        private void HandleAnimations(bool pointerEntered)
        {
            //blur image if true
            if (pointerEntered)
            {
                _blur ??= BuildBlur();
                _blur.Start(NormalAlbumImage);
            }
            else
            {
                _unblur ??= BuildUNBlur();
                _unblur.Start(NormalAlbumImage);
            }
        }

        public bool Invert(bool b)
        {
            return !b;
        }


        AnimationBuilder BuildBlur()
        {
            var anim = new BlurEffectAnimation
            {
                Duration = TimeSpan.FromMilliseconds(200),
                Target = ImageBlurEffect,
                From = 0,
                To = 32,
                EasingType = EasingType.Linear,
                EasingMode = EasingMode.EaseInOut
            };
            var builder = AnimationBuilder.Create();
            anim.AppendToBuilder(builder, null, null, null, null);
            return builder;
        }

        AnimationBuilder BuildUNBlur()
        {
            var anim = new BlurEffectAnimation
            {
                Duration = TimeSpan.FromMilliseconds(100),
                Target = ImageBlurEffect,
                From = 32,
                To = 0,
                EasingType = EasingType.Linear,
                EasingMode = EasingMode.EaseInOut
            };
            var builder = AnimationBuilder.Create();
            anim.AppendToBuilder(builder, null, null, null, null);
            return builder;
        }
    }
}
