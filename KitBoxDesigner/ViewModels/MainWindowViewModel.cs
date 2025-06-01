using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KitBoxDesigner.Services;
using KitBoxDesigner.Models;
using Microsoft.Extensions.DependencyInjection;

namespace KitBoxDesigner.ViewModels;

/// <summary>
/// Main window view model managing navigation and global application state
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAuthenticationService _authenticationService;
    
    [ObservableProperty]
    private ViewModelBase? _currentViewModel;
    
    [ObservableProperty]
    private string _statusMessage = "Ready";
      [ObservableProperty]
    private User? _currentUser;

    public bool IsAdminLoggedIn => _authenticationService.IsAdmin;    public bool IsConfiguratorActive => CurrentViewModel is ConfiguratorViewModel;
    public bool IsPriceCalculatorActive => CurrentViewModel is PriceCalculatorViewModel;
    public bool IsInventoryActive => CurrentViewModel is InventoryViewModel;
    public bool IsStockCheckerActive => CurrentViewModel is StockCheckerViewModel;    public MainWindowViewModel(IServiceProvider serviceProvider, IAuthenticationService authenticationService)
    {
        _serviceProvider = serviceProvider;
        _authenticationService = authenticationService;
        
        _authenticationService.AuthenticationStateChanged += OnAuthenticationStateChanged;

        // Set Cabinet Designer as the default page for all users
        ShowConfigurator();
    }

    private void OnAuthenticationStateChanged()
    {
        // Ensure UI updates are on the main thread
        RunOnUIThread(() => {
            OnPropertyChanged(nameof(IsAdminLoggedIn));
            UpdateViewBasedOnAuthState();
            UpdateNavigationButtonStates();
            
            // Update CanExecute for admin commands
            ShowInventoryCommand.NotifyCanExecuteChanged();
            ShowStockCheckerCommand.NotifyCanExecuteChanged();
            ShowOrderHistoryCommand.NotifyCanExecuteChanged();
        });
    }    private void UpdateViewBasedOnAuthState()
    {
        if (!_authenticationService.IsAdmin)
        {
            // If not admin and current view is admin-only, redirect to configurator
            if (CurrentViewModel is InventoryViewModel || 
                CurrentViewModel is StockCheckerViewModel)
            {
                ShowConfigurator();
                StatusMessage = "Admin access required for this feature. Switched to Cabinet Designer.";
            }
        }
        // If admin is logged in, allow them to stay on their current view
        // No need to force navigation - they can use the navigation buttons
    }

    [RelayCommand]
    private void ShowConfigurator()
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<ConfiguratorViewModel>();
        StatusMessage = "Cabinet Configurator";
        UpdateNavigationButtonStates();
    }

    [RelayCommand]
    private void ShowPriceCalculator()
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<PriceCalculatorViewModel>();
        StatusMessage = "Price Calculator";
        UpdateNavigationButtonStates();
    }    [RelayCommand(CanExecute = nameof(CanExecuteAdminCommands))]
    private void ShowInventory()
    {
        if (_authenticationService.IsAdmin)
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<InventoryViewModel>();
            StatusMessage = "Inventory Management";
        }
        else
        {
            StatusMessage = "Please login as admin to access inventory.";
        }
        UpdateNavigationButtonStates();
    }

    [RelayCommand(CanExecute = nameof(CanExecuteAdminCommands))]
    private void ShowStockChecker()
    {
        if (_authenticationService.IsAdmin)
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<StockCheckerViewModel>();
            StatusMessage = "Stock Checker";
        }
        else
        {
            StatusMessage = "Please login as admin to access stock checker.";
        }
        UpdateNavigationButtonStates();
    }

    [RelayCommand(CanExecute = nameof(CanExecuteAdminCommands))]
    private void ShowOrderHistory()
    {
        if (_authenticationService.IsAdmin) 
        {
            // For now, use a placeholder until OrderViewModel is properly implemented
            StatusMessage = "Order History - Coming Soon";
        }
        else
        {
            StatusMessage = "Please login as admin to access order history.";
        }
        UpdateNavigationButtonStates();
    }    [RelayCommand]
    private void ToggleLogin()
    {
        if (_authenticationService.IsAdmin)
        {
            // Logout
            _authenticationService.Logout();
            CurrentUser = null; 
            StatusMessage = "Logged out. Admin features are now restricted.";
            // Stay on current view but update permissions
        }
        else
        {
            // Login as admin
            _authenticationService.LoginAsAdmin();
            StatusMessage = "Welcome, Admin! You now have access to all features.";
        }
    }

    private bool CanExecuteAdminCommands() => _authenticationService.IsAdmin;    private void UpdateNavigationButtonStates()
    {
        OnPropertyChanged(nameof(IsConfiguratorActive));
        OnPropertyChanged(nameof(IsPriceCalculatorActive));
        OnPropertyChanged(nameof(IsInventoryActive));
        OnPropertyChanged(nameof(IsStockCheckerActive));
    }

    public void Dispose()
    {
        _authenticationService.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }
}
