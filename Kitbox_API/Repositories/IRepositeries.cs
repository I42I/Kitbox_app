using System.Collections.Generic;
using System.Threading.Tasks;
using Kitbox_API.Models;

namespace Kitbox_API.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }

    public interface ICabinetRepository : IGenericRepository<Cabinet>
    {
        Task<IEnumerable<Cabinet>> GetCabinetsByOrderIdAsync(int orderId);
    }

    public interface ICustomerOrderRepository : IGenericRepository<CustomerOrder>
    {
        Task<IEnumerable<CustomerOrder>> GetOrdersByStatusAsync(string status);
    }

    public interface ILockerRepository : IGenericRepository<Locker>
    {
        Task<IEnumerable<Locker>> GetLockersByCabinetIdAsync(int cabinetId);
    }

    public interface ILockerStockRepository : IGenericRepository<LockerStock>
    {
        Task<IEnumerable<LockerStock>> GetLockerStocksByLockerIdAsync(int lockerId);
        Task<IEnumerable<LockerStock>> GetLockerStocksByStockIdAsync(int stockId);
    }

    public interface IStockRepository : IGenericRepository<Stock>
    {
        Task<IEnumerable<Stock>> GetStocksByStatusAsync(string status);
        Task<IEnumerable<Stock>> GetStocksBySupplierOrderIdAsync(int supplierOrderId);
    }

    public interface ISupplierRepository : IGenericRepository<Supplier>
    {
        Task<Supplier> GetSupplierByNameAsync(string name);
    }

    public interface ISupplierOrderRepository : IGenericRepository<SupplierOrder>
    {
        Task<IEnumerable<SupplierOrder>> GetOrdersBySupplierIdAsync(int supplierId);
    }
}