using AutoMapper;
using Kitbox_API.DTOs;
using Kitbox_API.Models;

namespace Kitbox_API.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Cabinet mappings
            CreateMap<Cabinet, CabinetDto>();
            CreateMap<CabinetCreateDto, Cabinet>();
            CreateMap<CabinetUpdateDto, Cabinet>();
            
            // CustomerOrder mappings
            CreateMap<CustomerOrder, CustomerOrderDto>();
            CreateMap<CustomerOrderCreateDto, CustomerOrder>();
            CreateMap<CustomerOrderUpdateDto, CustomerOrder>();
            
            // Locker mappings
            CreateMap<Locker, LockerDto>();
            CreateMap<LockerCreateDto, Locker>();
            CreateMap<LockerUpdateDto, Locker>();
            
            // LockerStock mappings
            CreateMap<LockerStock, LockerStockDto>();
            CreateMap<LockerStockCreateDto, LockerStock>();
            CreateMap<LockerStockUpdateDto, LockerStock>();
            
            // Stock mappings
            CreateMap<Stock, StockDto>();
            CreateMap<StockCreateDto, Stock>();
            CreateMap<StockUpdateDto, Stock>();
            
            // Supplier mappings
            CreateMap<Supplier, SupplierDto>();
            CreateMap<SupplierCreateDto, Supplier>();
            CreateMap<SupplierUpdateDto, Supplier>();
            
            // SupplierOrder mappings
            CreateMap<SupplierOrder, SupplierOrderDto>();
            CreateMap<SupplierOrderCreateDto, SupplierOrder>();
            CreateMap<SupplierOrderUpdateDto, SupplierOrder>();
        }
    }
}