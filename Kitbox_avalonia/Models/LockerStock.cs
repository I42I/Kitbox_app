public class LockerStock
{
    public int Id { get; set; }  // id_locker_stock
    public int IdLocker { get; set; }  // id_locker
    public int IdStock { get; set; }  // id_stock
    public int QuantityNeeded { get; set; }  

    // Relations
    public Locker Locker { get; set; }
    public Stock Stock { get; set; }
}

