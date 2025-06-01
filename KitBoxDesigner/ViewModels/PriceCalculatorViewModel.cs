using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using KitBoxDesigner.Models;
using KitBoxDesigner.Services;

namespace KitBoxDesigner.ViewModels
{
    public class PriceCalculatorViewModel : ViewModelBase
    {
        private readonly IPriceCalculatorService _priceCalculatorService;
        private readonly IPartService _partService;
        
        private CabinetConfiguration? _configuration;
        private PriceBreakdown? _priceBreakdown;
        private bool _isCalculating;
        private string _statusMessage = "";
        private bool _includeAssembly = true;
        private bool _includeDelivery = true;
        private double _customAssemblyHours;
        private double _assemblyHourlyRate = 25.0; // Default rate
        private double _deliveryDistance;
        private double _deliveryRatePerKm = 0.5; // Default rate

        public PriceCalculatorViewModel(IPriceCalculatorService priceCalculatorService, IPartService partService)
        {
            _priceCalculatorService = priceCalculatorService;
            _partService = partService;            LineItems = new ObservableCollection<PriceLineItem>();
            CalculatePriceCommand = new SimpleAsyncCommand(CalculatePriceAsync);
            RecalculateCommand = new SimpleAsyncCommand(RecalculateAsync);
            ExportPriceBreakdownCommand = new SimpleCommand(ExportPriceBreakdown);

            // Setup auto-recalculation using a simpler pattern to avoid threading issues
            SetupAutoRecalculation();
        }        public ObservableCollection<PriceLineItem> LineItems { get; }
        
        public ICommand CalculatePriceCommand { get; }
        public ICommand RecalculateCommand { get; }
        public ICommand ExportPriceBreakdownCommand { get; }public CabinetConfiguration? Configuration
        {
            get => _configuration;
            set => this.SafeRaiseAndSetIfChanged(ref _configuration, value);
        }

        public PriceBreakdown? PriceBreakdown
        {
            get => _priceBreakdown;
            set => this.SafeRaiseAndSetIfChanged(ref _priceBreakdown, value);
        }

