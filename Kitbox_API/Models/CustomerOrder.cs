using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kitbox_API.Models
{
    [Table("customer_order")]
    public class CustomerOrder
    {
        [Key]
        [Column("id_order")]
        public int IdOrder { get; set; }

        [Column("order_date")]
        public DateTime? OrderDate { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("deposit_amount")]
        public decimal? DepositAmount { get; set; }

        [Column("tel")]
        public int? Tel { get; set; }

        [Column("mail")]
        public string? Mail { get; set; }
        
        public virtual ICollection<Cabinet> Cabinets { get; set; } = new List<Cabinet>();
    }
}