using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KitboxAPI.Data;
using KitboxAPI.Models;
using KitboxAPI.Dtos;

namespace KitboxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LockerStocksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LockerStocksController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET all locker stocks (DTO)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LockerStockDto>>> GetAll()
        {
            var stocks = await _context.LockerStocks
                .Include(ls => ls.Stock)
                    .ThenInclude(s => s.SupplierOrder)
                        .ThenInclude(so => so.Supplier)
                .Select(ls => new LockerStockDto
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
                })
                .ToListAsync();

            return Ok(stocks);
        }

        // ✅ GET by ID (DTO)
        [HttpGet("{id}")]
        public async Task<ActionResult<LockerStockDto>> GetById(int id)
        {
            var ls = await _context.LockerStocks
                .Include(ls => ls.Stock)
                    .ThenInclude(s => s.SupplierOrder)
                        .ThenInclude(so => so.Supplier)
                .Where(ls => ls.Id == id)
                .Select(ls => new LockerStockDto
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
                })
                .FirstOrDefaultAsync();

            if (ls == null) return NotFound();
            return Ok(ls);
        }

        // ✅ POST create (with DTO)
        [HttpPost]
        public async Task<ActionResult<LockerStock>> Create(LockerStockCreateDto dto)
        {
            var lockerStock = new LockerStock
            {
                IdLocker = dto.IdLocker,
                IdStock = dto.IdStock,
                QuantityNeeded = dto.QuantityNeeded
            };

            _context.LockerStocks.Add(lockerStock);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ LockerStock ajouté : ID = {lockerStock.Id}");
            return CreatedAtAction(nameof(GetById), new { id = lockerStock.Id }, lockerStock);
        }

        // ✅ PUT update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, LockerStock item)
        {
            if (id != item.Id) return BadRequest();
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"✏️ LockerStock modifié : ID = {id}");
            return NoContent();
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.LockerStocks.FindAsync(id);
            if (item == null) return NotFound();
            _context.LockerStocks.Remove(item);
            await _context.SaveChangesAsync();
            Console.WriteLine($"🗑️ LockerStock supprimé : ID = {id}");
            return NoContent();
        }
    }
}

