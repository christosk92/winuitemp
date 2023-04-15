using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Wavee.UI.ViewModels;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Wavee.UI.WinUI.Controls
{
    public sealed partial class SelectableItemsRepeater : ItemsRepeater
    {
        public SelectableItemsRepeater()
        {
            this.InitializeComponent();
        }

        private void ItemsRepeater_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.Focus(FocusState.Programmatic); // Ensure focus for key events.
        }

        private int _lastSelectedIndex;
        private void ItemsRepeater_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var pointerPoint = e.GetCurrentPoint(this);
            var item = ((FrameworkElement)e.OriginalSource)?.FindAscendant<SelectorItem>();
            var itemsSource = ((IEnumerable<object>)(this.ItemsSource)).Cast<ISelectable>().ToList();
            if (item != null)
            {
                int index = this.GetElementIndex(item);
                if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
                {
                    // int anchorIndex = _selectionModel.AnchorIndex.HasValue ? _selectionModel.AnchorIndex.Value.GetAt(1) : index;
                    // _selectionModel.SelectRangeFromAnchor(index); // Select a range of items if Shift is held.
                    //range of items
                    var range = Enumerable.Range(Math.Min(index, _lastSelectedIndex), Math.Abs(index - _lastSelectedIndex) + 1);
                    foreach (var i in range)
                    {
                        var item2 = itemsSource[i];
                        if (item2 != null)
                        {
                            item2.IsSelected = true;
                        }
                    }
                }
                else if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse &&
                         pointerPoint.Properties.PointerUpdateKind is PointerUpdateKind.LeftButtonReleased)
                {
                    var selectableItem = itemsSource[index];
                    selectableItem.IsSelected = !selectableItem.IsSelected;
                    if (selectableItem.IsSelected)
                    {
                        _lastSelectedIndex = index;
                    }
                    else
                    {
                        _lastSelectedIndex = Math.Max(0, _lastSelectedIndex - 1);
                    }
                }
                else if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse &&
                         pointerPoint.Properties.PointerUpdateKind is PointerUpdateKind.RightButtonReleased)
                {
                    //deselect
                    var selectableItem = itemsSource[index];
                    var wasSelected = selectableItem.IsSelected;
                    selectableItem.IsSelected = false;
                    if (wasSelected)
                    {
                        _lastSelectedIndex = Math.Max(0, _lastSelectedIndex - 1);
                    }
                }
            }
        }



        private void ItemsRepeater_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.A && InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down))
            {
                // _selectionModel.SelectAll(); // Select all items if Control+A is pressed.
                // UpdateAllItemsVisualState();
                //select all or deselect all
                var range = Enumerable.Range(0, this.ItemsSourceView.Count);
                bool select = false;
                var itemsSource = ((IEnumerable<object>)(this.ItemsSource)).Cast<ISelectable>().ToList();

                var items = new List<ISelectable>();
                foreach (var i in range)
                {
                    var item2 = itemsSource[i];
                    select = select || !item2.IsSelected;
                    items.Add(item2);
                }

                foreach (var item2 in items)
                {
                    item2.IsSelected = select;
                }
            }
        }

        private void SelectableItemsRepeater_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.Focus(FocusState.Programmatic); // Ensure focus for key events.
        }
    }
}
