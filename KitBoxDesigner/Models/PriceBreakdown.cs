using System;
using System.Collections.Generic;
using System.Linq;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Models
{
    /// <summary>
    /// Represents a detailed price breakdown for a cabinet configuration
    /// </summary>
    public class PriceBreakdown
    {
        /// <summary>
        /// Unique identifier for this price calculation
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Reference to the cabinet configuration
        /// </summary>
        public Guid ConfigurationId { get; set; }

        /// <summary>
        /// Individual line items for parts
        /// </summary>
        public List<PriceLineItem> LineItems { get; set; } = new();

        /// <summary>
        /// Assembly service cost
        /// </summary>
        public decimal AssemblyCost { get; set; }

        /// <summary>
        /// Delivery cost
        /// </summary>
        public decimal DeliveryCost { get; set; }

        /// <summary>
        /// Tax rate applied (e.g., 0.21 for 21% VAT)
        /// </summary>
        public decimal TaxRate { get; set; } = 0.21m;

        /// <summary>
        /// Discount amount applied
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Discount percentage applied
        /// </summary>
        public decimal DiscountPercentage { get; set; }

        /// <summary>
        /// Date when price was calculated
        /// </summary>
        public DateTime CalculationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Currency code (default EUR)
        /// </summary>
        public string Currency { get; set; } = "EUR";

        /// <summary>
        /// Price validity period in days
        /// </summary>
        public int ValidityPeriodDays { get; set; } = 30;

        /// <summary>
        /// Whether assembly is included
        /// </summary>
        public bool IncludesAssembly { get; set; }

        /// <summary>
        /// Whether delivery is included
        /// </summary>
        public bool IncludesDelivery { get; set; }

        /// <summary>
        /// Whether there are stock issues with any components
        /// </summary>
        public bool HasStockIssues { get; set; }

        /// <summary>
        /// Number of corner irons required for this configuration
        /// </summary>
        public int CornerIronCount { get; set; }

        /// <summary>
        /// Subtotal of all parts (before assembly, delivery, tax)
        /// </summary>
        public decimal PartsSubtotal => LineItems.Sum(item => item.TotalPrice);

        /// <summary>
        /// Assembly subtotal (includes assembly cost)
        /// </summary>
        public decimal AssemblySubtotal { get; set; }

        /// <summary>
        /// Delivery subtotal (includes parts, assembly, and delivery)
        /// </summary>
        public decimal DeliverySubtotal { get; set; }

        /// <summary>
        /// Subtotal before discounts and tax
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Total before tax (parts + assembly + delivery - discounts)
        /// </summary>
        public decimal SubtotalBeforeTax
        {
            get
            {
                var subtotal = PartsSubtotal + AssemblyCost + DeliveryCost;
                subtotal -= DiscountAmount;
                subtotal -= (subtotal * DiscountPercentage / 100);
                return Math.Max(0, subtotal);
            }
        }        /// <summary>
        /// Tax amount
        /// </summary>
        public decimal TaxAmount { get; set; }        /// <summary>
        /// Final total including tax
        /// </summary>
        public decimal TotalWithTax => SubtotalBeforeTax + TaxAmount;

        /// <summary>
        /// Total price (settable for flexibility)
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Total savings from discounts
        /// </summary>
        public decimal TotalSavings
        {
            get
            {
                var percentageDiscount = PartsSubtotal * DiscountPercentage / 100;
                return DiscountAmount + percentageDiscount;
            }
        }

        /// <summary>
        /// Price expiration date
        /// </summary>
        public DateTime ExpirationDate => CalculationDate.AddDays(ValidityPeriodDays);

        /// <summary>
        /// Check if price quote is still valid
        /// </summary>
        public bool IsValid => DateTime.Now <= ExpirationDate;

        /// <summary>
        /// Days remaining for price validity
        /// </summary>
        public int DaysRemaining => Math.Max(0, (ExpirationDate - DateTime.Now).Days);

        /// <summary>
        /// Formatted parts subtotal for display
        /// </summary>
        public string FormattedPartsSubtotal => $"€{PartsSubtotal:F2}";

        /// <summary>
        /// Formatted assembly subtotal for display
        /// </summary>
        public string FormattedAssemblySubtotal => $"€{AssemblySubtotal:F2}";

        /// <summary>
        /// Formatted delivery subtotal for display
        /// </summary>
        public string FormattedDeliverySubtotal => $"€{DeliverySubtotal:F2}";

        /// <summary>
        /// Formatted subtotal for display
        /// </summary>
        public string FormattedSubtotal => $"€{Subtotal:F2}";

        /// <summary>
        /// Formatted total price for display
        /// </summary>
        public string FormattedTotalPrice => $"€{TotalPrice:F2}";

        /// <summary>
        /// Get breakdown by category
        /// </summary>
        public Dictionary<PartCategory, decimal> GetBreakdownByCategory()
        {
            return LineItems
                .GroupBy(item => item.Category)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(item => item.TotalPrice)
                );
        }

        /// <summary>
        /// Get most expensive items (top N)
        /// </summary>
        public List<PriceLineItem> GetMostExpensiveItems(int count = 5)
        {
            return LineItems
                .OrderByDescending(item => item.TotalPrice)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Apply percentage discount
        /// </summary>
        public void ApplyPercentageDiscount(decimal percentage)
        {
            DiscountPercentage = Math.Max(0, Math.Min(100, percentage));
        }

        /// <summary>
        /// Apply fixed amount discount
        /// </summary>
        public void ApplyFixedDiscount(decimal amount)
        {
            DiscountAmount = Math.Max(0, Math.Min(PartsSubtotal, amount));
        }

        /// <summary>
        /// Clear all discounts
        /// </summary>
        public void ClearDiscounts()
        {
            DiscountAmount = 0;
            DiscountPercentage = 0;
        }

        /// <summary>
        /// Add or update a line item
        /// </summary>
        public void AddOrUpdateLineItem(string partCode, int quantity, decimal unitPrice, PartCategory category, string description = "")
        {
            var existingItem = LineItems.FirstOrDefault(item => item.PartCode == partCode);
            
            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
                existingItem.UnitPrice = unitPrice;
                existingItem.Description = description;
            }
            else
            {
                LineItems.Add(new PriceLineItem
                {
                    PartCode = partCode,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    Category = category,
                    Description = description
                });
            }
        }

        /// <summary>
        /// Remove a line item
        /// </summary>
        public bool RemoveLineItem(string partCode)
        {
            var item = LineItems.FirstOrDefault(item => item.PartCode == partCode);
            if (item != null)
            {
                LineItems.Remove(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get formatted price summary
        /// </summary>
        public string GetPriceSummary()
        {
            return $"Parts: {PartsSubtotal:C} | Assembly: {AssemblyCost:C} | Tax: {TaxAmount:C} | Total: {TotalWithTax:C}";
        }
    }

    /// <summary>
    /// Represents a single line item in the price breakdown
    /// </summary>
    public class PriceLineItem
    {
        /// <summary>
        /// Part code/reference
        /// </summary>
        public string PartCode { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Quantity required
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Total price for this line (quantity × unit price)
        /// </summary>
        public decimal TotalPrice => Quantity * UnitPrice;

        /// <summary>
        /// Formatted unit price for display
        /// </summary>
        public string FormattedUnitPrice => $"€{UnitPrice:F2}";

        /// <summary>
        /// Formatted total price for display
        /// </summary>
        public string FormattedTotalPrice => $"€{TotalPrice:F2}";

        /// <summary>
        /// Category of the part
        /// </summary>
        public PartCategory Category { get; set; }

        /// <summary>
        /// Whether this item is available in stock
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Delivery delay for this item (in days)
        /// </summary>
        public int DeliveryDelay { get; set; }

        /// <summary>
        /// Stock status for this item
        /// </summary>
        public StockStatus StockStatus { get; set; } = StockStatus.InStock;

        /// <summary>
        /// Notes about this line item
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{PartCode} - {Description} | Qty: {Quantity} × {UnitPrice:C} = {TotalPrice:C}";
        }
    }

    /// <summary>
    /// Predefined assembly service packages
    /// </summary>
    public static class AssemblyServices
    {
        public static readonly Dictionary<string, decimal> ServicePrices = new()
        {
            { "Basic Assembly", 50.00m },
            { "Premium Assembly", 75.00m },
            { "White Glove Service", 120.00m },
            { "Express Assembly", 100.00m }
        };

        public static readonly Dictionary<string, string> ServiceDescriptions = new()
        {
            { "Basic Assembly", "Standard assembly at our workshop" },
            { "Premium Assembly", "Assembly with quality inspection and testing" },
            { "White Glove Service", "Assembly, delivery, and installation at customer location" },
            { "Express Assembly", "Priority assembly within 24 hours" }
        };
    }

    /// <summary>
    /// Delivery options and pricing
    /// </summary>
    public static class DeliveryOptions
    {
        public static readonly Dictionary<string, decimal> DeliveryPrices = new()
        {
            { "Standard", 25.00m },
            { "Express", 45.00m },
            { "Premium", 65.00m },
            { "White Glove", 95.00m }
        };

        public static readonly Dictionary<string, int> DeliveryDays = new()
        {
            { "Standard", 7 },
            { "Express", 3 },
            { "Premium", 1 },
            { "White Glove", 2 }
        };

        public static readonly Dictionary<string, string> DeliveryDescriptions = new()
        {
            { "Standard", "Delivery to curb within 5-7 business days" },
            { "Express", "Priority delivery within 2-3 business days" },
            { "Premium", "Next business day delivery" },
            { "White Glove", "Delivery and installation inside your home" }
        };
    }
}
