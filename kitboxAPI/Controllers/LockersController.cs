using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KitboxAPI.Data;
using KitboxAPI.Models;
using KitboxAPI.Dtos;

namespace KitboxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LockersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LockersController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET all lockers (DTO)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LockerDto>>> GetAll()
        {
            var lockers = await _context.Lockers
                .Include(l => l.LockerStocks)
                    .ThenInclude(ls => ls.Stock)
                        .ThenInclude(s => s.SupplierOrder)
                            .ThenInclude(so => so.Supplier)
                .Select(l => new LockerDto
                {
                    Id = l.Id,
                    Reference = l.Reference,
                    Code = l.Code,
                    Dimensions = l.Dimensions,
                    LockerStocks = l.LockerStocks.Select(ls => new LockerStockDto
                    {
                        Id = ls.Id,
                        QuantityNeeded = ls.QuantityNeeded,
                        Stock = new StockDto
                        {
                            Id = ls.Stock.Id,
                            Reference = ls.Stock.Reference,
                            Code = ls.Stock.Code,
                            Dimensions = ls.Stock.Dimensions,
                            Quantity = ls.Stock.Quantity,
                            Status = ls.Stock.Status,
                            Location = ls.Stock.Location,
                            Supplier = new SupplierDto
                            {
                                Id = ls.Stock.SupplierOrder.Supplier.Id,
                                Name = ls.Stock.SupplierOrder.Supplier.Name,
                                Contact = ls.Stock.SupplierOrder.Supplier.Contact,
                                Address = ls.Stock.SupplierOrder.Supplier.Address
                            }
                        }
                    }).ToList()
                })
                .ToListAsync();

            return Ok(lockers);
        }

        // ✅ GET by ID (DTO)
        [HttpGet("{id}")]
        public async Task<ActionResult<LockerDto>> GetById(int id)
        {
            var locker = await _context.Lockers
                .Include(l => l.LockerStocks)
                    .ThenInclude(ls => ls.Stock)
                        .ThenInclude(s => s.SupplierOrder)
                            .ThenInclude(so => so.Supplier)
                .Where(l => l.Id == id)
                .Select(l => new LockerDto
                {
                    Id = l.Id,
                    Reference = l.Reference,
                    Code = l.Code,
                    Dimensions = l.Dimensions,
                    LockerStocks = l.LockerStocks.Select(ls => new LockerStockDto
                    {
                        Id = ls.Id,
                        QuantityNeeded = ls.QuantityNeeded,
                        Stock = new StockDto
                        {
                            Id = ls.Stock.Id,
                            Reference = ls.Stock.Reference,
                            Code = ls.Stock.Code,
                            Dimensions = ls.Stock.Dimensions,
                            Quantity = ls.Stock.Quantity,
                            Status = ls.Stock.Status,
                            Location = ls.Stock.Location,
                            Supplier = new SupplierDto
                            {
                                Id = ls.Stock.SupplierOrder.Supplier.Id,
                                Name = ls.Stock.SupplierOrder.Supplier.Name,
                                Contact = ls.Stock.SupplierOrder.Supplier.Contact,
                                Address = ls.Stock.SupplierOrder.Supplier.Address
                            }
                        }
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (locker == null) return NotFound();
            return Ok(locker);
        }

        // ✅ POST create locker (with DTO)
        [HttpPost]
        public async Task<ActionResult<Locker>> Create(LockerCreateDto dto)
        {
            var locker = new Locker
            {
                IdCabinet = dto.IdCabinet,
                Reference = dto.Reference,
                Code = dto.Code,
                Dimensions = dto.Dimensions
            };

            _context.Lockers.Add(locker);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ Locker ajouté : ID = {locker.Id}");
            return CreatedAtAction(nameof(GetById), new { id = locker.Id }, locker);
        }

        // ✅ PUT update locker
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Locker item)
        {
            if (id != item.Id) return BadRequest();
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"✏️ Locker modifié : ID = {id}");
            return NoContent();
        }

        // ✅ DELETE locker
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Lockers.FindAsync(id);
            if (item == null) return NotFound();
            _context.Lockers.Remove(item);
            await _context.SaveChangesAsync();
            Console.WriteLine($"🗑️ Locker supprimé : ID = {id}");
            return NoContent();
        }
    }
}

