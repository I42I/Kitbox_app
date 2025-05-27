using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitboxAPI.Models
{
    [Table("locker")]
    public class Locker
    {
        [Key]
        [Column("id_locker")]
        public int Id { get; set; }

        [Column("id_cabinet")]
        public int IdCabinet { get; set; }

        [Column("reference")]
        public string? Reference { get; set; }

        [Column("code")]
        public string? Code { get; set; }

        [Column("dimensions")]
        public string? Dimensions { get; set; }

        // Navigation vers Cabinet
        public Cabinet Cabinet { get; set; } = null!;

        // Navigation vers LockerStock
        public ICollection<LockerStock> LockerStocks { get; set; } = new List<LockerStock>();
    }
}
