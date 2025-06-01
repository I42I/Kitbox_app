using System.Collections.Generic;
using System.Threading.Tasks;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Services
{
    /// <summary>
    /// Service interface for calculating prices and generating quotes
    /// </summary>
    public interface IPriceCalculatorService
    {
        /// <summary>
        /// Calculate complete price breakdown for a cabinet configuration
        /// </summary>
        Task<PriceBreakdown> CalculatePriceAsync(CabinetConfiguration configuration);

        /// <summary>
        /// Calculate price for specific parts list
        /// </summary>
        Task<PriceBreakdown> CalculatePartsListPriceAsync(List<PartRequirement> requirements);

        /// <summary>
        /// Get assembly service pricing options
        /// </summary>
        Task<Dictionary<string, decimal>> GetAssemblyServicePricesAsync();

        /// <summary>
        /// Get delivery service pricing options
        /// </summary>
        Task<Dictionary<string, decimal>> GetDeliveryServicePricesAsync();

        /// <summary>
        /// Apply discount to price breakdown
        /// </summary>
        Task<PriceBreakdown> ApplyDiscountAsync(PriceBreakdown priceBreakdown, decimal discountPercentage);

        /// <summary>
        /// Apply fixed discount amount to price breakdown
        /// </summary>
        Task<PriceBreakdown> ApplyFixedDiscountAsync(PriceBreakdown priceBreakdown, decimal discountAmount);

        /// <summary>
        /// Calculate total delivery date based on all parts availability
        /// </summary>
        Task<System.DateTime> CalculateEstimatedDeliveryDateAsync(CabinetConfiguration configuration);

        /// <summary>
        /// Get price history for tracking
        /// </summary>
        Task<List<PriceBreakdown>> GetPriceHistoryAsync(System.Guid configurationId);

        /// <summary>
        /// Generate quote document
        /// </summary>
        Task<QuoteDocument> GenerateQuoteAsync(CabinetConfiguration configuration, PriceBreakdown priceBreakdown);

        /// <summary>
        /// Check if configuration is cost-optimized
        /// </summary>
        Task<OptimizationResult> CheckOptimizationAsync(CabinetConfiguration configuration);
    }

    /// <summary>
    /// Represents a generated quote document
    /// </summary>
    public class QuoteDocument
    {
        public System.Guid Id { get; set; } = System.Guid.NewGuid();
        public string QuoteNumber { get; set; } = string.Empty;
        public CabinetConfiguration Configuration { get; set; } = new();
        public PriceBreakdown PriceBreakdown { get; set; } = new();
        public System.DateTime CreatedDate { get; set; } = System.DateTime.Now;
        public System.DateTime ValidUntil { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public QuoteStatus Status { get; set; } = QuoteStatus.Draft;
    }

    /// <summary>
    /// Quote status enumeration
    /// </summary>
    public enum QuoteStatus
    {
        Draft,
        Sent,
        Accepted,
        Rejected,
        Expired
    }

    /// <summary>
    /// Result of configuration optimization analysis
    /// </summary>
    public class OptimizationResult
    {
        public bool IsOptimized { get; set; }
        public List<string> Suggestions { get; set; } = new();
        public decimal PotentialSavings { get; set; }
        public List<AlternativePart> Alternatives { get; set; } = new();
    }

    /// <summary>
    /// Alternative part suggestion for optimization
    /// </summary>
    public class AlternativePart
    {
        public string OriginalPartCode { get; set; } = string.Empty;
        public string AlternativePartCode { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public decimal Savings { get; set; }
    }
}
