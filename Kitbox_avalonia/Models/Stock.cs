using System;
using System.Collections.Generic;


public class Stock
{
    public int Id { get; set; }  // id_stock
    public string Reference { get; set; }  
    public string Code { get; set; }  
    public string Dimensions { get; set; }  
    public decimal PriceSupplier1 { get; set; }  
    public string DelaySupplier1 { get; set; }  
    public decimal PriceSupplier2 { get; set; }  
    public string DelaySupplier2 { get; set; }  
    public int IdSupplierOrder { get; set; }  // id_supplier_order
    public int Quantity { get; set; }  
    public string Status { get; set; }  
    public string Location { get; set; }  

    // Relation avec SupplierOrder
    public SupplierOrder SupplierOrder { get; set; }

    // Relation avec LockerStock
    public ICollection<LockerStock> LockerStocks { get; set; }
}
