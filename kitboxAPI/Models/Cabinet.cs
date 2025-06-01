using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitboxAPI.Models
{
    [Table("cabinet")]
    public class Cabinet
    {
        [Key]
        [Column("id_cabinet")]
        public int Id { get; set; }

        [Column("id_order")]
        public int IdOrder { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("dimensions")]
        public string? Dimensions { get; set; }

        [Column("reference")]
        public string? Reference { get; set; }

        // Navigation vers CustomerOrder
        public CustomerOrder CustomerOrder { get; set; } = null!;

        // Navigation vers Lockers
        public ICollection<Locker> Lockers { get; set; } = new List<Locker>();
    }
}
