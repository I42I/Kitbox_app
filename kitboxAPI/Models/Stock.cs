using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitboxAPI.Models
{
    public enum StockStatus
    {
        AVAILABLE,//disponible
        DELIVERY,//en_livraison,
        OUT_OF_STOCK //rupture
    }

    [Table("stock")]
    public class Stock
    {
        [Key]
        [Column("id_stock")]
        public int Id { get; set; }

        [Column("reference")]
        public string? Reference { get; set; }

        [Column("code")]
        public string? Code { get; set; }

        [Column("dimensions")]
        public string? Dimensions { get; set; }

        [Column("price_supplier1")]
        public decimal PriceSupplier1 { get; set; }

        [Column("delay_supplier1")]
        public string? DelaySupplier1 { get; set; }

        [Column("price_supplier2")]
        public decimal PriceSupplier2 { get; set; }

        [Column("delay_supplier2")]
        public string? DelaySupplier2 { get; set; }

        [Column("id_supplier_order")]
        public int IdSupplierOrder { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("status")]
        [EnumDataType(typeof(StockStatus))]
        public StockStatus Status { get; set; }  // ✅ Changement ici

        [Column("location")]
        public string? Location { get; set; }

        // Navigation vers SupplierOrder
        public SupplierOrder SupplierOrder { get; set; } = null!;

        // Navigation vers LockerStocks
        public ICollection<LockerStock> LockerStocks { get; set; } = new List<LockerStock>();
    }
}

