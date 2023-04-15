using System;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Wavee.UI.WinUI.Views;

namespace Wavee.UI.WinUI.Navigation;

public sealed class NavigationService : ObservableObject
{
    private ContentControl _contentControl;

    private object? _lastParameter;
    private readonly Stack<(Type Type, object? Parameter)> _backStack = new();
    private readonly Dictionary<Type, (INavigablePage Page, object? WithParameter, int InsertedAt)> _cachedPages = new();

    public NavigationService(ContentControl frame)
    {
        _contentControl = frame;
    }

    public void Navigate(Type pageType, object? parameter = null, bool addToStack = true)
    {
        if (!typeof(INavigablePage).IsAssignableFrom(pageType))
        {
            throw new ArgumentException("Page type must implement INavigablePage.", nameof(pageType));
        }

        if (_contentControl.Content is INavigablePage currentPage)
        {
            if (currentPage.GetType() == pageType && _lastParameter == parameter)
            {
                return;
            }

            if (addToStack)
                _backStack.Push((currentPage.GetType(), parameter));

            if (_cachedPages.TryGetValue(currentPage.GetType(), out var cached))
            {
                if (!cached.Page.ShouldKeepInCache(_backStack.Count - cached.InsertedAt))
                {
                    _cachedPages.Remove(cached.Page.GetType());
                }
            }
            else
            {
                if (currentPage.ShouldKeepInCache(0))
                {
                    _cachedPages[currentPage.GetType()] = (currentPage, parameter, _backStack.Count);
                }
            }
        }

        _lastParameter = parameter;
        INavigablePage nextPage;
        if (_cachedPages.TryGetValue(pageType, out var cachedPage))
        {
            nextPage = cachedPage.Page;
        }
        else
        {
            nextPage = (INavigablePage)Ioc.Default.GetService(pageType);
            nextPage.ViewModel.OnNavigatedTo(parameter);
        }

        _contentControl.Content = nextPage;
        OnPropertyChanged(nameof(CanGoBack));
    }


    public bool CanGoBack => _backStack.Count > 0;
    public bool CanGoForward => false;

    public void GoBack()
    {
        if (CanGoBack)
        {
            (Type Type, object? Parameter) previousPageType = _backStack.Pop();
            Navigate(previousPageType.Type, parameter: previousPageType.Parameter, false);
        }
    }

    public void Clear()
    {
        _cachedPages.Clear();
        _backStack.Clear();
        _contentControl = null;
        _lastParameter = null;
    }
}