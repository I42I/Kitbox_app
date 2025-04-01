using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kitbox_API.Models
{
    [Table("supplier_order")]
    public class SupplierOrder
    {
        [Key]
        [Column("id_supplier_order")]
        public int IdSupplierOrder { get; set; }

        [Column("id_supplier")]
        public int? IdSupplier { get; set; }

        [Column("order_date")]
        public DateTime? OrderDate { get; set; }

        [ForeignKey("IdSupplier")]
        public virtual Supplier? Supplier { get; set; }
        
        public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}