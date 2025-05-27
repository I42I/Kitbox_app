using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KitboxAPI.Data;
using KitboxAPI.Models;
using KitboxAPI.Dtos;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace KitboxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerOrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerOrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerOrder>>> GetAll()
        {
            try
            {
                var orders = await _context.CustomerOrders.ToListAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur dans GetAll CustomerOrders: {ex.Message}");
                return StatusCode(500, "Une erreur interne est survenue.");
            }
        }

        // ✅ GET /api/customerorders/{id}/full
        [HttpGet("{id}/full")]
        public async Task<ActionResult<CustomerOrderFullDto>> GetFullOrder(int id)
        {
            try
            {
                var order = await _context.CustomerOrders
                    .Include(o => o.Cabinets)
                        .ThenInclude(c => c.Lockers)
                            .ThenInclude(l => l.LockerStocks)
                                .ThenInclude(ls => ls.Stock)
                                    .ThenInclude(s => s.SupplierOrder)
                                        .ThenInclude(so => so.Supplier)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                    return NotFound();

                var result = new CustomerOrderFullDto
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    DepositAmount = order.DepositAmount,
                    Tel = order.Tel,
                    Mail = order.Mail,
                    Cabinets = order.Cabinets.Select(c => new CabinetDto
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
                        }).ToList()
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log l'exception pour déboguer
                Console.WriteLine($"Erreur dans la récupération de la commande {id}: {ex.Message}");
                return StatusCode(500, "Une erreur interne est survenue.");
            }
        }

        // ✅ POST create customer order
        [HttpPost]
        public async Task<ActionResult<CustomerOrder>> Create([FromBody] CustomerOrderCreateDto dto)
        {
            try
            {
                var order = new CustomerOrder
                {
                    OrderDate = dto.OrderDate,
                    Status = dto.Status,
                    DepositAmount = dto.DepositAmount,
                    Tel = dto.Tel,
                    Mail = dto.Mail
                };

                _context.CustomerOrders.Add(order);
                await _context.SaveChangesAsync();
                Console.WriteLine($"✅ CustomerOrder ajouté : ID = {order.Id}");

                return CreatedAtAction(nameof(GetFullOrder), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                // Log l'exception pour le débogage
                Console.WriteLine($"Erreur dans la création de la commande : {ex.Message}");
                return StatusCode(500, "Une erreur interne est survenue.");
            }
        }
    }
}
