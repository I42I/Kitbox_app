public class Supplier
{
    public int Id { get; set; }  // id_supplier
    public string Name { get; set; }  
    public string Contact { get; set; }  
    public string Address { get; set; }  

    // Relation avec SupplierOrder
    public ICollection<SupplierOrder> SupplierOrders { get; set; }
}


