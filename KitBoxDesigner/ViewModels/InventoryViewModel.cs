using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using KitBoxDesigner.Models;
using KitBoxDesigner.Services;

namespace KitBoxDesigner.ViewModels
{
    /// <summary>
    /// View model for inventory management
    /// </summary>
    public class InventoryViewModel : ViewModelBase
    {
        private readonly IPartService _partService;
        private readonly IStockService _stockService;

        private ObservableCollection<StockItemViewModel> _stockItems;
        private ObservableCollection<StockItemViewModel> _filteredStockItems;
        private string _searchText = string.Empty;
        private InventoryFilter _currentFilter = InventoryFilter.All;
        private StockItemViewModel? _selectedStockItem;
        private bool _isLoading;
        private string _statusMessage = string.Empty;        private StockSummary? _stockSummary;

        // Commands
        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand UpdateStockCommand { get; }
        public ICommand AddStockCommand { get; }
        public ICommand RemoveStockCommand { get; }
        public ICommand ExportInventoryCommand { get; }
        public ICommand EditStockCommand { get; }
        public ICommand ViewDetailsCommand { get; }        public InventoryViewModel(IPartService partService, IStockService stockService)
        {
            _partService = partService;
            _stockService = stockService;

            _stockItems = new ObservableCollection<StockItemViewModel>();
            _filteredStockItems = new ObservableCollection<StockItemViewModel>();
            
            // Initialize commands
            RefreshCommand = new SimpleAsyncCommand(RefreshData);
            SearchCommand = new SimpleCommand(ApplyFilter);
            UpdateStockCommand = new SimpleAsyncCommand<StockItemViewModel>(UpdateStock);
            AddStockCommand = new SimpleAsyncCommand<StockItemViewModel>(AddStock);
            RemoveStockCommand = new SimpleAsyncCommand<StockItemViewModel>(RemoveStock);
            ExportInventoryCommand = new SimpleCommand(ExportInventory);
            EditStockCommand = new SimpleAsyncCommand<StockItemViewModel>(EditStock);
            ViewDetailsCommand = new SimpleAsyncCommand<StockItemViewModel>(ViewDetails);

            // Load initial data with error handling
            RunOnUIThread(() =>
            {
                _ = Task.Run(async () =>
                {
                    try 
                    {
                        await RefreshData();
                    }
                    catch (Exception ex)
                    {
                        RunOnUIThread(() => {
                            StatusMessage = $"Error loading initial data: {ex.Message}";
                        });
                    }
                });
            });
        }

        #region Properties
        
        public ObservableCollection<StockItemViewModel> StockItems
        {
            get => _stockItems;
            set => this.SafeRaiseAndSetIfChanged(ref _stockItems, value);
        }

        public ObservableCollection<StockItemViewModel> FilteredStockItems
        {
            get => _filteredStockItems;
            set => this.SafeRaiseAndSetIfChanged(ref _filteredStockItems, value);
        }

        public string SearchText
        {
            get => _searchText;
            set 
            { 
                this.SafeRaiseAndSetIfChanged(ref _searchText, value);
                RunOnUIThread(() => ApplyFilter());
            }
        }

        public InventoryFilter CurrentFilter
        {
            get => _currentFilter;
            set 
            { 
                this.SafeRaiseAndSetIfChanged(ref _currentFilter, value);
                RunOnUIThread(() => ApplyFilter());
            }
        }

        public StockItemViewModel? SelectedStockItem
        {
            get => _selectedStockItem;
            set => this.SafeRaiseAndSetIfChanged(ref _selectedStockItem, value);
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
        }        public StockSummary? StockSummary
        {
            get => _stockSummary;
            set => this.SafeRaiseAndSetIfChanged(ref _stockSummary, value);
        }

        // Missing properties for XAML binding
        public ObservableCollection<StockItemViewModel> FilteredParts => FilteredStockItems;
        
        public StockItemViewModel? SelectedPart
        {
            get => SelectedStockItem;
            set => SelectedStockItem = value;
        }

        // Statistics properties
        public int TotalParts => StockItems.Count;
        public int InStockCount => StockItems.Count(s => s.StockStatus == StockStatus.InStock);
        public int LowStockCount => StockItems.Count(s => s.StockStatus == StockStatus.LowStock);
        public int OutOfStockCount => StockItems.Count(s => s.StockStatus == StockStatus.OutOfStock);

