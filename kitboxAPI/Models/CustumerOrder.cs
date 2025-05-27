using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitboxAPI.Models
{
    public enum OrderStatus
    {
        PURCHASED, 
        RESERVED,
        PAID
    }

    [Table("customer_order")]
    public class CustomerOrder
    {
        [Key]
        [Column("id_order")]
        public int Id { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Column("status")]
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus Status { get; set; }

        [Column("deposit_amount")]
        public decimal DepositAmount { get; set; }

        [Column("tel")]
        public string? Tel { get; set; }

        [Column("mail")]
        public string? Mail { get; set; }

        // Navigation vers les cabinets
        public ICollection<Cabinet> Cabinets { get; set; } = new List<Cabinet>();
    }
}

