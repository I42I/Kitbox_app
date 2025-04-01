using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kitbox_API.Models
{
    [Table("cabinet")]
    public class Cabinet
    {
        [Key]
        [Column("id_cabinet")]
        public int IdCabinet { get; set; }

        [Column("id_order")]
        public int? IdOrder { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("dimensions")]
        public string? Dimensions { get; set; }

        [Column("reference")]
        public string? Reference { get; set; }

        [ForeignKey("IdOrder")]
        public virtual CustomerOrder? Order { get; set; }
        
        public virtual ICollection<Locker> Lockers { get; set; } = new List<Locker>();
    }
}