        // Filter properties
        public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>
        {
            "All Categories",
            "Panel",
            "Door", 
            "Corner",
            "CornerBar",
            "Accessory",
            "Handle"
        };        private string _selectedCategory = "All Categories";
        public string SelectedCategory
        {
            get => _selectedCategory;
            set 
            { 
                this.SafeRaiseAndSetIfChanged(ref _selectedCategory, value);
                RunOnUIThread(() => ApplyFilter());
            }
        }

        private bool _showLowStockOnly = false;
        public bool ShowLowStockOnly
        {
            get => _showLowStockOnly;
            set 
            { 
                this.SafeRaiseAndSetIfChanged(ref _showLowStockOnly, value);
                RunOnUIThread(() => ApplyFilter());
            }
        }

        private bool _showOutOfStockOnly = false;
        public bool ShowOutOfStockOnly
        {
            get => _showOutOfStockOnly;
            set 
            { 
                this.SafeRaiseAndSetIfChanged(ref _showOutOfStockOnly, value);
                RunOnUIThread(() => ApplyFilter());
            }
        }

        // Export command alias
        public ICommand ExportReportCommand => ExportInventoryCommand;

        #endregion        #endregion

        #region Methods

        private async Task RefreshData()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading inventory data...";                var stockItems = await _stockService.GetAllStockItemsAsync();
                StockSummary = await _stockService.GetStockSummaryAsync();

