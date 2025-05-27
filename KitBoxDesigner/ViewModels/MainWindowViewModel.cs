using System;
using System.Windows.Input;
using ReactiveUI;
using KitBoxDesigner.Services;

namespace KitBoxDesigner.ViewModels;

/// <summary>
/// Main window view model managing navigation and global application state
/// </summary>
public class MainWindowViewModel : ViewModelBase
{private readonly IPartService _partService;
    private readonly IStockService _stockService;
    private readonly IPriceCalculatorService _priceCalculatorService;
    private readonly ConfigurationStorageService _configurationStorageService;

    private ViewModelBase _currentViewModel;
    private string _statusMessage = "Ready";

    // Navigation state
    private bool _isConfiguratorActive = true;
    private bool _isPriceCalculatorActive;
    private bool _isInventoryActive;
    private bool _isStockCheckerActive;    public MainWindowViewModel(
        IPartService partService, 
        IStockService stockService, 
        IPriceCalculatorService priceCalculatorService,
        ConfigurationStorageService configurationStorageService)
    {
        _partService = partService;
        _stockService = stockService;
        _priceCalculatorService = priceCalculatorService;
        _configurationStorageService = configurationStorageService;        // Initialize with configurator view
        _currentViewModel = new ConfiguratorViewModel(_partService, _stockService, _priceCalculatorService, _configurationStorageService);

        // Initialize commands
        ShowConfiguratorCommand = new SimpleCommand(ShowConfigurator);
        ShowInventoryCommand = new SimpleCommand(ShowInventory);
        ShowStockCheckerCommand = new SimpleCommand(ShowStockChecker);
        ShowPriceCalculatorCommand = new SimpleCommand(ShowPriceCalculator);
    }/// <summary>
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

    public bool IsPriceCalculatorActive
    {
        get => _isPriceCalculatorActive;
        set => this.SafeRaiseAndSetIfChanged(ref _isPriceCalculatorActive, value);
    }

    public bool IsInventoryActive
    {
        get => _isInventoryActive;
        set => this.SafeRaiseAndSetIfChanged(ref _isInventoryActive, value);
    }

    public bool IsStockCheckerActive
    {
        get => _isStockCheckerActive;
        set => this.SafeRaiseAndSetIfChanged(ref _isStockCheckerActive, value);
    }    // Navigation Commands
    public ICommand ShowConfiguratorCommand { get; }
    public ICommand ShowInventoryCommand { get; }
    public ICommand ShowStockCheckerCommand { get; }
    public ICommand ShowPriceCalculatorCommand { get; }/// <summary>
    /// Navigate to cabinet configurator
    /// </summary>
    private void ShowConfigurator()
    {
        try
        {
            // Make sure we're on the UI thread for all UI operations
            EnsureUIThread(() => {
                CurrentViewModel = new ConfiguratorViewModel(_partService, _stockService, _priceCalculatorService, _configurationStorageService);
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
    /// Navigate to inventory management
    /// </summary>
    private void ShowInventory()
    {
        try
        {
            EnsureUIThread(() => {
                CurrentViewModel = new InventoryViewModel(_partService, _stockService);
                UpdateNavigationState(inventory: true);
                StatusMessage = "Inventory Management - Manage parts and stock levels";
            });
        }
        catch (Exception ex)
        {
            HandleNavigationError("inventory", ex);
        }
    }

    /// <summary>
    /// Navigate to stock checker
    /// </summary>
    private void ShowStockChecker()
    {
        try
        {
            EnsureUIThread(() => {
                CurrentViewModel = new StockCheckerViewModel(_stockService, _partService);
                UpdateNavigationState(stockChecker: true);
                StatusMessage = "Stock Checker - Check availability and stock levels";
            });
        }
        catch (Exception ex)
        {
            HandleNavigationError("stock checker", ex);
        }
    }

    /// <summary>
    /// Navigate to price calculator
    /// </summary>
    private void ShowPriceCalculator()
    {
        try
        {
            EnsureUIThread(() => {
                CurrentViewModel = new PriceCalculatorViewModel(_priceCalculatorService, _partService);
                UpdateNavigationState(priceCalculator: true);
                StatusMessage = "Price Calculator - Calculate cabinet costs and generate quotes";
            });
        }
        catch (Exception ex)
        {
            HandleNavigationError("price calculator", ex);
        }
    }    /// <summary>
    /// Update navigation state to highlight active section
    /// </summary>
    private void UpdateNavigationState(
        bool configurator = false, 
        bool inventory = false, 
        bool stockChecker = false, 
        bool priceCalculator = false)
    {
        IsConfiguratorActive = configurator;
        IsInventoryActive = inventory;
        IsStockCheckerActive = stockChecker;        
        IsPriceCalculatorActive = priceCalculator;
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
}
