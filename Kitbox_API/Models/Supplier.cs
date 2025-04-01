using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kitbox_API.Models
{
    [Table("supplier")]
    public class Supplier
    {
        [Key]
        [Column("id_supplier")]
        public int IdSupplier { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("contact")]
        public string? Contact { get; set; }

        [Column("address")]
        public string? Address { get; set; }
        
        public virtual ICollection<SupplierOrder> SupplierOrders { get; set; } = new List<SupplierOrder>();
    }
}