using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KitboxAPI.Data;
using KitboxAPI.Models;
using KitboxAPI.Dtos;

namespace KitboxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StocksController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET all stocks (DTO)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetAll()
        {
            var stocks = await _context.Stocks
                .Include(s => s.SupplierOrder)
                    .ThenInclude(so => so.Supplier)
                .Select(s => new StockDto
                {
                    Id = s.Id,
                    Reference = s.Reference,
                    Code = s.Code,
                    Dimensions = s.Dimensions,
                    Quantity = s.Quantity,
                    Status = s.Status,
                    Location = s.Location,
                    Supplier = new SupplierDto
                    {
                        Id = s.SupplierOrder.Supplier.Id,
                        Name = s.SupplierOrder.Supplier.Name,
                        Contact = s.SupplierOrder.Supplier.Contact,
                        Address = s.SupplierOrder.Supplier.Address
                    }
                })
                .ToListAsync();

            return Ok(stocks);
        }

        // ✅ GET by ID (DTO)
        [HttpGet("{id}")]
        public async Task<ActionResult<StockDto>> GetById(int id)
        {
            var stock = await _context.Stocks
                .Include(s => s.SupplierOrder)
                    .ThenInclude(so => so.Supplier)
                .Where(s => s.Id == id)
                .Select(s => new StockDto
                {
                    Id = s.Id,
                    Reference = s.Reference,
                    Code = s.Code,
                    Dimensions = s.Dimensions,
                    Quantity = s.Quantity,
                    Status = s.Status,
                    Location = s.Location,
                    Supplier = new SupplierDto
                    {
                        Id = s.SupplierOrder.Supplier.Id,
                        Name = s.SupplierOrder.Supplier.Name,
                        Contact = s.SupplierOrder.Supplier.Contact,
                        Address = s.SupplierOrder.Supplier.Address
                    }
                })
                .FirstOrDefaultAsync();

            if (stock == null) return NotFound();
            return Ok(stock);
        }

        // ✅ POST create (with minimal fields)
        [HttpPost]
        public async Task<ActionResult<Stock>> Create([FromBody] StockDto dto)
        {
            var stock = new Stock
            {
                Reference = dto.Reference,
                Code = dto.Code,
                Dimensions = dto.Dimensions,
                Quantity = dto.Quantity,
                Status = dto.Status,
                Location = dto.Location,
                IdSupplierOrder = dto.Supplier.Id
            };

            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ Stock ajouté : ID = {stock.Id}");
            return CreatedAtAction(nameof(GetById), new { id = stock.Id }, stock);
        }

        // ✅ PUT update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Stock item)
        {
            if (id != item.Id) return BadRequest();
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"✏️ Stock modifié : ID = {id}");
            return NoContent();
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Stocks.FindAsync(id);
            if (item == null) return NotFound();
            _context.Stocks.Remove(item);
            await _context.SaveChangesAsync();
            Console.WriteLine($"🗑️ Stock supprimé : ID = {id}");
            return NoContent();
        }
    }
}