                RunOnUIThread(() => {
                    StockItems.Clear();
                    foreach (var item in stockItems)
                    {
                        StockItems.Add(new StockItemViewModel(item, _stockService));
                    }

                    ApplyFilter();
                    StatusMessage = $"Loaded {StockItems.Count} inventory items";
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading inventory: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilter()
        {
            var filtered = StockItems.AsEnumerable();

            // Apply text search
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(item => 
                    item.PartCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    item.PartReference.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // Apply filter
            filtered = CurrentFilter switch
            {
                InventoryFilter.LowStock => filtered.Where(item => item.StockStatus == StockStatus.LowStock),
                InventoryFilter.OutOfStock => filtered.Where(item => item.StockStatus == StockStatus.OutOfStock),
                InventoryFilter.InStock => filtered.Where(item => item.StockStatus == StockStatus.InStock),
                InventoryFilter.NeedReorder => filtered.Where(item => item.NeedsReorder),
                _ => filtered
            };

            // Apply category filter
            if (_selectedCategory != "All Categories")
            {
                filtered = filtered.Where(item => item.Category.ToString() == _selectedCategory);
            }            // Apply low stock and out of stock filters
            if (ShowLowStockOnly)
            {
                filtered = filtered.Where(item => item.StockStatus == StockStatus.LowStock);
            }

            if (ShowOutOfStockOnly)
            {
                filtered = filtered.Where(item => item.StockStatus == StockStatus.OutOfStock);
            }

            RunOnUIThread(() => {
                FilteredStockItems.Clear();
                foreach (var item in filtered.OrderBy(i => i.PartCode))
                {
                    FilteredStockItems.Add(item);
                }

                // Update statistics properties
                this.SafeRaisePropertyChanged(nameof(TotalParts));
                this.SafeRaisePropertyChanged(nameof(InStockCount));
                this.SafeRaisePropertyChanged(nameof(LowStockCount));
                this.SafeRaisePropertyChanged(nameof(OutOfStockCount));
            });
        }        private async Task UpdateStock(StockItemViewModel? stockItem)
        {
            if (stockItem == null) return;
            
            // In real app would show dialog for new quantity
            var newQuantity = stockItem.CurrentStock + 10; // Simulate adding 10 units
            
            var success = await _stockService.UpdateStockAsync(stockItem.PartCode, newQuantity, "Manual update");
            if (success)
            {
                stockItem.CurrentStock = newQuantity;
                StatusMessage = $"Updated stock for {stockItem.PartCode}";
                await RefreshData();
            }
        }

        private async Task AddStock(StockItemViewModel? stockItem)
        {
            if (stockItem == null) return;
            
            var quantity = 50; // Simulate delivery of 50 units
            
            var success = await _stockService.AddStockAsync(stockItem.PartCode, quantity, "Delivery");
            if (success)
            {
                StatusMessage = $"Added {quantity} units to {stockItem.PartCode}";
                await RefreshData();
            }
        }

        private async Task RemoveStock(StockItemViewModel? stockItem)
        {
            if (stockItem == null) return;
            
            var quantity = 5; // Simulate sale of 5 units
            
            var success = await _stockService.RemoveStockAsync(stockItem.PartCode, quantity, "Sale");
            if (success)
            {
                StatusMessage = $"Removed {quantity} units from {stockItem.PartCode}";
                await RefreshData();
            }
        }

        private void ExportInventory()
        {
            // In real app would export to CSV or Excel
            StatusMessage = "Inventory exported successfully";
        }        private async Task EditStock(StockItemViewModel? stockItem)
        {
            if (stockItem == null) return;
            
            // In real app, would open edit dialog
            StatusMessage = $"Editing stock for {stockItem.PartCode}";
            await Task.Delay(100); // Simulate async operation
        }

        private async Task ViewDetails(StockItemViewModel? stockItem)
        {
            if (stockItem == null) return;
            
            // In real app, would open details view
            StatusMessage = $"Viewing details for {stockItem.PartCode}";
            await Task.Delay(100); // Simulate async operation
        }

        #endregion
    }

    /// <summary>
    /// View model for a stock item in inventory
    /// </summary>
    public class StockItemViewModel : ViewModelBase
    {
        private readonly StockItem _stockItem;
        private readonly IStockService _stockService;

        public StockItemViewModel(StockItem stockItem, IStockService stockService)
        {
            _stockItem = stockItem;
            _stockService = stockService;
        }

        public string PartCode => _stockItem.Part.Code;
        public string PartReference => _stockItem.Part.Reference;
        public string Dimensions => _stockItem.Part.Dimensions;
        public decimal UnitPrice => _stockItem.Part.Price;
        public PartCategory Category => _stockItem.Part.Category;

        public int CurrentStock
        {
            get => _stockItem.CurrentStock;
            set 
            { 
                _stockItem.CurrentStock = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(TotalValue));
                this.RaisePropertyChanged(nameof(StockLevelPercentage));
            }
        }

        public int ReservedStock => _stockItem.ReservedStock;
        public int AvailableStock => _stockItem.AvailableStock;
        public int MinimumStockLevel => _stockItem.MinimumStockLevel;
        public StockStatus StockStatus => _stockItem.Status;
        public bool NeedsReorder => _stockItem.NeedsReorder;
        public string WarehouseLocation => _stockItem.WarehouseLocation;
        public string Supplier => _stockItem.Supplier;

        public decimal TotalValue => CurrentStock * UnitPrice;
        public double StockLevelPercentage => _stockItem.StockLevelPercentage;

        public string StockStatusDisplay => StockStatus switch
        {
            StockStatus.InStock => "✅ In Stock",
            StockStatus.LowStock => "⚠️ Low Stock",
            StockStatus.OutOfStock => "❌ Out of Stock",
            _ => "❓ Unknown"
        };

        public string StockStatusColor => StockStatus switch
        {
            StockStatus.InStock => "Green",
            StockStatus.LowStock => "Orange",
            StockStatus.OutOfStock => "Red",
            _ => "Gray"
        };

        // Additional properties for XAML binding compatibility
        public string Code => PartCode;
        public string Name => PartReference;
        public int MinimumStock => _stockItem.MinimumStockLevel;
        public bool IsInStock => CurrentStock > 0;
        public bool IsLowStock => CurrentStock <= MinimumStock && CurrentStock > 0;
        public bool IsOutOfStock => CurrentStock <= 0;
        public string FormattedPrice => UnitPrice.ToString("C2");
        public string DeliveryDelayText => $"{_stockItem.Part.DeliveryDelay} days";
        public string LastUpdatedText => _stockItem.LastUpdated.ToString("MMM dd, yyyy");
    }

    /// <summary>
    /// Inventory filter options
    /// </summary>
    public enum InventoryFilter
    {
        All,
        InStock,
        LowStock,
        OutOfStock,
        NeedReorder
    }
}
