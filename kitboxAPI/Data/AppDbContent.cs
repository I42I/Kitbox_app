using Microsoft.EntityFrameworkCore;
using KitboxAPI.Models;

namespace KitboxAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<Cabinet> Cabinets { get; set; }
        public DbSet<Locker> Lockers { get; set; }
        public DbSet<LockerStock> LockerStocks { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<SupplierOrder> SupplierOrders { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ✅ Enum StockStatus (Stock.Status) => string
            modelBuilder.Entity<Stock>()
                .Property(s => s.Status)
                .HasConversion<string>();

            // ✅ Enum OrderStatus (CustomerOrder.Status) => string
            modelBuilder.Entity<CustomerOrder>()
                .Property(co => co.Status)
                .HasConversion<string>();
            
            // 🛠 Mappages supplémentaires
            modelBuilder.Entity<Stock>()
                .Property(s => s.Status)
                .HasConversion<string>();

            // 📦 Relations
            modelBuilder.Entity<Cabinet>()
                .HasOne(c => c.CustomerOrder)
                .WithMany(co => co.Cabinets)
                .HasForeignKey(c => c.IdOrder)
                .HasConstraintName("FK_Cabinet_CustomerOrder");

            modelBuilder.Entity<Locker>()
                .HasOne(l => l.Cabinet)
                .WithMany(c => c.Lockers)
                .HasForeignKey(l => l.IdCabinet)
                .HasConstraintName("FK_Locker_Cabinet");

            modelBuilder.Entity<LockerStock>()
                .HasOne(ls => ls.Locker)
                .WithMany(l => l.LockerStocks)
                .HasForeignKey(ls => ls.IdLocker)
                .HasConstraintName("FK_LockerStock_Locker");

            modelBuilder.Entity<LockerStock>()
                .HasOne(ls => ls.Stock)
                .WithMany(s => s.LockerStocks)
                .HasForeignKey(ls => ls.IdStock)
                .HasConstraintName("FK_LockerStock_Stock");

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.SupplierOrder)
                .WithMany(so => so.Stocks)
                .HasForeignKey(s => s.IdSupplierOrder)
                .HasConstraintName("FK_Stock_SupplierOrder");

            modelBuilder.Entity<SupplierOrder>()
                .HasOne(so => so.Supplier)
                .WithMany(s => s.SupplierOrders)
                .HasForeignKey(so => so.IdSupplier)
                .HasConstraintName("FK_SupplierOrder_Supplier");

            base.OnModelCreating(modelBuilder);
        }
    }
}

