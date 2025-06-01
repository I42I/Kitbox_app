using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KitboxAPI.Data;
using KitboxAPI.Models;
using KitboxAPI.Dtos;

namespace KitboxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierOrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SupplierOrdersController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET all supplier orders (DTO)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierOrderDto>>> GetAll()
        {
            var orders = await _context.SupplierOrders
                .Include(so => so.Supplier)
                .Select(so => new SupplierOrderDto
                {
                    Id = so.Id,
                    OrderDate = so.OrderDate,
                    Supplier = new SupplierDto
                    {
                        Id = so.Supplier.Id,
                        Name = so.Supplier.Name,
                        Contact = so.Supplier.Contact,
                        Address = so.Supplier.Address
                    }
                })
                .ToListAsync();

            return Ok(orders);
        }

        // ✅ GET by ID (DTO)
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierOrderDto>> GetById(int id)
        {
            var order = await _context.SupplierOrders
                .Include(so => so.Supplier)
                .Where(so => so.Id == id)
                .Select(so => new SupplierOrderDto
                {
                    Id = so.Id,
                    OrderDate = so.OrderDate,
                    Supplier = new SupplierDto
                    {
                        Id = so.Supplier.Id,
                        Name = so.Supplier.Name,
                        Contact = so.Supplier.Contact,
                        Address = so.Supplier.Address
                    }
                })
                .FirstOrDefaultAsync();

            if (order == null) return NotFound();
            return Ok(order);
        }

        // ✅ POST create (with Supplier ID from DTO)
        [HttpPost]
        public async Task<ActionResult<SupplierOrder>> Create(SupplierOrderDto dto)
        {
            var supplierOrder = new SupplierOrder
            {
                OrderDate = dto.OrderDate,
                IdSupplier = dto.Supplier.Id
            };

            _context.SupplierOrders.Add(supplierOrder);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ SupplierOrder ajouté : ID = {supplierOrder.Id}");
            return CreatedAtAction(nameof(GetById), new { id = supplierOrder.Id }, supplierOrder);
        }

        // ✅ PUT update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SupplierOrder item)
        {
            if (id != item.Id) return BadRequest();
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"✏️ SupplierOrder modifié : ID = {id}");
            return NoContent();
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.SupplierOrders.FindAsync(id);
            if (item == null) return NotFound();
            _context.SupplierOrders.Remove(item);
            await _context.SaveChangesAsync();
            Console.WriteLine($"🗑️ SupplierOrder supprimé : ID = {id}");
            return NoContent();
        }
    }
}

