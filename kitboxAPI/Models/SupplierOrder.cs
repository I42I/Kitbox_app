using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitboxAPI.Models
{
    [Table("supplier_order")]
    public class SupplierOrder
    {
        [Key]
        [Column("id_supplier_order")]
        public int Id { get; set; }

        [Column("id_supplier")]
        public int IdSupplier { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        // Navigation vers Supplier
        public Supplier Supplier { get; set; } = null!;

        // Navigation vers les Stocks
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}
