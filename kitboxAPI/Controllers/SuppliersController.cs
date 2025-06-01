using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KitboxAPI.Data;
using KitboxAPI.Models;
using KitboxAPI.Dtos;

namespace KitboxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SuppliersController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET all suppliers (DTO)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll()
        {
            var suppliers = await _context.Suppliers
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Contact = s.Contact,
                    Address = s.Address
                })
                .ToListAsync();

            return Ok(suppliers);
        }

        // ✅ GET by ID (DTO)
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDto>> GetById(int id)
        {
            var supplier = await _context.Suppliers
                .Where(s => s.Id == id)
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Contact = s.Contact,
                    Address = s.Address
                })
                .FirstOrDefaultAsync();

            if (supplier == null) return NotFound();
            return Ok(supplier);
        }

        // ✅ POST create supplier (DTO)
        [HttpPost]
        public async Task<ActionResult<Supplier>> Create(SupplierDto dto)
        {
            var supplier = new Supplier
            {
                Name = dto.Name,
                Contact = dto.Contact,
                Address = dto.Address
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ Supplier ajouté : ID = {supplier.Id}");
            return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
        }

        // ✅ PUT update supplier
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Supplier item)
        {
            if (id != item.Id) return BadRequest();
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"✏️ Supplier modifié : ID = {id}");
            return NoContent();
        }

        // ✅ DELETE supplier
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Suppliers.FindAsync(id);
            if (item == null) return NotFound();
            _context.Suppliers.Remove(item);
            await _context.SaveChangesAsync();
            Console.WriteLine($"🗑️ Supplier supprimé : ID = {id}");
            return NoContent();
        }
    }
}

