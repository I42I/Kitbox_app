using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kitbox_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Kitbox_API.Repositories
{
    public class CustomerOrderRepository : GenericRepository<CustomerOrder>, ICustomerOrderRepository
    {
        public CustomerOrderRepository(KitboxContext context) : base(context) { }

        public override async Task<IEnumerable<CustomerOrder>> GetAllAsync()
        {
            return await _context.CustomerOrders
                .Include(o => o.Cabinets)
                .ToListAsync();
        }

        public override async Task<CustomerOrder> GetByIdAsync(int id)
        {
            return await _context.CustomerOrders
                .Include(o => o.Cabinets)
                .FirstOrDefaultAsync(o => o.IdOrder == id);
        }

        public async Task<IEnumerable<CustomerOrder>> GetOrdersByStatusAsync(string status)
        {
            return await _context.CustomerOrders
                .Include(o => o.Cabinets)
                .Where(o => o.Status == status)
                .ToListAsync();
        }
    }
}