        public bool IsCalculating
        {
            get => _isCalculating;
            set => this.SafeRaiseAndSetIfChanged(ref _isCalculating, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.SafeRaiseAndSetIfChanged(ref _statusMessage, value);
        }        public double AssemblyHourlyRate
        {
            get => _assemblyHourlyRate;
            set => this.SafeRaiseAndSetIfChanged(ref _assemblyHourlyRate, Math.Max(0, value));
        }

        public double DeliveryRatePerKm
        {
            get => _deliveryRatePerKm;
            set => this.SafeRaiseAndSetIfChanged(ref _deliveryRatePerKm, Math.Max(0, value));
        }// Calculated properties
        public double PartsSubtotal => (double)(PriceBreakdown?.PartsSubtotal ?? 0);
        public double AssemblySubtotal => (double)(PriceBreakdown?.AssemblySubtotal ?? 0);
        public double DeliverySubtotal => (double)(PriceBreakdown?.DeliverySubtotal ?? 0);
        public double Subtotal => (double)(PriceBreakdown?.Subtotal ?? 0);
        public double TaxAmount => (double)(PriceBreakdown?.TaxAmount ?? 0);
        public double TotalPrice => (double)(PriceBreakdown?.TotalPrice ?? 0);
        public double TaxRate => (double)(PriceBreakdown?.TaxRate ?? 0);

        public string FormattedPartsSubtotal => $"€{PartsSubtotal:F2}";
        public string FormattedAssemblySubtotal => $"€{AssemblySubtotal:F2}";
        public string FormattedDeliverySubtotal => $"€{DeliverySubtotal:F2}";
        public string FormattedSubtotal => $"€{Subtotal:F2}";
        public string FormattedTaxAmount => $"€{TaxAmount:F2} ({TaxRate:P1})";
        public string FormattedTotalPrice => $"€{TotalPrice:F2}";        public void SetConfiguration(CabinetConfiguration configuration)
        {
            Configuration = configuration;
            CalculatePriceCommand.Execute(null);
        }

        private async Task CalculatePriceAsync()
        {
            if (Configuration == null)
            {
                StatusMessage = "No configuration to calculate.";
                return;
            }

            try
            {
                IsCalculating = true;
                StatusMessage = "Calculating price...";
                
                var breakdown = await _priceCalculatorService.CalculatePriceAsync(Configuration);
                
                // Apply custom options
                if (!IncludeAssembly)
                {
                    breakdown.AssemblySubtotal = 0;
                }                else if (CustomAssemblyHours > 0)
                {
                    breakdown.AssemblySubtotal = (decimal)(CustomAssemblyHours * AssemblyHourlyRate);
                }
                
                if (!IncludeDelivery)
                {
                    breakdown.DeliverySubtotal = 0;
                }
                else if (DeliveryDistance > 0)
                {
                    breakdown.DeliverySubtotal = (decimal)(DeliveryDistance * DeliveryRatePerKm);
                }
                
                // Recalculate totals
                breakdown.Subtotal = breakdown.PartsSubtotal + breakdown.AssemblySubtotal + breakdown.DeliverySubtotal;
                breakdown.TaxAmount = breakdown.Subtotal * breakdown.TaxRate;
                breakdown.TotalPrice = breakdown.Subtotal + breakdown.TaxAmount;
                
                PriceBreakdown = breakdown;
                  // Update line items
                RunOnUIThread(() => {
                    LineItems.Clear();
                    foreach (var item in breakdown.LineItems)
                    {
                        LineItems.Add(item);
                    }
                });
                  // Update all calculated properties
                RunOnUIThread(() => {
                    this.SafeRaisePropertyChanged(nameof(PartsSubtotal));
                    this.SafeRaisePropertyChanged(nameof(AssemblySubtotal));
                    this.SafeRaisePropertyChanged(nameof(DeliverySubtotal));
                    this.SafeRaisePropertyChanged(nameof(Subtotal));
                    this.SafeRaisePropertyChanged(nameof(TaxAmount));
                    this.SafeRaisePropertyChanged(nameof(TotalPrice));
                    this.SafeRaisePropertyChanged(nameof(FormattedPartsSubtotal));
                    this.SafeRaisePropertyChanged(nameof(FormattedAssemblySubtotal));
                    this.SafeRaisePropertyChanged(nameof(FormattedDeliverySubtotal));
                    this.SafeRaisePropertyChanged(nameof(FormattedSubtotal));
                    this.SafeRaisePropertyChanged(nameof(FormattedTaxAmount));
                    this.SafeRaisePropertyChanged(nameof(FormattedTotalPrice));
                });
                OnPropertyChanged(nameof(FormattedSubtotal));
                OnPropertyChanged(nameof(FormattedTaxAmount));
                OnPropertyChanged(nameof(FormattedTotalPrice));
                
                StatusMessage = $"Price calculated: {FormattedTotalPrice}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error calculating price: {ex.Message}";
                PriceBreakdown = null;
            }
            finally
            {
                IsCalculating = false;
            }
        }

        private async Task RecalculateAsync()
        {
            await CalculatePriceAsync();
        }

        private void ExportPriceBreakdown()
        {
            if (PriceBreakdown == null)
            {
                StatusMessage = "No price breakdown to export.";
                return;
            }

            try
            {
                // Generate a simple text-based price breakdown
                var breakdown = GenerateTextBreakdown();
                
                // In a real app, you'd use a file dialog or export service
                // For now, just copy to clipboard or show in a message
                StatusMessage = "Price breakdown ready for export.";
                
                // TODO: Implement actual export functionality
                // - Save to file
                // - Copy to clipboard
                // - Email
                // - Print
            }
            catch (Exception ex)
            {
                StatusMessage = $"Export error: {ex.Message}";
            }
        }

        private string GenerateTextBreakdown()
        {
            if (PriceBreakdown == null) return "";
            
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("KITBOX CABINET PRICE BREAKDOWN");
            sb.AppendLine("==============================");
            sb.AppendLine();
            sb.AppendLine($"Configuration: {Configuration?.Width}W x {Configuration?.Depth}D x {Configuration?.Height}H cm");
            sb.AppendLine($"Compartments: {Configuration?.Compartments.Count ?? 0}");
            sb.AppendLine($"Color: {Configuration?.Color}");
            sb.AppendLine();
            sb.AppendLine("PARTS:");
            
            foreach (var item in LineItems)
            {
                sb.AppendLine($"  {item.Description} x{item.Quantity} @ €{item.UnitPrice:F2} = €{item.TotalPrice:F2}");
            }
            
            sb.AppendLine();
            sb.AppendLine($"Parts Subtotal:     {FormattedPartsSubtotal}");
            
            if (IncludeAssembly && AssemblySubtotal > 0)
                sb.AppendLine($"Assembly:           {FormattedAssemblySubtotal}");
                
            if (IncludeDelivery && DeliverySubtotal > 0)
                sb.AppendLine($"Delivery:           {FormattedDeliverySubtotal}");
                
            sb.AppendLine($"Subtotal:           {FormattedSubtotal}");
            sb.AppendLine($"Tax:                {FormattedTaxAmount}");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"TOTAL:              {FormattedTotalPrice}");
            
            return sb.ToString();
        }

        private void SetupAutoRecalculation()
        {
            // Setup property change notifications for auto-recalculation without complex reactive bindings
            // This approach avoids threading issues by handling recalculation in property setters
        }        // Properties with auto-recalculation
        public bool IncludeAssembly
        {
            get => _includeAssembly;
            set 
            { 
                if (this.SafeRaiseAndSetIfChanged(ref _includeAssembly, value))
                {
                    TriggerRecalculation();
                }
            }
        }

        public bool IncludeDelivery
        {
            get => _includeDelivery;
            set 
            { 
                if (this.SafeRaiseAndSetIfChanged(ref _includeDelivery, value))
                {
                    TriggerRecalculation();
                }
            }
        }

        public double CustomAssemblyHours
        {
            get => _customAssemblyHours;
            set 
            { 
                if (this.SafeRaiseAndSetIfChanged(ref _customAssemblyHours, Math.Max(0, value)))
                {
                    TriggerRecalculation();
                }
            }
        }

        public double DeliveryDistance
        {
            get => _deliveryDistance;
            set 
            { 
                if (this.SafeRaiseAndSetIfChanged(ref _deliveryDistance, Math.Max(0, value)))
                {
                    TriggerRecalculation();
                }
            }
        }

        private void TriggerRecalculation()
        {
            if (Configuration != null)
            {
                RunOnUIThread(async () =>
                {
                    try
                    {
                        await RecalculateAsync();
                    }
                    catch (Exception ex)
                    {
                        StatusMessage = $"Error during recalculation: {ex.Message}";
                    }
                });
            }
        }
    }
}
