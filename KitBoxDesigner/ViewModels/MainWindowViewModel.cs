using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using KitBoxDesigner.Models;
using KitBoxDesigner.Services;

namespace KitBoxDesigner.ViewModels;

/// <summary>
/// Main window view model managing navigation and global application state
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    private readonly IPartService _partService;
    private readonly IStockService _stockService;
    private readonly IPriceCalculatorService _priceCalculatorService;
    private readonly ConfigurationStorageService _configurationStorageService;
    private readonly IAuthenticationService _authenticationService;
    private readonly ICustomerOrderService _customerOrderService;

    private ViewModelBase _currentViewModel;
    private string _statusMessage = "Ready";

    // Navigation state
    private bool _isConfiguratorActive = true;
    private bool _isStockManagementActive;
    private bool _isOrderManagementActive;

    public MainWindowViewModel(
        IPartService partService, 
        IStockService stockService, 
        IPriceCalculatorService priceCalculatorService,
        ConfigurationStorageService configurationStorageService,
        IAuthenticationService authenticationService,
        ICustomerOrderService customerOrderService)
    {
        _partService = partService;
        _stockService = stockService;
        _priceCalculatorService = priceCalculatorService;
        _configurationStorageService = configurationStorageService;
        _authenticationService = authenticationService;
        _customerOrderService = customerOrderService;

        // Initialize with configurator view
        _currentViewModel = new ConfiguratorViewModel(_partService, _stockService, _priceCalculatorService, _configurationStorageService, ShowOrderCompletion);

        // Initialize commands
        ShowConfiguratorCommand = new SimpleCommand(ShowConfigurator);
        ShowStockManagementCommand = new SimpleCommand(ShowStockManagement, () => _authenticationService.IsAdmin);
        ShowOrderManagementCommand = new SimpleCommand(ShowOrderManagement, () => _authenticationService.IsAdmin);
        LoginCommand = new SimpleCommand(ShowLogin);
        LogoutCommand = new SimpleCommand(Logout);

        // Listen to authentication changes
        _authenticationService.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    /// <summary>
    /// Current active view model
    /// </summary>
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.SafeRaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    /// <summary>
    /// Status message displayed in the UI
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => this.SafeRaiseAndSetIfChanged(ref _statusMessage, value);
    }

    // Navigation state properties
    public bool IsConfiguratorActive
    {
        get => _isConfiguratorActive;
        set => this.SafeRaiseAndSetIfChanged(ref _isConfiguratorActive, value);
    }

    public bool IsStockManagementActive
    {
        get => _isStockManagementActive;
        set => this.SafeRaiseAndSetIfChanged(ref _isStockManagementActive, value);
    }

    public bool IsOrderManagementActive
    {
        get => _isOrderManagementActive;
        set => this.SafeRaiseAndSetIfChanged(ref _isOrderManagementActive, value);
    }

    // Authentication properties
    public bool IsAdmin => _authenticationService.IsAdmin;
    public string CurrentUser => _authenticationService.CurrentUser;

    // Navigation Commands
    public ICommand ShowConfiguratorCommand { get; }
    public ICommand ShowStockManagementCommand { get; }
    public ICommand ShowOrderManagementCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand LogoutCommand { get; }

    /// <summary>
    /// Navigate to cabinet configurator
    /// </summary>
    private void ShowConfigurator()
    {
        try
        {
            // Make sure we're on the UI thread for all UI operations
            EnsureUIThread(() => {
                CurrentViewModel = new ConfiguratorViewModel(_partService, _stockService, _priceCalculatorService, _configurationStorageService, ShowOrderCompletion);
                UpdateNavigationState(configurator: true);
                StatusMessage = "Cabinet Configurator - Design your custom cabinet";
            });
        }
        catch (Exception ex)
        {
            HandleNavigationError("cabinet configurator", ex);
        }
    }

    /// <summary>
    /// Navigate to stock management (inventory view)
    /// </summary>
    private void ShowStockManagement()
    {
        try
        {
            EnsureUIThread(() => {
                CurrentViewModel = new InventoryViewModel(_partService, _stockService);
                UpdateNavigationState(stockManagement: true);
                StatusMessage = "Stock Management - Inventory and stock control";
            });
        }
        catch (Exception ex)
        {
            HandleNavigationError("stock management", ex);
        }
    }

    /// <summary>
    /// Navigate to order management (admin only)
    /// </summary>
    private void ShowOrderManagement()
    {
        try
        {
            EnsureUIThread(() => {
                CurrentViewModel = new OrderManagementViewModel(_customerOrderService, _priceCalculatorService);
                UpdateNavigationState(orderManagement: true);
                StatusMessage = "Order Management - View and manage all customer orders";
            });
        }
        catch (Exception ex)
        {
            HandleNavigationError("order management", ex);
        }
    }

    /// <summary>
    /// Navigate to order completion with configuration
    /// </summary>
    public void ShowOrderCompletion(CabinetConfiguration configuration)
    {
        try
        {
            EnsureUIThread(() => {
                var orderCompletionViewModel = new OrderCompletionViewModel(
                    _priceCalculatorService, 
                    _customerOrderService, 
                    configuration);
                
                // Subscribe to events for navigation
                orderCompletionViewModel.OrderCompleted += OnOrderCompleted;
                orderCompletionViewModel.OrderCancelled += OnOrderCancelled;
                
                CurrentViewModel = orderCompletionViewModel;
                UpdateNavigationState(); // No specific navigation state for order completion
                StatusMessage = "Order Completion - Finalize your cabinet order";
            });
        }
        catch (Exception ex)
        {
            HandleNavigationError("order completion", ex);
        }
    }

    /// <summary>
    /// Handle order completion event
    /// </summary>
    private void OnOrderCompleted(object? sender, OrderCompletedEventArgs e)
    {
        EnsureUIThread(() =>
        {
            // Unsubscribe from events
            if (sender is OrderCompletionViewModel orderViewModel)
            {
                orderViewModel.OrderCompleted -= OnOrderCompleted;
                orderViewModel.OrderCancelled -= OnOrderCancelled;
            }
            
            // Navigate back to configurator after successful order
            ShowConfigurator();
            StatusMessage = $"Order #{e.OrderId} completed successfully! You can start a new configuration.";
        });
    }

    /// <summary>
    /// Handle order cancellation event
    /// </summary>
    private void OnOrderCancelled(object? sender, EventArgs e)
    {
        EnsureUIThread(() =>
        {
            // Unsubscribe from events
            if (sender is OrderCompletionViewModel orderViewModel)
            {
                orderViewModel.OrderCompleted -= OnOrderCompleted;
                orderViewModel.OrderCancelled -= OnOrderCancelled;
            }
            
            // Navigate back to configurator
            ShowConfigurator();
            StatusMessage = "Order cancelled. You're back to the cabinet configurator.";
        });
    }    /// <summary>
    /// Update navigation state to highlight active section
    /// </summary>
    private void UpdateNavigationState(
        bool configurator = false, 
        bool stockManagement = false,
        bool orderManagement = false)
    {
        IsConfiguratorActive = configurator;
        IsStockManagementActive = stockManagement;
        IsOrderManagementActive = orderManagement;
    }
      /// <summary>
    /// Ensures that the provided action is executed on the UI thread
    /// </summary>
    private void EnsureUIThread(Action action)
    {
        if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
        {
            // We're already on the UI thread
            action();
        }
        else
        {
            // We're not on the UI thread, dispatch to it
            Avalonia.Threading.Dispatcher.UIThread.Post(action);
        }
    }
    
    /// <summary>
    /// Handle navigation errors
    /// </summary>
    private void HandleNavigationError(string destination, Exception ex)
    {
        EnsureUIThread(() => 
        {
            // Log the error
            Console.WriteLine($"Error navigating to {destination}: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            // Update UI to show error
            StatusMessage = $"Error loading {destination}. Please try again.";
            
            // Keep the current view model instead of crashing
            // If there's no current view model, create a simple one
            if (CurrentViewModel == null)
            {
                ShowConfigurator(); // Default to configurator
            }
        });
    }

    /// <summary>
    /// Show simple login dialog
    /// </summary>
    private void ShowLogin()
    {
        EnsureUIThread(async () =>
        {
            try
            {
                var (username, password) = await ShowLoginDialog("Admin Login", "Enter admin credentials:");
                if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                {
                    bool loginSuccess = _authenticationService.LoginAsAdmin(username, password);
                    if (loginSuccess)
                    {
                        StatusMessage = $"Logged in as admin: {username}";
                    }
                    else
                    {
                        StatusMessage = "Invalid password. Please try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                StatusMessage = "Login failed. Please try again.";
            }
        });
    }

    /// <summary>
    /// Logout current user
    /// </summary>
    private void Logout()
    {
        _authenticationService.Logout();
        StatusMessage = "Logged out. Admin features are now hidden.";
        
        // If currently viewing an admin-only page, switch to configurator
        if (!_isConfiguratorActive)
        {
            ShowConfigurator();
        }
    }

    /// <summary>
    /// Handle authentication state changes
    /// </summary>
    private void OnAuthenticationStateChanged()
    {
        EnsureUIThread(() =>
        {
            // Notify UI of property changes
            this.RaisePropertyChanged(nameof(IsAdmin));
            this.RaisePropertyChanged(nameof(CurrentUser));
            
            // Update command states
            (ShowStockManagementCommand as SimpleCommand)?.RaiseCanExecuteChanged();
            (ShowOrderManagementCommand as SimpleCommand)?.RaiseCanExecuteChanged();
        });
    }

    /// <summary>
    /// Simple login dialog helper
    /// </summary>
    private async Task<(string username, string password)> ShowLoginDialog(string title, string prompt)
    {
        // Create login dialog window
        var window = new Avalonia.Controls.Window
        {
            Title = title,
            Width = 450,
            Height = 300,
            WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var usernameTextBox = new Avalonia.Controls.TextBox
        {
            Watermark = "Enter username",
            Margin = new Avalonia.Thickness(10, 5)
        };

        var passwordBox = new Avalonia.Controls.TextBox
        {
            Watermark = "Enter password",
            PasswordChar = '*',
            Margin = new Avalonia.Thickness(10, 5)
        };

        var okButton = new Avalonia.Controls.Button
        {
            Content = "Login",
            Margin = new Avalonia.Thickness(10),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            Background = Avalonia.Media.Brushes.DodgerBlue,
            Foreground = Avalonia.Media.Brushes.White,
            Padding = new Avalonia.Thickness(20, 8)
        };

        var cancelButton = new Avalonia.Controls.Button
        {
            Content = "Cancel",
            Margin = new Avalonia.Thickness(10),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            Padding = new Avalonia.Thickness(20, 8)
        };

        string username = "";
        string password = "";
        bool dialogResult = false;

        okButton.Click += (s, e) =>
        {
            username = usernameTextBox.Text ?? "";
            password = passwordBox.Text ?? "";
            dialogResult = true;
            window.Close();
        };

        cancelButton.Click += (s, e) => window.Close();

        // Handle Enter key in password field
        passwordBox.KeyDown += (s, e) =>
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                okButton.Command?.Execute(null);
            }
        };

        var panel = new Avalonia.Controls.StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Children =
            {
                new Avalonia.Controls.TextBlock 
                { 
                    Text = prompt, 
                    FontSize = 16,
                    FontWeight = Avalonia.Media.FontWeight.SemiBold,
                    Margin = new Avalonia.Thickness(0, 0, 0, 20),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                },
                new Avalonia.Controls.TextBlock 
                { 
                    Text = "Username:", 
                    Margin = new Avalonia.Thickness(10, 10, 10, 5) 
                },
                usernameTextBox,
                new Avalonia.Controls.TextBlock 
                { 
                    Text = "Password:", 
                    Margin = new Avalonia.Thickness(10, 15, 10, 5) 
                },
                passwordBox,
                new Avalonia.Controls.StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    Margin = new Avalonia.Thickness(0, 20, 0, 0),
                    Children = { cancelButton, okButton }
                },
                new Avalonia.Controls.TextBlock
                {
                    Text = "💡 Default password: admin",
                    FontSize = 12,
                    Foreground = Avalonia.Media.Brushes.Gray,
                    Margin = new Avalonia.Thickness(10, 15, 10, 0),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                }
            }
        };

        window.Content = panel;
        
        await window.ShowDialog(Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
        
        return dialogResult ? (username, password) : ("", "");
    }
}
