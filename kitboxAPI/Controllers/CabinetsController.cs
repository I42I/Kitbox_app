using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KitboxAPI.Data;
using KitboxAPI.Models;
using KitboxAPI.Dtos;

namespace KitboxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CabinetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CabinetsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET all cabinets (DTO)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CabinetDto>>> GetAll()
        {
            var cabinets = await _context.Cabinets
                .Include(c => c.Lockers)
                .Select(c => new CabinetDto
                {
                    Id = c.Id,
                    IdOrder = c.IdOrder,
                    Price = c.Price,
                    Dimensions = c.Dimensions,
                    Reference = c.Reference,
                    Lockers = c.Lockers.Select(l => new LockerDto
                    {
                        Id = l.Id,
                        Reference = l.Reference,
                        Code = l.Code,
                        Dimensions = l.Dimensions
                    }).ToList()
                })
                .ToListAsync();

            return Ok(cabinets);
        }

        // ✅ GET cabinet by id (DTO)
        [HttpGet("{id}")]
        public async Task<ActionResult<CabinetDto>> GetById(int id)
        {
            var cabinet = await _context.Cabinets
                .Include(c => c.Lockers)
                .Where(c => c.Id == id)
                .Select(c => new CabinetDto
                {
                    Id = c.Id,
                    IdOrder = c.IdOrder,
                    Price = c.Price,
                    Dimensions = c.Dimensions,
                    Reference = c.Reference,
                    Lockers = c.Lockers.Select(l => new LockerDto
                    {
                        Id = l.Id,
                        Reference = l.Reference,
                        Code = l.Code,
                        Dimensions = l.Dimensions
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (cabinet == null) return NotFound();
            return Ok(cabinet);
        }

        // ✅ POST create cabinet (with DTO)
        [HttpPost]
        public async Task<ActionResult> Create(CabinetCreateDto dto)
        {
            var cabinet = new Cabinet
            {
                IdOrder = dto.IdOrder,
                Price = dto.Price,
                Dimensions = dto.Dimensions,
                Reference = dto.Reference
            };

            _context.Cabinets.Add(cabinet);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ Cabinet ajouté : ID = {cabinet.Id}");
            return CreatedAtAction(nameof(GetById), new { id = cabinet.Id }, cabinet);
        }

        // ✅ PUT update cabinet
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Cabinet cabinet)
        {
            if (id != cabinet.Id) return BadRequest();

            _context.Entry(cabinet).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            Console.WriteLine($"✏️ Cabinet modifié : ID = {id}");
            return NoContent();
        }

        // ✅ DELETE cabinet
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cabinet = await _context.Cabinets.FindAsync(id);
            if (cabinet == null) return NotFound();

            _context.Cabinets.Remove(cabinet);
            await _context.SaveChangesAsync();

            Console.WriteLine($"🗑️ Cabinet supprimé : ID = {id}");
            return NoContent();
        }
    }
}
