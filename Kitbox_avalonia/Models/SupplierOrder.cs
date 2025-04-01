using System;
using System.Collections.Generic;


public class SupplierOrder
{
    public int Id { get; set; }  // id_supplier_order
    public int IdSupplier { get; set; }  // id_supplier
    public DateTime OrderDate { get; set; }  

    // Relation avec Supplier
    public Supplier Supplier { get; set; }

    // Relation avec Stock
    public ICollection<Stock> Stocks { get; set; }
}

