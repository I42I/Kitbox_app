using System.Collections.Generic;
using System.Threading.Tasks;
using Kitbox_API.DTOs;

namespace Kitbox_API.Services
{
    public interface ICabinetService
    {
        Task<IEnumerable<CabinetDto>> GetAllCabinetsAsync();
        Task<CabinetDto> GetCabinetByIdAsync(int id);
        Task<CabinetDto> CreateCabinetAsync(CabinetCreateDto cabinetDto);
        Task UpdateCabinetAsync(int id, CabinetUpdateDto cabinetDto);
        Task DeleteCabinetAsync(int id);
        Task<IEnumerable<CabinetDto>> GetCabinetsByOrderIdAsync(int orderId);
    }

    public interface ICustomerOrderService
    {
        Task<IEnumerable<CustomerOrderDto>> GetAllOrdersAsync();
        Task<CustomerOrderDto> GetOrderByIdAsync(int id);
        Task<CustomerOrderDto> CreateOrderAsync(CustomerOrderCreateDto orderDto);
        Task UpdateOrderAsync(int id, CustomerOrderUpdateDto orderDto);
        Task DeleteOrderAsync(int id);
        Task<IEnumerable<CustomerOrderDto>> GetOrdersByStatusAsync(string status);
    }

    public interface ILockerService
    {
        Task<IEnumerable<LockerDto>> GetAllLockersAsync();
        Task<LockerDto> GetLockerByIdAsync(int id);
        Task<LockerDto> CreateLockerAsync(LockerCreateDto lockerDto);
        Task UpdateLockerAsync(int id, LockerUpdateDto lockerDto);
        Task DeleteLockerAsync(int id);
        Task<IEnumerable<LockerDto>> GetLockersByCabinetIdAsync(int cabinetId);
    }

    public interface ILockerStockService
    {
        Task<IEnumerable<LockerStockDto>> GetAllLockerStocksAsync();
        Task<LockerStockDto> GetLockerStockByIdAsync(int id);
        Task<LockerStockDto> CreateLockerStockAsync(LockerStockCreateDto lockerStockDto);
        Task UpdateLockerStockAsync(int id, LockerStockUpdateDto lockerStockDto);
        Task DeleteLockerStockAsync(int id);
        Task<IEnumerable<LockerStockDto>> GetLockerStocksByLockerIdAsync(int lockerId);
        Task<IEnumerable<LockerStockDto>> GetLockerStocksByStockIdAsync(int stockId);
    }

    public interface IStockService
    {
        Task<IEnumerable<StockDto>> GetAllStocksAsync();
        Task<StockDto> GetStockByIdAsync(int id);
        Task<StockDto> CreateStockAsync(StockCreateDto stockDto);
        Task UpdateStockAsync(int id, StockUpdateDto stockDto);
        Task DeleteStockAsync(int id);
        Task<IEnumerable<StockDto>> GetStocksByStatusAsync(string status);
        Task<IEnumerable<StockDto>> GetStocksBySupplierOrderIdAsync(int supplierOrderId);
    }

    public interface ISupplierService
    {
        Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
        Task<SupplierDto> GetSupplierByIdAsync(int id);
        Task<SupplierDto> CreateSupplierAsync(SupplierCreateDto supplierDto);
        Task UpdateSupplierAsync(int id, SupplierUpdateDto supplierDto);
        Task DeleteSupplierAsync(int id);
        Task<SupplierDto> GetSupplierByNameAsync(string name);
    }

    public interface ISupplierOrderService
    {
        Task<IEnumerable<SupplierOrderDto>> GetAllSupplierOrdersAsync();
        Task<SupplierOrderDto> GetSupplierOrderByIdAsync(int id);
        Task<SupplierOrderDto> CreateSupplierOrderAsync(SupplierOrderCreateDto supplierOrderDto);
        Task UpdateSupplierOrderAsync(int id, SupplierOrderUpdateDto supplierOrderDto);
        Task DeleteSupplierOrderAsync(int id);
        Task<IEnumerable<SupplierOrderDto>> GetOrdersBySupplierIdAsync(int supplierId);
    }
}