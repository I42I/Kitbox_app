using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kitbox_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Kitbox_API.Repositories
{
    public class CabinetRepository : GenericRepository<Cabinet>, ICabinetRepository
    {
        public CabinetRepository(KitboxContext context) : base(context) { }

        public override async Task<IEnumerable<Cabinet>> GetAllAsync()
        {
            return await _context.Cabinets
                .Include(c => c.Order)
                .Include(c => c.Lockers)
                .ToListAsync();
        }

        public override async Task<Cabinet> GetByIdAsync(int id)
        {
            return await _context.Cabinets
                .Include(c => c.Order)
                .Include(c => c.Lockers)
                .FirstOrDefaultAsync(c => c.IdCabinet == id);
        }

        public async Task<IEnumerable<Cabinet>> GetCabinetsByOrderIdAsync(int orderId)
        {
            return await _context.Cabinets
                .Include(c => c.Lockers)
                .Where(c => c.IdOrder == orderId)
                .ToListAsync();
        }
    }
}