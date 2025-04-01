using System;
using System.Collections.Generic;


public class CustomerOrder
{
    public int Id { get; set; }  // id_order
    public DateTime OrderDate { get; set; }  
    public string Status { get; set; }  
    public decimal DepositAmount { get; set; }  
    public string Tel { get; set; }  
    public string Mail { get; set; }  

    // Relation avec Cabinet
    public ICollection<Cabinet> Cabinets { get; set; }
}

