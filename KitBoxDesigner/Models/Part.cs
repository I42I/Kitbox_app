using System;

namespace KitBoxDesigner.Models
{    /// <summary>
    /// Represents a part/component used in KitBox cabinet construction
    /// </summary>
    public class Part
    {
        /// <summary>
        /// Reference name of the part
        /// </summary>
        public string Reference { get; set; } = string.Empty;

        /// <summary>
        /// Display name of the part (same as Reference for compatibility)
        /// </summary>
        public string Name => Reference;

        /// <summary>
        /// Unique code identifier for the part
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Dimensions of the part with height, width, depth information
        /// </summary>
        public string Dimensions { get; set; } = string.Empty;

        /// <summary>
        /// Price per unit in EUR
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Delivery delay in days
        /// </summary>
        public int DeliveryDelay { get; set; }

        /// <summary>
        /// Current stock quantity (simulated)
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Minimum stock threshold for alerts
        /// </summary>
        public int MinimumStock { get; set; } = 10;

        /// <summary>
        /// Category of the part (Vertical Batten, Crossbar, Panel, etc.)
        /// </summary>
        public PartCategory Category { get; set; }

        /// <summary>
        /// Color/finish of the part
        /// </summary>
        public string Color { get; set; } = "Default";

        /// <summary>
        /// Check if part is in stock
        /// </summary>
        public bool IsInStock => StockQuantity > 0;

        /// <summary>
        /// Check if stock is low (below minimum threshold)
        /// </summary>
        public bool IsLowStock => StockQuantity <= MinimumStock && StockQuantity > 0;

        /// <summary>
        /// Check if part is out of stock
        /// </summary>
        public bool IsOutOfStock => StockQuantity <= 0;

        /// <summary>
        /// Get stock status as enum
        /// </summary>
        public StockStatus StockStatus
        {
            get
            {
                if (IsOutOfStock) return StockStatus.OutOfStock;
                if (IsLowStock) return StockStatus.LowStock;
                return StockStatus.InStock;
            }
        }

        /// <summary>
        /// Estimated delivery date based on current date and delivery delay
        /// </summary>
        public DateTime EstimatedDeliveryDate => DateTime.Now.AddDays(DeliveryDelay);

        /// <summary>
        /// Formatted price for display purposes
        /// </summary>
        public string FormattedPrice => $"€{Price:F2}";

        /// <summary>
        /// Display name combining reference and code
        /// </summary>
        public string DisplayName => $"{Reference} ({Code})";

        public override string ToString()
        {
            return $"{Code} - {Reference} ({Dimensions}) - €{Price:F2}";
        }
    }

    /// <summary>
    /// Categories of parts used in cabinet construction
    /// </summary>
    public enum PartCategory
    {
        VerticalBatten,
        CrossbarLeft,
        CrossbarRight,
        CrossbarFront,
        CrossbarBack,
        PanelHorizontal,
        PanelVertical,
        PanelBack,
        Door,
        AngleIron,
        Coupelles,
        Hardware
    }

    /// <summary>
    /// Stock status indicators
    /// </summary>
    public enum StockStatus
    {
        InStock,
        LowStock,
        OutOfStock
    }
}
