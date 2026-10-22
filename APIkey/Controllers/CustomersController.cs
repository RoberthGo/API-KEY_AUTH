using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIkey.Data;
using APIkey.DTOs;
using APIkey.Filters;

namespace APIkey.Controllers
{
    [EnableCors("AllowAll")]
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ContextDB _context;

        public CustomersController(ContextDB context)
        {
            _context = context;
        }

        // GET: api/customers
        // Retorna solo CustomerId y Nombre, evitando traer bytes de imagen o datos pesados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var customers = await _context.Customers
                .AsNoTracking()
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    CompanyName = c.CompanyName,
                    ContactName = c.ContactName
                })
                .ToListAsync();

            return Ok(customers);
        }

        // GET: api/customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(string id)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .Where(c => c.CustomerId == id)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    CompanyName = c.CompanyName,
                    ContactName = c.ContactName
                })
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Cliente no encontrado",
                    detail: $"No se encontró ningún cliente con el ID '{id}'."
                );
            }

            return Ok(customer);
        }
    }
}
