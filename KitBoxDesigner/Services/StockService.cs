using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Services
{
    /// <summary>
    /// Implementation of stock service for managing inventory operations
    /// </summary>
    public class StockService : IStockService
    {
        private readonly List<StockItem> _stockItems;
        private readonly List<StockMovement> _stockMovements;
        private readonly List<StockAlert> _stockAlerts;

        public StockService()
        {
            _stockItems = new List<StockItem>();
            _stockMovements = new List<StockMovement>();
            _stockAlerts = new List<StockAlert>();
            
            // Generate initial alerts
            GenerateStockAlerts();
        }

        /// <inheritdoc />
        public async Task<List<StockItem>> GetAllStockItemsAsync()
        {
            await Task.Delay(50);
            return _stockItems.ToList();
        }

        /// <inheritdoc />
        public async Task<StockItem?> GetStockItemByCodeAsync(string partCode)
        {
            await Task.Delay(10);
            return _stockItems.FirstOrDefault(s => s.Part.Code.Equals(partCode, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc />
        public async Task<List<StockItem>> GetLowStockItemsAsync()
        {
            await Task.Delay(30);
            return _stockItems.Where(s => s.Status == StockStatus.LowStock).ToList();
        }

        /// <inheritdoc />
        public async Task<List<StockItem>> GetOutOfStockItemsAsync()
        {
            await Task.Delay(30);
            return _stockItems.Where(s => s.Status == StockStatus.OutOfStock).ToList();
        }

        /// <inheritdoc />
        public async Task<List<StockItem>> GetItemsNeedingReorderAsync()
        {
            await Task.Delay(40);
            return _stockItems.Where(s => s.NeedsReorder).ToList();
        }

        /// <inheritdoc />
        public async Task<bool> UpdateStockAsync(string partCode, int newQuantity, string reason = "Manual Update")
        {
            await Task.Delay(50);
            
            var stockItem = await GetStockItemByCodeAsync(partCode);
            if (stockItem == null)
                return false;

            var oldQuantity = stockItem.CurrentStock;
            stockItem.UpdateStock(newQuantity, reason);

            // Record movement
            RecordStockMovement(partCode, StockMovementType.Adjustment, newQuantity - oldQuantity, oldQuantity, newQuantity, reason);
            
            // Update alerts
            UpdateAlertsForItem(stockItem);
            
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> AddStockAsync(string partCode, int quantity, string reason = "Delivery")
        {
            await Task.Delay(40);
            
            var stockItem = await GetStockItemByCodeAsync(partCode);
            if (stockItem == null || quantity <= 0)
                return false;

            var oldQuantity = stockItem.CurrentStock;
            stockItem.AddStock(quantity, reason);

            // Record movement
            RecordStockMovement(partCode, StockMovementType.Incoming, quantity, oldQuantity, stockItem.CurrentStock, reason);
            
            // Update alerts
            UpdateAlertsForItem(stockItem);
            
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> RemoveStockAsync(string partCode, int quantity, string reason = "Sale")
        {
            await Task.Delay(40);
            
            var stockItem = await GetStockItemByCodeAsync(partCode);
            if (stockItem == null || quantity <= 0)
                return false;

            var oldQuantity = stockItem.CurrentStock;
            var success = stockItem.RemoveStock(quantity, reason);
            
            if (success)
            {
                // Record movement
                RecordStockMovement(partCode, StockMovementType.Outgoing, -quantity, oldQuantity, stockItem.CurrentStock, reason);
                
                // Update alerts
                UpdateAlertsForItem(stockItem);
            }
            
            return success;
        }

        /// <inheritdoc />
        public async Task<bool> ReserveStockAsync(string partCode, int quantity)
        {
            await Task.Delay(30);
            
            var stockItem = await GetStockItemByCodeAsync(partCode);
            if (stockItem == null)
                return false;

            var success = stockItem.ReserveStock(quantity);
            
            if (success)
            {
                RecordStockMovement(partCode, StockMovementType.Reserved, -quantity, 
                    stockItem.CurrentStock, stockItem.CurrentStock, "Reserved for order");
                UpdateAlertsForItem(stockItem);
            }
            
            return success;
        }

        /// <inheritdoc />
        public async Task<bool> ReleaseReservedStockAsync(string partCode, int quantity)
        {
            await Task.Delay(30);
            
            var stockItem = await GetStockItemByCodeAsync(partCode);
            if (stockItem == null)
                return false;

            stockItem.ReleaseReservedStock(quantity);
            
            RecordStockMovement(partCode, StockMovementType.Released, quantity, 
                stockItem.CurrentStock, stockItem.CurrentStock, "Released from reservation");
            UpdateAlertsForItem(stockItem);
            
            return true;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, StockStatus>> CheckStockAvailabilityAsync(List<PartRequirement> requirements)
        {
            await Task.Delay(80);
            
            var availability = new Dictionary<string, StockStatus>();
            
            foreach (var requirement in requirements)
            {
                var stockItem = await GetStockItemByCodeAsync(requirement.PartCode);
                if (stockItem == null)
                {
                    availability[requirement.PartCode] = StockStatus.OutOfStock;
                    continue;
                }

                if (stockItem.AvailableStock >= requirement.Quantity)
                    availability[requirement.PartCode] = StockStatus.InStock;
                else if (stockItem.AvailableStock > 0)
                    availability[requirement.PartCode] = StockStatus.LowStock;
                else
                    availability[requirement.PartCode] = StockStatus.OutOfStock;
            }

            return availability;
        }

        /// <inheritdoc />
        public async Task<List<StockMovement>> GetStockMovementsAsync(string partCode, int daysBack = 30)
        {
            await Task.Delay(40);
            
            var cutoffDate = DateTime.Now.AddDays(-daysBack);
            return _stockMovements
                .Where(m => m.PartCode.Equals(partCode, StringComparison.OrdinalIgnoreCase) && m.Timestamp >= cutoffDate)
                .OrderByDescending(m => m.Timestamp)
                .ToList();
        }

        /// <inheritdoc />
        public async Task<List<StockAlert>> GetStockAlertsAsync()
        {
            await Task.Delay(20);
            return _stockAlerts.Where(a => !a.IsAcknowledged).OrderByDescending(a => a.Timestamp).ToList();
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, DateTime>> GetEstimatedDeliveryDatesAsync(List<PartRequirement> requirements)
        {
            await Task.Delay(60);
            
            var deliveryDates = new Dictionary<string, DateTime>();
            
            foreach (var requirement in requirements)
            {
                var stockItem = await GetStockItemByCodeAsync(requirement.PartCode);
                if (stockItem == null)
                {
                    deliveryDates[requirement.PartCode] = DateTime.Now.AddDays(30); // Default long delay
                    continue;
                }

                if (stockItem.AvailableStock >= requirement.Quantity)
                {
                    deliveryDates[requirement.PartCode] = DateTime.Now.AddDays(1); // Available immediately
                }
                else
                {
                    // Calculate based on delivery delay and expected restock
                    var deliveryDelay = stockItem.Part.DeliveryDelay;
                    var estimatedDate = stockItem.ExpectedRestockDate ?? DateTime.Now.AddDays(deliveryDelay);
                    deliveryDates[requirement.PartCode] = estimatedDate;
                }
            }

            return deliveryDates;
        }

        /// <summary>
        /// Record a stock movement
        /// </summary>
        private void RecordStockMovement(string partCode, StockMovementType type, int quantity, int stockBefore, int stockAfter, string reason)
        {
            _stockMovements.Add(new StockMovement
            {
                PartCode = partCode,
                Type = type,
                Quantity = quantity,
                StockBefore = stockBefore,
                StockAfter = stockAfter,
                Reason = reason,
                UserId = "System" // In real app would be current user
            });
        }

        /// <summary>
        /// Update alerts for a specific stock item
        /// </summary>
        private void UpdateAlertsForItem(StockItem stockItem)
        {
            // Remove old alerts for this part
            _stockAlerts.RemoveAll(a => a.PartCode == stockItem.Part.Code);

            // Add new alerts if needed
            if (stockItem.Status == StockStatus.OutOfStock)
            {
                _stockAlerts.Add(new StockAlert
                {
                    PartCode = stockItem.Part.Code,
                    Message = $"Part {stockItem.Part.Code} is out of stock",
                    Type = StockAlertType.OutOfStock
                });
            }
            else if (stockItem.Status == StockStatus.LowStock)
            {
                _stockAlerts.Add(new StockAlert
                {
                    PartCode = stockItem.Part.Code,
                    Message = $"Part {stockItem.Part.Code} has low stock ({stockItem.AvailableStock} remaining)",
                    Type = StockAlertType.LowStock
                });
            }

            if (stockItem.NeedsReorder)
            {
                _stockAlerts.Add(new StockAlert
                {
                    PartCode = stockItem.Part.Code,
                    Message = $"Part {stockItem.Part.Code} needs reordering",
                    Type = StockAlertType.ReorderRequired
                });
            }
        }

        /// <summary>
        /// Generate initial stock alerts
        /// </summary>
        private void GenerateStockAlerts()
        {
            foreach (var stockItem in _stockItems)
            {
                UpdateAlertsForItem(stockItem);
            }
        }

        /// <summary>
        /// Acknowledge an alert
        /// </summary>
        public async Task<bool> AcknowledgeAlertAsync(Guid alertId)
        {
            await Task.Delay(10);
            
            var alert = _stockAlerts.FirstOrDefault(a => a.Id == alertId);
            if (alert != null)
            {
                alert.IsAcknowledged = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get stock summary statistics
        /// </summary>
        public async Task<StockSummary> GetStockSummaryAsync()
        {
            await Task.Delay(50);
            
            return new StockSummary
            {
                TotalParts = _stockItems.Count,
                InStockParts = _stockItems.Count(s => s.Status == StockStatus.InStock),
                LowStockParts = _stockItems.Count(s => s.Status == StockStatus.LowStock),
                OutOfStockParts = _stockItems.Count(s => s.Status == StockStatus.OutOfStock),
                PartsNeedingReorder = _stockItems.Count(s => s.NeedsReorder),
                TotalValue = _stockItems.Sum(s => s.CurrentStock * s.Part.Price),                ActiveAlerts = _stockAlerts.Count(a => !a.IsAcknowledged)
            };
        }

        /// <summary>
        /// Get specific stock item by part code
        /// </summary>
        public async Task<StockItem?> GetStockItemAsync(string partCode)
        {
            return await GetStockItemByCodeAsync(partCode);
        }

        /// <summary>
        /// Check availability for a specific part and quantity
        /// </summary>
        public async Task<bool> CheckAvailabilityAsync(string partCode, int requiredQuantity)
        {
            await Task.Delay(20);
            
            var stockItem = await GetStockItemByCodeAsync(partCode);
            if (stockItem == null)
                return false;
                
            return stockItem.AvailableStock >= requiredQuantity;
        }
    }

    /// <summary>
    /// Stock summary statistics
    /// </summary>
    public class StockSummary
    {
        public int TotalParts { get; set; }
        public int InStockParts { get; set; }
        public int LowStockParts { get; set; }
        public int OutOfStockParts { get; set; }
        public int PartsNeedingReorder { get; set; }
        public decimal TotalValue { get; set; }
        public int ActiveAlerts { get; set; }
        
        public double InStockPercentage => TotalParts > 0 ? (double)InStockParts / TotalParts * 100 : 0;
        public double LowStockPercentage => TotalParts > 0 ? (double)LowStockParts / TotalParts * 100 : 0;
        public double OutOfStockPercentage => TotalParts > 0 ? (double)OutOfStockParts / TotalParts * 100 : 0;
    }
}
