using Avalonia.Controls;
using Kitbox_avalonia.ViewModels;
using Kitbox_avalonia.Views;
using System;
using System.Collections.Generic;

namespace Kitbox_avalonia.Services
{
    public class ViewModelLocator
    {
        private readonly Dictionary<Type, Type> _viewModelToViewMap = new();
        private readonly Dictionary<Type, Func<ViewModelBase>> _viewModelFactory = new();

        public ViewModelLocator()
        {
            // Register ViewModels and their corresponding Views
            RegisterViewModelToView<HomeViewModel, HomeView>();
            RegisterViewModelToView<LoginViewModel, LoginView>();
            RegisterViewModelToView<ConfigViewModel, ConfigView>();
            RegisterViewModelToView<OrdersViewModel, OrdersView>();
            RegisterViewModelToView<ManagerViewModel, ManagerView>();
            RegisterViewModelToView<OrdersManagementViewModel, OrdersManagementView>();
            RegisterViewModelToView<StockManagementViewModel, StockManagementView>();
            RegisterViewModelToView<SupplierManagementViewModel, SupplierManagementView>();
            
            // Register ViewModel factories
            RegisterViewModel(() => new HomeViewModel(App.Current.Services.GetService<INavigationService>()));
            RegisterViewModel(() => new LoginViewModel(App.Current.Services.GetService<INavigationService>()));
            RegisterViewModel(() => new ConfigViewModel(App.Current.Services.GetService<INavigationService>()));
            RegisterViewModel(() => new OrdersViewModel(App.Current.Services.GetService<INavigationService>()));
            RegisterViewModel(() => new ManagerViewModel(App.Current.Services.GetService<INavigationService>()));
            RegisterViewModel(() => new OrdersManagementViewModel(App.Current.Services.GetService<INavigationService>()));
            RegisterViewModel(() => new StockManagementViewModel(App.Current.Services.GetService<INavigationService>()));
            RegisterViewModel(() => new SupplierManagementViewModel(App.Current.Services.GetService<INavigationService>()));
        }

        private void RegisterViewModel<TViewModel>(Func<TViewModel> factory) where TViewModel : ViewModelBase
        {
            _viewModelFactory[typeof(TViewModel)] = () => factory();
        }

        private void RegisterViewModelToView<TViewModel, TView>()
            where TViewModel : ViewModelBase
            where TView : Control
        {
            _viewModelToViewMap[typeof(TViewModel)] = typeof(TView);
        }

        public TViewModel GetViewModel<TViewModel>() where TViewModel : ViewModelBase
        {
            return (TViewModel)_viewModelFactory[typeof(TViewModel)]();
        }

        public Control GetViewForViewModel(ViewModelBase viewModel)
        {
            var viewType = _viewModelToViewMap[viewModel.GetType()];
            var view = (Control)Activator.CreateInstance(viewType)!;
            view.DataContext = viewModel;
            return view;
        }
    }
}