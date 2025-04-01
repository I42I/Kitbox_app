using System;
using System.Collections.Generic;

namespace Kitbox_API.DTOs
{
    // DTOs pour CustomerOrder
    public class CustomerOrderDto
    {
        public int IdOrder { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Status { get; set; }
        public decimal? DepositAmount { get; set; }
        public int? Tel { get; set; }
        public string? Mail { get; set; }
        public List<CabinetDto>? Cabinets { get; set; }
    }

    public class CreateCustomerOrderDto
    {
        public DateTime? OrderDate { get; set; }
        public string? Status { get; set; }
        public decimal? DepositAmount { get; set; }
        public int? Tel { get; set; }
        public string? Mail { get; set; }
    }

    // DTOs pour Cabinet
    public class CabinetDto
    {
        public int IdCabinet { get; set; }
        public int? IdOrder { get; set; }
        public decimal? Price { get; set; }
        public string? Dimensions { get; set; }
        public string? Reference { get; set; }
        public List<LockerDto>? Lockers { get; set; }
    }

    public class CreateCabinetDto
    {
        public int? IdOrder { get; set; }
        public decimal? Price { get; set; }
        public string? Dimensions { get; set; }
        public string? Reference { get; set; }
    }

    // DTOs pour Locker
    public class LockerDto
    {
        public int IdLocker { get; set; }
        public int? IdCabinet { get; set; }
        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Dimensions { get; set; }
        public List<LockerStockDto>? LockerStocks { get; set; }
    }

    public class CreateLockerDto
    {
        public int? IdCabinet { get; set; }
        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Dimensions { get; set; }
    }

    // DTOs pour LockerStock
    public class LockerStockDto
    {
        public int IdLockerStock { get; set; }
        public int? IdLocker { get; set; }
        public int? IdStock { get; set; }
        public int? QuantityNeeded { get; set; }
        public StockDto? Stock { get; set; }
    }

    public class CreateLockerStockDto
    {
        public int? IdLocker { get; set; }
        public int? IdStock { get; set; }
        public int? QuantityNeeded { get; set; }
    }

    // DTOs pour Stock
    public class StockDto
    {
        public int IdStock { get; set; }
        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Dimensions { get; set; }
        public decimal? PriceSupplier1 { get; set; }
        public string? DelaySupplier1 { get; set; }
        public decimal? PriceSupplier2 { get; set; }
        public string? DelaySupplier2 { get; set; }
        public int? IdSupplierOrder { get; set; }
        public int? Quantity { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
    }

    public class CreateStockDto
    {
        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Dimensions { get; set; }
        public decimal? PriceSupplier1 { get; set; }
        public string? DelaySupplier1 { get; set; }
        public decimal? PriceSupplier2 { get; set; }
        public string? DelaySupplier2 { get; set; }
        public int? IdSupplierOrder { get; set; }
        public int? Quantity { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
    }

    // DTOs pour Supplier
    public class SupplierDto
    {
        public int IdSupplier { get; set; }
        public string? Name { get; set; }
        public string? Contact { get; set; }
        public string? Address { get; set; }
    }

    public class CreateSupplierDto
    {
        public string? Name { get; set; }
        public string? Contact { get; set; }
        public string? Address { get; set; }
    }

    // DTOs pour SupplierOrder
    public class SupplierOrderDto
    {
        public int IdSupplierOrder { get; set; }
        public int? IdSupplier { get; set; }
        public DateTime? OrderDate { get; set; }
        public SupplierDto? Supplier { get; set; }
        public List<StockDto>? Stocks { get; set; }
    }

    public class CreateSupplierOrderDto
    {
        public int? IdSupplier { get; set; }
        public DateTime? OrderDate { get; set; }
    }

    // DTO pour l'authentification
    public class AuthRequestDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}