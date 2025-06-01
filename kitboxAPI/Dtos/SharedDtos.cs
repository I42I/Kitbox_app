using KitboxAPI.Models; // pour utiliser StockStatus et OrderStatus

namespace KitboxAPI.Dtos
{
    public class CustomerOrderFullDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; } // ✅ enum utilisé ici
        public decimal DepositAmount { get; set; }
        public string? Tel { get; set; }
        public string? Mail { get; set; }
        public List<CabinetDto> Cabinets { get; set; } = new();
    }

    public class CustomerOrderCreateDto
    {
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; } // ✅ enum utilisé ici
        public decimal DepositAmount { get; set; }
        public string? Tel { get; set; }
        public string? Mail { get; set; }
    }

    public class CabinetDto
    {
        public int Id { get; set; }
        public int IdOrder { get; set; }
        public decimal Price { get; set; }
        public string? Dimensions { get; set; }
        public string? Reference { get; set; }
        public List<LockerDto> Lockers { get; set; } = new();
    }

    public class CabinetCreateDto
    {
        public int IdOrder { get; set; }
        public decimal Price { get; set; }
        public string? Dimensions { get; set; }
        public string? Reference { get; set; }
    }

    public class LockerDto
    {
        public int Id { get; set; }
        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Dimensions { get; set; }
        public List<LockerStockDto> LockerStocks { get; set; } = new();
    }

    public class LockerCreateDto
    {
        public int IdCabinet { get; set; }
        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Dimensions { get; set; }
    }

    public class LockerStockDto
    {
        public int Id { get; set; }
        public int IdStock { get; set; }
        public int QuantityNeeded { get; set; }
        public StockDto Stock { get; set; } = null!;
    }

    public class LockerStockCreateDto
    {
        public int IdLocker { get; set; }
        public int IdStock { get; set; }
        public int QuantityNeeded { get; set; }
    }

    public class StockDto
    {
        public int Id { get; set; }
        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Dimensions { get; set; }
        public int Quantity { get; set; }
        public StockStatus Status { get; set; } // ✅ enum pour Stock
        public string? Location { get; set; }
        public int IdSupplierOrder { get; set; }
        public SupplierDto Supplier { get; set; } = null!;
    }

    public class StockCreateDto
    {
        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Dimensions { get; set; }
        public int Quantity { get; set; }
        public StockStatus Status { get; set; } // ✅ enum pour Stock
        public string? Location { get; set; }
        public int IdSupplierOrder { get; set; }
    }

    public class SupplierDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Contact { get; set; }
        public string? Address { get; set; }
    }

    public class SupplierOrderDto
    {
        public int Id { get; set; }
        public int IdSupplier { get; set; }
        public DateTime OrderDate { get; set; }
        public SupplierDto Supplier { get; set; } = null!;
    }
}
