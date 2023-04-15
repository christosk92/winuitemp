using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT;

namespace Wavee.UI.WinUI.Panels
{
    public sealed class UsefulWrapGrid : VirtualizingLayout
    {
        public double MaximumWidth
        {
            get => (double)GetValue(MaximumWidthProperty);
            set => SetValue(MaximumWidthProperty, value);
        }

        public static readonly DependencyProperty MaximumWidthProperty =
            DependencyProperty.Register(nameof(MaximumWidth), typeof(double), typeof(UsefulWrapGrid),
                new PropertyMetadata(350));


        protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
        {
            //Basically what we want is a grid that does the following:
            // We have a maximum width defined as a property
            // 1) Fit as many items as possible in a row and then wrap to the next row

            //Example:  Available width = 1000, Lets say each item has a width of 200, We can fit 1000/200 = 5 items without resizing.
            //But let's say  Available width = 932, we can fit 932/200 = 4.66 items, rounding gives us 5 items. 
            //So we have 5 - 4.66 = 0.34 items too much. So each item needs to be resized to 932/5 = 186.4
            //So we have 5 items in a row, each item is 186.4 wide, and the total width of the row is 932.
            //All other rows should have the same logic applied to them.


            if (context.ItemCount == 0)
            {
                return new Size(0, 0);
            }

            double childWidth = 200;
            int itemsPerRow = (int)Math.Floor(availableSize.Width / childWidth);
            double newChildWidth = availableSize.Width / itemsPerRow;

            Rect viewport = context.RealizationRect;
            int firstVisibleRowIndex = Math.Max(0, (int)Math.Floor(viewport.Y / newChildWidth));
            int lastVisibleRowIndex = (int)Math.Floor((viewport.Y + viewport.Height) / newChildWidth);

            for (int i = firstVisibleRowIndex; i <= lastVisibleRowIndex; i++)
            {
                for (int j = 0; j < itemsPerRow; j++)
                {
                    int itemIndex = i * itemsPerRow + j;
                    if (itemIndex < context.ItemCount)
                    {
                        UIElement element = context.GetOrCreateElementAt(itemIndex, ElementRealizationOptions.None);
                        element.Measure(new Size(newChildWidth, double.PositiveInfinity));
                    }
                }
            }

            double totalHeight = 0;
            double maxHeightInRow = 0;
            int currentColumn = 0;

            for (int i = firstVisibleRowIndex; i <= lastVisibleRowIndex; i++)
            {
                for (int j = 0; j < itemsPerRow; j++)
                {
                    int itemIndex = i * itemsPerRow + j;
                    if (itemIndex < context.ItemCount)
                    {
                        UIElement element = context.GetOrCreateElementAt(itemIndex, ElementRealizationOptions.None);
                        maxHeightInRow = Math.Max(maxHeightInRow, element.DesiredSize.Height);

                        if (++currentColumn >= itemsPerRow)
                        {
                            totalHeight += maxHeightInRow;
                            currentColumn = 0;
                        }
                    }
                }
            }

            if (currentColumn > 0)
            {
                totalHeight += maxHeightInRow;
            }

            return new Size(availableSize.Width, totalHeight);
        }

        protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
        {
            double childWidth = 200;
            int itemsPerRow = (int)Math.Floor(finalSize.Width / childWidth);
            double newChildWidth = finalSize.Width / itemsPerRow;

            Rect viewport = context.RealizationRect;
            int firstVisibleRowIndex = Math.Max(0, (int)Math.Floor(viewport.Y / newChildWidth));
            int lastVisibleRowIndex = (int)Math.Floor((viewport.Y + viewport.Height) / newChildWidth);

            double totalHeight = 0;
            double previousMaxHeight = 0;
            for (int i = firstVisibleRowIndex; i <= lastVisibleRowIndex; i++)
            {
                double maxHeight = 0;
                for (int j = 0; j < itemsPerRow; j++)
                {
                    int itemIndex = i * itemsPerRow + j;

                    if (itemIndex < context.ItemCount)
                    {
                        UIElement element = context.GetOrCreateElementAt(itemIndex, ElementRealizationOptions.None);
                        double x = j * newChildWidth;
                        double y = i * previousMaxHeight;
                        element.Arrange(new Rect(new Point(x, y), new Size(newChildWidth, element.DesiredSize.Height)));
                        maxHeight = Math.Max(maxHeight, element.DesiredSize.Height);
                    }
                }
                previousMaxHeight = maxHeight;
                totalHeight += maxHeight;
            }

            int rows = (int)Math.Ceiling((double)context.ItemCount / itemsPerRow);

            return new Size(finalSize.Width, totalHeight);
        }
    }
}