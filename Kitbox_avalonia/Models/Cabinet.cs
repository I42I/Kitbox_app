using System;
using System.Collections.Generic;


public class Cabinet
{
    public int Id { get; set; }  // id_cabinet
    public int IdOrder { get; set; }  // id_order
    public decimal Price { get; set; }  
    public string Dimensions { get; set; }  
    public string Reference { get; set; }  

    // Relation avec CustomerOrder
    public CustomerOrder CustomerOrder { get; set; }

    // Relation avec Locker
    public ICollection<Locker> Lockers { get; set; }
}
