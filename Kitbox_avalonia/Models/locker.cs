using System;
using System.Collections.Generic;


public class Locker
{
    public int Id { get; set; }  // id_locker
    public int IdCabinet { get; set; }  // id_cabinet
    public string Reference { get; set; }  
    public string Code { get; set; }  
    public string Dimensions { get; set; }  

    // Relation avec Cabinet
    public Cabinet Cabinet { get; set; }

    // Relation avec LockerStock
    public ICollection<LockerStock> LockerStocks { get; set; }
}

