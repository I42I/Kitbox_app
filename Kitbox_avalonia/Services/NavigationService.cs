using System.Collections.Generic;


using Avalonia.Controls;
using Kitbox_avalonia.ViewModels;

namespace Kitbox_avalonia.Services
{
    public interface INavigationService
    {
        void Navigate<TViewModel>(object? parameter = null) where TViewModel : ViewModelBase;
        bool CanGoBack { get; }
        void GoBack();
    }

    public class NavigationService : INavigationService
    {
        private readonly ContentControl _contentControl;
        private readonly Stack<(ViewModelBase ViewModel, object? Parameter)> _navigationStack = new();
        private readonly ViewModelLocator _viewModelLocator;

        public NavigationService(ContentControl contentControl)
        {
            _contentControl = contentControl;
            _viewModelLocator = new ViewModelLocator();
        }

        public bool CanGoBack => _navigationStack.Count > 0;

        public void Navigate<TViewModel>(object? parameter = null) where TViewModel : ViewModelBase
        {
            // Save current view to history if there is one
            if (_contentControl.Content is Control currentView && 
                currentView.DataContext is ViewModelBase currentViewModel)
            {
                _navigationStack.Push((currentViewModel, null));
            }

            // Create and initialize the view model
            var viewModel = _viewModelLocator.GetViewModel<TViewModel>();
            
            if (viewModel is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(parameter);
            }

            // Create the view for this view model
            var view = _viewModelLocator.GetViewForViewModel(viewModel);
            
            // Set the view content
            _contentControl.Content = view;
        }

        public void GoBack()
        {
            if (!CanGoBack) return;

            var (previousViewModel, parameter) = _navigationStack.Pop();
            
            if (previousViewModel is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(parameter);
            }

            var view = _viewModelLocator.GetViewForViewModel(previousViewModel);
            _contentControl.Content = view;
        }
    }
}