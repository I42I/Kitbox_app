using Microsoft.EntityFrameworkCore;

namespace Kitbox_API.Models
{
    public class KitboxContext : DbContext
    {
        public KitboxContext(DbContextOptions<KitboxContext> options) : base(options) { }

        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<Cabinet> Cabinets { get; set; }
        public DbSet<Locker> Lockers { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<LockerStock> LockerStocks { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierOrder> SupplierOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure les relations et les contraintes si nécessaire
            modelBuilder.Entity<Cabinet>()
                .HasOne(c => c.Order)
                .WithMany(o => o.Cabinets)
                .HasForeignKey(c => c.IdOrder)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Locker>()
                .HasOne(l => l.Cabinet)
                .WithMany(c => c.Lockers)
                .HasForeignKey(l => l.IdCabinet)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.SupplierOrder)
                .WithMany(so => so.Stocks)
                .HasForeignKey(s => s.IdSupplierOrder)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SupplierOrder>()
                .HasOne(so => so.Supplier)
                .WithMany(s => s.SupplierOrders)
                .HasForeignKey(so => so.IdSupplier)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<LockerStock>()
                .HasOne(ls => ls.Locker)
                .WithMany(l => l.LockerStocks)
                .HasForeignKey(ls => ls.IdLocker)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<LockerStock>()
                .HasOne(ls => ls.Stock)
                .WithMany(s => s.LockerStocks)
                .HasForeignKey(ls => ls.IdStock)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}