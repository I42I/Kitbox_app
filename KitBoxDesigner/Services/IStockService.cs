using System.Collections.Generic;
using System.Threading.Tasks;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Services
{
    /// <summary>
    /// Service interface for managing stock and inventory operations
    /// </summary>
    public interface IStockService
    {
        /// <summary>
        /// Get all stock items with current inventory information
        /// </summary>
        Task<List<StockItem>> GetAllStockItemsAsync();

        /// <summary>
        /// Get stock item by part code
        /// </summary>
        Task<StockItem?> GetStockItemByCodeAsync(string partCode);

        /// <summary>
        /// Get stock items with low stock levels
        /// </summary>
        Task<List<StockItem>> GetLowStockItemsAsync();

        /// <summary>
        /// Get stock items that are out of stock
        /// </summary>
        Task<List<StockItem>> GetOutOfStockItemsAsync();

        /// <summary>
        /// Get stock items that need reordering
        /// </summary>
        Task<List<StockItem>> GetItemsNeedingReorderAsync();

        /// <summary>
        /// Update stock quantity for a part
        /// </summary>
        Task<bool> UpdateStockAsync(string partCode, int newQuantity, string reason = "Manual Update");

        /// <summary>
        /// Add stock (incoming delivery)
        /// </summary>
        Task<bool> AddStockAsync(string partCode, int quantity, string reason = "Delivery");

        /// <summary>
        /// Remove stock (sale/usage)
        /// </summary>
        Task<bool> RemoveStockAsync(string partCode, int quantity, string reason = "Sale");

        /// <summary>
        /// Reserve stock for an order
        /// </summary>
        Task<bool> ReserveStockAsync(string partCode, int quantity);

        /// <summary>
        /// Release reserved stock
        /// </summary>
        Task<bool> ReleaseReservedStockAsync(string partCode, int quantity);

        /// <summary>
        /// Check stock availability for multiple parts
        /// </summary>
        Task<Dictionary<string, StockStatus>> CheckStockAvailabilityAsync(List<PartRequirement> requirements);

        /// <summary>
        /// Get stock movements/history for a part
        /// </summary>
        Task<List<StockMovement>> GetStockMovementsAsync(string partCode, int daysBack = 30);

        /// <summary>
        /// Get stock alerts (low stock, out of stock, etc.)
        /// </summary>
        Task<List<StockAlert>> GetStockAlertsAsync();        /// <summary>
        /// Calculate estimated delivery date for parts based on stock status
        /// </summary>
        Task<Dictionary<string, System.DateTime>> GetEstimatedDeliveryDatesAsync(List<PartRequirement> requirements);

        /// <summary>
        /// Get stock summary statistics
        /// </summary>
        Task<StockSummary> GetStockSummaryAsync();

        /// <summary>
        /// Get specific stock item by part code
        /// </summary>
        Task<StockItem?> GetStockItemAsync(string partCode);

        /// <summary>
        /// Check availability for a specific part and quantity
        /// </summary>
        Task<bool> CheckAvailabilityAsync(string partCode, int requiredQuantity);
    }/// <summary>
    /// Represents a stock alert
    /// </summary>
    public class StockAlert
    {
        public System.Guid Id { get; set; } = System.Guid.NewGuid();
        public string PartCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public StockAlertType Type { get; set; }
        public System.DateTime Timestamp { get; set; } = System.DateTime.Now;
        public bool IsAcknowledged { get; set; }
    }

    /// <summary>
    /// Types of stock alerts
    /// </summary>
    public enum StockAlertType
    {
        LowStock,
        OutOfStock,
        ReorderRequired,
        Overstocked,
        DelayedDelivery
    }
}
