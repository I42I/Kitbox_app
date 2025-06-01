using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitboxAPI.Models
{
    [Table("supplier")]
    public class Supplier
    {
        [Key]
        [Column("id_supplier")]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("contact")]
        public string? Contact { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        // Navigation vers les SupplierOrders
        public ICollection<SupplierOrder> SupplierOrders { get; set; } = new List<SupplierOrder>();
    }
}
