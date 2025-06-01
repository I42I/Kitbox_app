using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using KitBoxDesigner.Models;
using KitBoxDesigner.Services;

namespace KitBoxDesigner.ViewModels
{
    /// <summary>
    /// View model for completing a cabinet order with customer information
    /// </summary>
    public class OrderCompletionViewModel : ViewModelBase
    {
        private readonly IPriceCalculatorService _priceCalculatorService;
        private readonly ICustomerOrderService _customerOrderService;
        
        private CabinetConfiguration _configuration;
        private CustomerInfo _customerInfo;
        private PriceBreakdown? _priceBreakdown;
        private bool _isProcessing;
        private string _statusMessage = "";
        private bool _isCompleted;

        public OrderCompletionViewModel(
            IPriceCalculatorService priceCalculatorService,
            ICustomerOrderService customerOrderService,
            CabinetConfiguration configuration)
        {
            _priceCalculatorService = priceCalculatorService ?? throw new ArgumentNullException(nameof(priceCalculatorService));
            _customerOrderService = customerOrderService ?? throw new ArgumentNullException(nameof(customerOrderService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            
            _customerInfo = new CustomerInfo();
            
            // Commands
            CalculatePriceCommand = new SimpleAsyncCommand(CalculatePriceAsync);
            CompleteOrderCommand = new SimpleAsyncCommand(CompleteOrderAsync);
            CancelCommand = new SimpleCommand(Cancel);
            
            // Calculate initial price
            _ = Task.Run(CalculatePriceAsync);
        }

        public ICommand CalculatePriceCommand { get; }
        public ICommand CompleteOrderCommand { get; }
        public ICommand CancelCommand { get; }

        public CabinetConfiguration Configuration
        {
            get => _configuration;
            set => this.SafeRaiseAndSetIfChanged(ref _configuration, value);
        }

        public CustomerInfo CustomerInfo
        {
            get => _customerInfo;
            set => this.SafeRaiseAndSetIfChanged(ref _customerInfo, value);
        }

        public PriceBreakdown? PriceBreakdown
        {
            get => _priceBreakdown;
            set => this.SafeRaiseAndSetIfChanged(ref _priceBreakdown, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => this.SafeRaiseAndSetIfChanged(ref _isProcessing, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.SafeRaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => this.SafeRaiseAndSetIfChanged(ref _isCompleted, value);
        }

        public decimal SuggestedDeposit => PriceBreakdown?.TotalPrice * 0.3m ?? 0m; // 30% deposit suggestion

        private async Task CalculatePriceAsync()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = "Calcul du prix en cours...";
                
                var breakdown = await _priceCalculatorService.CalculatePriceAsync(Configuration);
                PriceBreakdown = breakdown;
                
                // Set suggested deposit amount
                CustomerInfo.DepositAmount = SuggestedDeposit;
                
                StatusMessage = "Prix calculé avec succès";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors du calcul: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task CompleteOrderAsync()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = "Finalisation de la commande...";
                
                // Validate customer info
                if (string.IsNullOrWhiteSpace(CustomerInfo.Name))
                {
                    StatusMessage = "Le nom du client est requis";
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(CustomerInfo.Email))
                {
                    StatusMessage = "L'email est requis";
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(CustomerInfo.Phone))
                {
                    StatusMessage = "Le numéro de téléphone est requis";
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(CustomerInfo.Address))
                {
                    StatusMessage = "L'adresse est requise";
                    return;
                }

                if (CustomerInfo.DepositAmount < 0)
                {
                    StatusMessage = "Le montant du dépôt doit être positif";
                    return;
                }

                // Save customer order
                var orderId = await _customerOrderService.SaveCustomerOrderAsync(
                    Configuration,
                    CustomerInfo.Name,
                    CustomerInfo.Email,
                    CustomerInfo.Phone,
                    CustomerInfo.Address);

                StatusMessage = $"Commande #{orderId} créée avec succès!";
                IsCompleted = true;

                // Raise event for navigation
                OrderCompleted?.Invoke(this, new OrderCompletedEventArgs 
                { 
                    OrderId = orderId,
                    CustomerInfo = CustomerInfo,
                    Configuration = Configuration,
                    PriceBreakdown = PriceBreakdown
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la finalisation: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private void Cancel()
        {
            OrderCancelled?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<OrderCompletedEventArgs>? OrderCompleted;
        public event EventHandler? OrderCancelled;
    }

    /// <summary>
    /// Event arguments for order completion
    /// </summary>
    public class OrderCompletedEventArgs : EventArgs
    {
        public int OrderId { get; set; }
        public CustomerInfo CustomerInfo { get; set; } = new();
        public CabinetConfiguration Configuration { get; set; } = new();
        public PriceBreakdown? PriceBreakdown { get; set; }
    }
}
