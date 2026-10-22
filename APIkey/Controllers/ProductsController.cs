using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIkey.Data;
using APIkey.DTOs;
using APIkey.Filters;
using APIkey.Models;

namespace APIkey.Controllers
{
    [EnableCors("AllowAll")]
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ContextDB _context;

        public ProductsController(ContextDB context)
        {
            _context = context;
        }

        // GET: api/Products
        // Retorna la cantidad total de productos y solo nombre, precio y stock de cada uno
        [HttpGet]
        public async Task<ActionResult<ProductsSummaryDto>> GetProducts()
        {
            var total = await _context.Products.CountAsync();
            var products = await _context.Products
                .AsNoTracking()
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    UnitPrice = p.UnitPrice,
                    UnitsInStock = p.UnitsInStock
                })
                .ToListAsync();

            return Ok(new ProductsSummaryDto
            {
                TotalProducts = total,
                Products = products
            });
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    UnitPrice = p.UnitPrice,
                    UnitsInStock = p.UnitsInStock
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Producto no encontrado",
                    detail: $"No se encontró ningún producto con el identificador {id}."
                );
            }

            return Ok(product);
        }

        // POST: api/Products
        // Valida con DataAnnotations antes de guardar
        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductUpsertDto dto)
        {
            try
            {
                var product = new Product
                {
                    ProductName = dto.ProductName,
                    UnitPrice = dto.UnitPrice,
                    UnitsInStock = dto.UnitsInStock,
                    Discontinued = dto.Discontinued
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                var responseDto = new ProductDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice,
                    UnitsInStock = product.UnitsInStock
                };

                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, responseDto);
            }
            catch (DbUpdateException ex)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Error al registrar producto",
                    detail: ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        // PUT: api/Products/5
        // Actualiza un producto existente validando los datos recibidos
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductUpsertDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Producto no encontrado",
                    detail: $"No se encontró ningún producto con el identificador {id} para actualizar."
                );
            }

            product.ProductName = dto.ProductName;
            product.UnitPrice = dto.UnitPrice;
            product.UnitsInStock = dto.UnitsInStock;
            product.Discontinued = dto.Discontinued;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Error al actualizar producto",
                    detail: ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Producto no encontrado",
                    detail: $"No se encontró ningún producto con el identificador {id} para eliminar."
                );
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Conflicto de integridad referencial",
                    detail: $"El producto con ID {id} no puede ser eliminado porque tiene órdenes u otros registros vinculados."
                );
            }
        }
    }
}
