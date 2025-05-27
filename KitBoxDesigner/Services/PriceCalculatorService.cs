using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Services
{
    /// <summary>
    /// Implementation of price calculator service for generating quotes and calculating prices
    /// </summary>
    public class PriceCalculatorService : IPriceCalculatorService
    {
        private readonly IPartService _partService;
        private readonly IStockService _stockService;
        private readonly List<PriceBreakdown> _priceHistory;

        public PriceCalculatorService(IPartService partService, IStockService stockService)
        {
            _partService = partService;
            _stockService = stockService;
            _priceHistory = new List<PriceBreakdown>();
        }

        /// <inheritdoc />
        public async Task<PriceBreakdown> CalculatePriceAsync(CabinetConfiguration configuration)
        {
            var requirements = await _partService.GetRequiredPartsAsync(configuration);
            var priceBreakdown = await CalculatePartsListPriceAsync(requirements);
            
            priceBreakdown.ConfigurationId = configuration.Id;
            
            // Add assembly cost if requested
            if (configuration.IncludeAssembly)
            {
                priceBreakdown.AssemblyCost = GetAssemblyServicePrice(configuration);
                priceBreakdown.IncludesAssembly = true;
            }

            // Add delivery cost (standard delivery for now)
            priceBreakdown.DeliveryCost = 25.00m; // Standard delivery
            priceBreakdown.IncludesDelivery = true;

            // Store in history
            _priceHistory.Add(priceBreakdown);

            return priceBreakdown;
        }        /// <inheritdoc />
        public async Task<PriceBreakdown> CalculatePartsListPriceAsync(List<PartRequirement> requirements)
        {
            var priceBreakdown = new PriceBreakdown();
            var stockAvailability = await _stockService.CheckStockAvailabilityAsync(requirements);
            bool hasStockIssues = false;
            int cornerIronCount = 0;

            foreach (var requirement in requirements)
            {
                var part = await _partService.GetPartByCodeAsync(requirement.PartCode);
                if (part == null) continue;

                var stockStatus = stockAvailability.GetValueOrDefault(part.Code, StockStatus.OutOfStock);
                var isAvailable = stockStatus != StockStatus.OutOfStock;
                
                // Check for stock issues
                if (!isAvailable || stockStatus == StockStatus.LowStock)
                {
                    hasStockIssues = true;
                }

                // Count corner irons (assuming corner irons have "iron" in their reference)
                if (part.Reference.Contains("iron", StringComparison.OrdinalIgnoreCase) || 
                    part.Reference.Contains("corner", StringComparison.OrdinalIgnoreCase))
                {
                    cornerIronCount += requirement.Quantity;
                }

                var lineItem = new PriceLineItem
                {
                    PartCode = part.Code,
                    Description = $"{part.Reference} - {part.Dimensions}",
                    Quantity = requirement.Quantity,
                    UnitPrice = part.Price,
                    Category = part.Category,
                    DeliveryDelay = part.DeliveryDelay,
                    StockStatus = stockStatus,
                    IsAvailable = isAvailable
                };

                priceBreakdown.LineItems.Add(lineItem);
            }

            // Set new properties
            priceBreakdown.HasStockIssues = hasStockIssues;
            priceBreakdown.CornerIronCount = cornerIronCount;

            return priceBreakdown;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, decimal>> GetAssemblyServicePricesAsync()
        {
            await Task.Delay(10);
            return AssemblyServices.ServicePrices;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, decimal>> GetDeliveryServicePricesAsync()
        {
            await Task.Delay(10);
            return DeliveryOptions.DeliveryPrices;
        }

        /// <inheritdoc />
        public async Task<PriceBreakdown> ApplyDiscountAsync(PriceBreakdown priceBreakdown, decimal discountPercentage)
        {
            await Task.Delay(10);
            priceBreakdown.ApplyPercentageDiscount(discountPercentage);
            return priceBreakdown;
        }

        /// <inheritdoc />
        public async Task<PriceBreakdown> ApplyFixedDiscountAsync(PriceBreakdown priceBreakdown, decimal discountAmount)
        {
            await Task.Delay(10);
            priceBreakdown.ApplyFixedDiscount(discountAmount);
            return priceBreakdown;
        }

        /// <inheritdoc />
        public async Task<DateTime> CalculateEstimatedDeliveryDateAsync(CabinetConfiguration configuration)
        {
            var requirements = await _partService.GetRequiredPartsAsync(configuration);
            var deliveryDates = await _stockService.GetEstimatedDeliveryDatesAsync(requirements);
            
            // Return the latest delivery date among all parts
            return deliveryDates.Values.Any() ? deliveryDates.Values.Max() : DateTime.Now.AddDays(14);
        }

        /// <inheritdoc />
        public async Task<List<PriceBreakdown>> GetPriceHistoryAsync(Guid configurationId)
        {
            await Task.Delay(20);
            return _priceHistory
                .Where(p => p.ConfigurationId == configurationId)
                .OrderByDescending(p => p.CalculationDate)
                .ToList();
        }

        /// <inheritdoc />
        public async Task<QuoteDocument> GenerateQuoteAsync(CabinetConfiguration configuration, PriceBreakdown priceBreakdown)
        {
            await Task.Delay(100);
            
            var quote = new QuoteDocument
            {
                QuoteNumber = GenerateQuoteNumber(),
                Configuration = configuration,
                PriceBreakdown = priceBreakdown,
                ValidUntil = DateTime.Now.AddDays(priceBreakdown.ValidityPeriodDays),
                Status = QuoteStatus.Draft
            };

            return quote;
        }

        /// <inheritdoc />
        public async Task<OptimizationResult> CheckOptimizationAsync(CabinetConfiguration configuration)
        {
            await Task.Delay(150);
            
            var result = new OptimizationResult
            {
                IsOptimized = true,
                Suggestions = new List<string>(),
                Alternatives = new List<AlternativePart>()
            };

            // Analyze configuration for optimization opportunities
            if (configuration.CompartmentCount > 5)
            {
                result.Suggestions.Add("Consider using fewer compartments to reduce assembly complexity and cost");
                result.PotentialSavings += 15.00m;
            }

            // Check for standard vs non-standard dimensions
            var nonStandardCompartments = configuration.Compartments
                .Where(c => !IsStandardDimension(c.Width) || !IsStandardDimension(c.Height) || !IsStandardDimension(c.Depth))
                .ToList();

            if (nonStandardCompartments.Any())
            {
                result.Suggestions.Add("Some compartments use non-standard dimensions which may increase cost");
                result.IsOptimized = false;
            }

            // Check for bulk discounts
            if (configuration.CompartmentCount >= 3)
            {
                result.Suggestions.Add("Eligible for bulk discount on multiple compartments");
                result.PotentialSavings += configuration.CompartmentCount * 2.50m;
            }

            return result;
        }

        /// <summary>
        /// Get assembly service price based on configuration complexity
        /// </summary>
        private decimal GetAssemblyServicePrice(CabinetConfiguration configuration)
        {
            var basePrice = AssemblyServices.ServicePrices["Basic Assembly"];
            
            // Add complexity cost based on number of compartments
            var complexityCost = (configuration.CompartmentCount - 1) * 10.00m;
            
            // Add door assembly cost
            var doorCount = configuration.Compartments.Count(c => c.HasDoor);
            var doorCost = doorCount * 5.00m;

            return basePrice + complexityCost + doorCost;
        }

        /// <summary>
        /// Check if dimension is standard (reduces cost)
        /// </summary>
        private bool IsStandardDimension(int dimension)
        {
            var standardDimensions = new[] { 32, 42, 52, 62 };
            return standardDimensions.Contains(dimension);
        }

        /// <summary>
        /// Generate unique quote number
        /// </summary>
        private string GenerateQuoteNumber()
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            var random = new Random().Next(1000, 9999);
            return $"KB-{date}-{random}";
        }

        /// <summary>
        /// Calculate bulk discount based on quantity
        /// </summary>
        public decimal CalculateBulkDiscount(int totalQuantity, decimal totalAmount)
        {
            if (totalQuantity >= 50) return totalAmount * 0.15m; // 15% for 50+ items
            if (totalQuantity >= 30) return totalAmount * 0.10m; // 10% for 30+ items
            if (totalQuantity >= 20) return totalAmount * 0.05m; // 5% for 20+ items
            return 0;
        }

        /// <summary>
        /// Get seasonal pricing adjustments
        /// </summary>
        public decimal GetSeasonalAdjustment()
        {
            var month = DateTime.Now.Month;
            
            // Summer discount (June-August)
            if (month >= 6 && month <= 8) return -0.05m; // 5% discount
            
            // Holiday premium (November-December)
            if (month >= 11) return 0.10m; // 10% premium
            
            return 0; // No adjustment
        }
    }
}
