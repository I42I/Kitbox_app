using System;
using System.Collections.Generic;

namespace KitBoxDesigner.Models
{
    /// <summary>
    /// Represents a stock item with current inventory information
    /// </summary>
    public class StockItem
    {
        /// <summary>
        /// Unique identifier for this stock item
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Reference to the part
        /// </summary>
        public Part Part { get; set; } = new();

        /// <summary>
        /// Current quantity in stock
        /// </summary>
        public int CurrentStock { get; set; }

        /// <summary>
        /// Reserved quantity (for pending orders)
        /// </summary>
        public int ReservedStock { get; set; }

        /// <summary>
        /// Available quantity (current - reserved)
        /// </summary>
        public int AvailableStock => Math.Max(0, CurrentStock - ReservedStock);

        /// <summary>
        /// Minimum stock level for reorder alerts
        /// </summary>
        public int MinimumStockLevel { get; set; } = 10;

        /// <summary>
        /// Maximum stock level for inventory management
        /// </summary>
        public int MaximumStockLevel { get; set; } = 100;

        /// <summary>
        /// Reorder point quantity
        /// </summary>
        public int ReorderPoint { get; set; } = 20;

        /// <summary>
        /// Quantity to reorder when stock is low
        /// </summary>
        public int ReorderQuantity { get; set; } = 50;

        /// <summary>
        /// Last time stock was updated
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// Expected restock date
        /// </summary>
        public DateTime? ExpectedRestockDate { get; set; }

        /// <summary>
        /// Supplier information
        /// </summary>
        public string Supplier { get; set; } = string.Empty;

        /// <summary>
        /// Location in warehouse
        /// </summary>
        public string WarehouseLocation { get; set; } = string.Empty;

        /// <summary>
        /// Check if item needs reordering
        /// </summary>
        public bool NeedsReorder => AvailableStock <= ReorderPoint;        /// <summary>
        /// Check if item is overstocked
        /// </summary>
        public bool IsOverstocked => CurrentStock > MaximumStockLevel;

        /// <summary>
        /// Check if item is out of stock
        /// </summary>
        public bool IsOutOfStock => AvailableStock <= 0;

        /// <summary>
        /// Check if item has low stock
        /// </summary>
        public bool IsLowStock => AvailableStock > 0 && AvailableStock <= MinimumStockLevel;

        /// <summary>
        /// Available quantity for display
        /// </summary>
        public int AvailableQuantity => AvailableStock;

        /// <summary>
        /// Get stock status
        /// </summary>
        public StockStatus Status
        {
            get
            {
                if (AvailableStock <= 0) return StockStatus.OutOfStock;
                if (AvailableStock <= MinimumStockLevel) return StockStatus.LowStock;
                return StockStatus.InStock;
            }
        }

        /// <summary>
        /// Get stock status percentage
        /// </summary>
        public double StockLevelPercentage => MinimumStockLevel > 0 
            ? Math.Min(100, (double)CurrentStock / MinimumStockLevel * 100) 
            : 100;

        /// <summary>
        /// Stock status as text for display
        /// </summary>
        public string StockStatusText => Status switch
        {
            StockStatus.InStock => "In Stock",
            StockStatus.LowStock => "Low Stock",
            StockStatus.OutOfStock => "Out of Stock",
            _ => "Unknown"
        };

        /// <summary>
        /// Check if item is in stock (compatibility property)
        /// </summary>
        public bool IsInStock => Status == StockStatus.InStock;

        /// <summary>
        /// Calculate days of stock remaining based on average usage
        /// </summary>
        public int DaysOfStockRemaining(double averageDailyUsage)
        {
            if (averageDailyUsage <= 0) return int.MaxValue;
            return (int)Math.Ceiling(AvailableStock / averageDailyUsage);
        }

        /// <summary>
        /// Update stock quantity
        /// </summary>
        public void UpdateStock(int newQuantity, string reason = "")
        {
            CurrentStock = Math.Max(0, newQuantity);
            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Add stock (incoming delivery)
        /// </summary>
        public void AddStock(int quantity, string reason = "Delivery")
        {
            CurrentStock += Math.Max(0, quantity);
            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Remove stock (sale/usage)
        /// </summary>
        public bool RemoveStock(int quantity, string reason = "Sale")
        {
            if (AvailableStock < quantity) return false;
            
            CurrentStock -= quantity;
            LastUpdated = DateTime.Now;
            return true;
        }

        /// <summary>
        /// Reserve stock for an order
        /// </summary>
        public bool ReserveStock(int quantity)
        {
            if (AvailableStock < quantity) return false;
            
            ReservedStock += quantity;
            LastUpdated = DateTime.Now;
            return true;
        }

        /// <summary>
        /// Release reserved stock
        /// </summary>
        public void ReleaseReservedStock(int quantity)
        {
            ReservedStock = Math.Max(0, ReservedStock - quantity);
            LastUpdated = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Part.Code} - Stock: {CurrentStock} ({Status})";
        }
    }

    /// <summary>
    /// Represents stock movement/transaction
    /// </summary>
    public class StockMovement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PartCode { get; set; } = string.Empty;
        public StockMovementType Type { get; set; }
        public int Quantity { get; set; }
        public int StockBefore { get; set; }
        public int StockAfter { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty; // Order number, delivery note, etc.
    }

    /// <summary>
    /// Types of stock movements
    /// </summary>
    public enum StockMovementType
    {
        Incoming,    // Delivery from supplier
        Outgoing,    // Sale to customer
        Adjustment,  // Manual adjustment
        Reserved,    // Reserved for order
        Released,    // Released from reservation
        Transfer,    // Transfer between locations
        Damaged,     // Damaged/defective items
        Returned     // Customer return
    }
}
