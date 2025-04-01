using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kitbox_API.Models
{
    [Table("locker_stock")]
    public class LockerStock
    {
        [Key]
        [Column("id_locker_stock")]
        public int IdLockerStock { get; set; }

        [Column("id_locker")]
        public int? IdLocker { get; set; }

        [Column("id_stock")]
        public int? IdStock { get; set; }

        [Column("quantity_needed")]
        public int? QuantityNeeded { get; set; }

        [ForeignKey("IdLocker")]
        public virtual Locker? Locker { get; set; }

        [ForeignKey("IdStock")]
        public virtual Stock? Stock { get; set; }
    }
}