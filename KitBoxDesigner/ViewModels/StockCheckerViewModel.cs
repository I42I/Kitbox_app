using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using KitBoxDesigner.Models;
using KitBoxDesigner.Services;

namespace KitBoxDesigner.ViewModels
{
    public class StockCheckerViewModel : ViewModelBase
    {
        private readonly IStockService _stockService;
        private readonly IPartService _partService;
        private readonly IAuthenticationService _authenticationService;
        
        private string _searchText = "";
        private bool _isLoading;
        private string _statusMessage = "";
        private Part? _selectedPart;
        private string _requiredQuantityText = "1";
        private bool _showLowStockOnly;
        private bool _showOutOfStockOnly;

        public bool IsAdmin => _authenticationService.IsAdmin;

        public StockCheckerViewModel(IStockService stockService, IPartService partService, IAuthenticationService authenticationService)
        {
            _stockService = stockService;
            _partService = partService;
            _authenticationService = authenticationService;
            _authenticationService.AuthenticationStateChanged += () => OnPropertyChanged(nameof(IsAdmin));
            
            StockItems = new ObservableCollection<StockItem>();
            LowStockItems = new ObservableCollection<StockItem>();
            OutOfStockItems = new ObservableCollection<StockItem>();
              CheckStockCommand = new SimpleAsyncCommand(CheckStockAsync);
            SearchCommand = new SimpleAsyncCommand(SearchPartsAsync);
            RefreshCommand = new SimpleAsyncCommand(LoadStockDataAsync);
            CheckPartAvailabilityCommand = new SimpleAsyncCommand(CheckPartAvailabilityAsync);

            // Simple auto-search without complex reactive bindings to avoid threading issues
            SetupAutoSearch();
                
            // Load initial data - make sure to handle errors
            RunOnUIThread(() =>
            {
                _ = Task.Run(async () =>
                {
                    try {
                        await LoadStockDataAsync();
                    } catch (Exception ex) {
                        RunOnUIThread(() => {
                            StatusMessage = $"Error loading data: {ex.Message}";
                        });
                    }                });
            });
        }

        public ObservableCollection<StockItem> StockItems { get; }
        public ObservableCollection<StockItem> LowStockItems { get; }
        public ObservableCollection<StockItem> OutOfStockItems { get; }
          public ICommand CheckStockCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CheckPartAvailabilityCommand { get; }public string SearchText
        {
            get => _searchText;
            set 
            { 
                if (this.SafeRaiseAndSetIfChanged(ref _searchText, value))
                {
                    TriggerSearch();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => this.SafeRaiseAndSetIfChanged(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.SafeRaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public Part? SelectedPart
        {
            get => _selectedPart;
            set => this.SafeRaiseAndSetIfChanged(ref _selectedPart, value);
        }

        public string RequiredQuantityText
        {
            get => _requiredQuantityText;
            set => this.SafeRaiseAndSetIfChanged(ref _requiredQuantityText, value);
        }        public bool ShowLowStockOnly
        {
            get => _showLowStockOnly;
            set
            {
                this.SafeRaiseAndSetIfChanged(ref _showLowStockOnly, value);
                RunOnUIThread(() => FilterStockItems());
            }
        }        
        
        public bool ShowOutOfStockOnly
        {
            get => _showOutOfStockOnly;
            set
            {
                this.SafeRaiseAndSetIfChanged(ref _showOutOfStockOnly, value);
                RunOnUIThread(() => FilterStockItems());
            }
        }

        public int RequiredQuantity
        {
            get
            {
                if (int.TryParse(RequiredQuantityText, out int result))
                    return Math.Max(1, result);
                return 1;
            }
        }

        // Missing properties for XAML binding
        public ObservableCollection<Part> AvailableParts { get; } = new ObservableCollection<Part>();
          private StockItem? _selectedStockItem;
        public StockItem? SelectedStockItem
        {
            get => _selectedStockItem;
            set => this.SafeRaiseAndSetIfChanged(ref _selectedStockItem, value);
        }

        private async Task LoadStockDataAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading stock data...";
                  var allStock = await _stockService.GetAllStockItemsAsync();
                
                RunOnUIThread(() => {
                    StockItems.Clear();
                    LowStockItems.Clear();
                    OutOfStockItems.Clear();
                    
                    foreach (var item in allStock)
                    {
                        StockItems.Add(item);
                        
                        if (item.IsOutOfStock)
                            OutOfStockItems.Add(item);
                        else if (item.IsLowStock)
                            LowStockItems.Add(item);
                    }
                    
                    StatusMessage = $"Loaded {StockItems.Count} items. {LowStockItems.Count} low stock, {OutOfStockItems.Count} out of stock.";
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading stock data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SearchPartsAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadStockDataAsync();
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "Searching...";
                
                var searchResults = await _partService.SearchPartsAsync(SearchText);
                var stockItems = new List<StockItem>();
                
                foreach (var part in searchResults)
                {
                    var stockItem = await _stockService.GetStockItemAsync(part.Code);                    if (stockItem != null)
                        stockItems.Add(stockItem);
                }
                
                RunOnUIThread(() => {
                    StockItems.Clear();
                    foreach (var item in stockItems)
                    {
                        StockItems.Add(item);
                    }
                    
                    StatusMessage = $"Found {StockItems.Count} matching items.";
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Search error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterStockItems()
        {
            // This would be called when filter checkboxes change
            // Implementation depends on whether you want to filter the existing collection
            // or reload with filters
        }

        private async Task CheckStockAsync()
        {
            await LoadStockDataAsync();
        }

        private async Task CheckPartAvailabilityAsync()
        {
            if (SelectedPart == null)
            {
                StatusMessage = "Please select a part to check availability.";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "Checking availability...";
                
                var isAvailable = await _stockService.CheckAvailabilityAsync(SelectedPart.Code, RequiredQuantity);
                var stockItem = await _stockService.GetStockItemAsync(SelectedPart.Code);
                
                if (stockItem != null)
                {
                    var availableQty = stockItem.AvailableQuantity;
                    if (isAvailable)
                    {
                        StatusMessage = $"✓ Available: {RequiredQuantity} units of {SelectedPart.Name} (Stock: {availableQty})";
                    }
                    else
                    {                        var shortfall = RequiredQuantity - availableQty;
                        StatusMessage = $"⚠ Insufficient stock: Need {RequiredQuantity}, available {availableQty}, short {shortfall} units";
                        
                        if (stockItem.Part.DeliveryDelay > 0)
                        {
                            StatusMessage += $". Delivery delay: {stockItem.Part.DeliveryDelay} days";
                        }
                    }
                }
                else
                {
                    StatusMessage = $"⚠ Part {SelectedPart.Code} not found in stock";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error checking availability: {ex.Message}";
            }
            finally
            {                IsLoading = false;
            }
        }

        private void SetupAutoSearch()
        {
            // Simple auto-search setup without complex reactive bindings
            // Search will be triggered by property setter
        }

        private void TriggerSearch()
        {
            // Simple debounced search to avoid too many searches
            RunOnUIThread(async () =>
            {
                try
                {
                    await Task.Delay(300); // Simple debounce
                    if (SearchCommand != null)
                    {
                        await SearchPartsAsync();
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error during auto-search: {ex.Message}";
                }
            });
        }
    }
}
