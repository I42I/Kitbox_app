using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kitbox_API.Models
{
    [Table("locker")]
    public class Locker
    {
        [Key]
        [Column("id_locker")]
        public int IdLocker { get; set; }

        [Column("id_cabinet")]
        public int? IdCabinet { get; set; }

        [Column("reference")]
        public string? Reference { get; set; }

        [Column("code")]
        public string? Code { get; set; }

        [Column("dimensions")]
        public string? Dimensions { get; set; }

        [ForeignKey("IdCabinet")]
        public virtual Cabinet? Cabinet { get; set; }
        
        public virtual ICollection<LockerStock> LockerStocks { get; set; } = new List<LockerStock>();
    }
}