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
    /// <summary>
    /// ViewModel for managing and viewing customer orders (Admin only)
    /// </summary>
    public class OrderManagementViewModel : ViewModelBase
    {
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IPriceCalculatorService _priceCalculatorService;

        private ObservableCollection<CustomerOrderViewModel> _orders;
        private ObservableCollection<CustomerOrderViewModel> _filteredOrders;
        private CustomerOrderViewModel? _selectedOrder;
        private string _searchText = string.Empty;        private bool _isLoading;
        private string _statusMessage = "Ready";
        private OrderStatusFilter _filterStatus = OrderStatusFilter.All;

        public OrderManagementViewModel(ICustomerOrderService customerOrderService, IPriceCalculatorService priceCalculatorService)
        {
            _customerOrderService = customerOrderService;
            _priceCalculatorService = priceCalculatorService;
            
            _orders = new ObservableCollection<CustomerOrderViewModel>();
            _filteredOrders = new ObservableCollection<CustomerOrderViewModel>();

            // Initialize commands
            RefreshCommand = new SimpleAsyncCommand(RefreshOrders);
            SearchCommand = new SimpleCommand(ApplyFilter);
            ViewOrderDetailsCommand = new SimpleAsyncCommand<CustomerOrderViewModel>(ViewOrderDetails);
            ExportOrdersCommand = new SimpleCommand(ExportOrders);

            // Load initial data
            _ = Task.Run(async () =>
            {
                try
                {
                    await RefreshOrders();
                }
                catch (Exception ex)
                {
                    RunOnUIThread(() => {
                        StatusMessage = $"Error loading orders: {ex.Message}";
                    });
                }
            });
        }

        #region Properties

        public ObservableCollection<CustomerOrderViewModel> FilteredOrders
        {
            get => _filteredOrders;
            set => this.SafeRaiseAndSetIfChanged(ref _filteredOrders, value);
        }

        public CustomerOrderViewModel? SelectedOrder
        {
            get => _selectedOrder;
            set => this.SafeRaiseAndSetIfChanged(ref _selectedOrder, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                this.SafeRaiseAndSetIfChanged(ref _searchText, value);
                ApplyFilter();
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
            set => this.SafeRaiseAndSetIfChanged(ref _statusMessage, value);        }        public OrderStatusFilter FilterStatus
        {
            get => _filterStatus;
            set
            {
                this.SafeRaiseAndSetIfChanged(ref _filterStatus, value);
                ApplyFilter();
            }
        }

        public IEnumerable<OrderStatusFilter> StatusFilters { get; } = Enum.GetValues<OrderStatusFilter>();

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ViewOrderDetailsCommand { get; }
        public ICommand ExportOrdersCommand { get; }

        #endregion

        #region Methods

        private async Task RefreshOrders()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading orders...";

                var orders = await _customerOrderService.GetAllOrdersAsync();
                
                RunOnUIThread(() => {
                    _orders.Clear();
                    foreach (var order in orders)
                    {
                        var orderViewModel = new CustomerOrderViewModel(order, _priceCalculatorService);
                        _orders.Add(orderViewModel);
                    }

                    ApplyFilter();
                    StatusMessage = $"Loaded {_orders.Count} orders";
                });
            }
            catch (Exception ex)
            {
                RunOnUIThread(() => {
                    StatusMessage = $"Error loading orders: {ex.Message}";
                });
            }
            finally
            {
                RunOnUIThread(() => {
                    IsLoading = false;
                });
            }
        }

        private void ApplyFilter()
        {
            RunOnUIThread(() => {
                var filtered = _orders.AsEnumerable();

                // Apply text search
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var searchLower = SearchText.ToLower();
                    filtered = filtered.Where(o => 
                        o.CustomerName.ToLower().Contains(searchLower) ||
                        o.CustomerEmail.ToLower().Contains(searchLower) ||
                        o.OrderId.ToLower().Contains(searchLower));
                }                // Apply status filter
                if (FilterStatus != OrderStatusFilter.All)
                {
                    // Convert filter status to actual OrderStatus for comparison
                    var actualStatus = ConvertFilterStatusToOrderStatus(FilterStatus);
                    filtered = filtered.Where(o => o.Status == actualStatus);
                }

                FilteredOrders.Clear();
                foreach (var order in filtered.OrderByDescending(o => o.OrderDate))
                {
                    FilteredOrders.Add(order);
                }
            });
        }

        private async Task ViewOrderDetails(CustomerOrderViewModel? orderViewModel)
        {
            if (orderViewModel == null) return;

            try
            {
                // Ensure price breakdown is calculated
                await orderViewModel.CalculatePriceAsync();
                SelectedOrder = orderViewModel;
                StatusMessage = $"Viewing details for order {orderViewModel.OrderId}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading order details: {ex.Message}";
            }
        }        private void ExportOrders()
        {
            try
            {
                // TODO: Implement export functionality
                StatusMessage = "Export functionality not yet implemented";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Export error: {ex.Message}";
            }
        }

        private Services.OrderStatus ConvertFilterStatusToOrderStatus(OrderStatusFilter filterStatus)
        {
            return filterStatus switch
            {
                OrderStatusFilter.Pending => Services.OrderStatus.Pending,
                OrderStatusFilter.Confirmed => Services.OrderStatus.Confirmed,
                OrderStatusFilter.InProduction => Services.OrderStatus.InProduction,
                OrderStatusFilter.Shipped => Services.OrderStatus.ReadyForDelivery,
                OrderStatusFilter.Delivered => Services.OrderStatus.Delivered,
                OrderStatusFilter.Cancelled => Services.OrderStatus.Cancelled,
                _ => Services.OrderStatus.Pending
            };
        }

        #endregion
    }

    /// <summary>
    /// ViewModel wrapper for customer orders
    /// </summary>
    public class CustomerOrderViewModel : ViewModelBase
    {
        private readonly CustomerOrder _order;
        private readonly IPriceCalculatorService _priceCalculatorService;
        private PriceBreakdown? _priceBreakdown;
        private bool _isPriceCalculated;

        public CustomerOrderViewModel(CustomerOrder order, IPriceCalculatorService priceCalculatorService)
        {
            _order = order;
            _priceCalculatorService = priceCalculatorService;
        }        public string OrderId => $"#{_order.Id:D6}";
        public DateTime OrderDate => _order.OrderDate;public string CustomerName => _order.CustomerName;
        public string CustomerEmail => _order.CustomerEmail;
        public string CustomerPhone => _order.CustomerPhone;
        public string CustomerAddress => _order.CustomerAddress;
        public Services.OrderStatus Status => _order.Status;
        public CabinetConfiguration Configuration => _order.Configuration;

        public string FormattedOrderDate => OrderDate.ToString("yyyy-MM-dd HH:mm");
        public string StatusText => Status.ToString();
        
        public PriceBreakdown? PriceBreakdown
        {
            get => _priceBreakdown;
            private set => this.SafeRaiseAndSetIfChanged(ref _priceBreakdown, value);
        }

        public bool IsPriceCalculated
        {
            get => _isPriceCalculated;
            private set => this.SafeRaiseAndSetIfChanged(ref _isPriceCalculated, value);
        }        public string FormattedTotalPrice 
        { 
            get 
            {
                if (PriceBreakdown == null) 
                    return "Calculating...";
                return $"€{PriceBreakdown.TotalPrice:F2}";
            }
        }

        public async Task CalculatePriceAsync()
        {
            if (IsPriceCalculated) return;

            try
            {
                var breakdown = await _priceCalculatorService.CalculatePriceAsync(Configuration);
                PriceBreakdown = breakdown;
                IsPriceCalculated = true;
                this.SafeRaisePropertyChanged(nameof(FormattedTotalPrice));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating price for order {OrderId}: {ex.Message}");
            }
        }
    }    /// <summary>
    /// Order status for filtering
    /// </summary>
    public enum OrderStatusFilter
    {
        All,
        Pending,
        Confirmed,
        InProduction,
        Shipped,
        Delivered,
        Cancelled
    }
}
