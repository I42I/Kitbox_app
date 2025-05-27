using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitboxAPI.Models
{
    [Table("locker_stock")]
    public class LockerStock
    {
        [Key]
        [Column("id_locker_stock")]
        public int Id { get; set; }

        [Column("id_locker")]
        public int IdLocker { get; set; }

        [Column("id_stock")]
        public int IdStock { get; set; }

        [Column("quantity_needed")]
        public int QuantityNeeded { get; set; }

        // Navigation vers Locker
        public Locker Locker { get; set; } = null!;

        // Navigation vers Stock
        public Stock Stock { get; set; } = null!;
    }
}
