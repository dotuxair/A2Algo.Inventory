using A2Algo.Inventory.Contracts.Requests;
using A2Algo.Inventory.Contracts.Responses;
using A2Algo.Inventory.Data;
using A2Algo.Inventory.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace A2Algo.Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly InventoryDbContext _dbContext;

        public ProductsController(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest productRequest, CancellationToken token)
        {
            var product = new Product
            {
                Name = productRequest.Name,
                Description = productRequest.Description,
                Price = productRequest.Price,
                Quantity = productRequest.Quantity
            };

            try
            {
                await _dbContext.Products.AddAsync(product, token);
                await _dbContext.SaveChangesAsync(token);

                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, new BaseResponse(201, "Product Created Successfully", null, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse(500, "Internal Server Error", ex.Message, null));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(CancellationToken token)
        {
            var products = await _dbContext.Products.AsNoTracking().Where(p => p.IsDeleted == false).ToListAsync(token);
            return Ok(new BaseResponse(200, "Products Retrieved Successfully", null, products));
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id, CancellationToken token)
        {

            var product = await _dbContext.Products.FirstOrDefaultAsync(p => (p.Id == id && p.IsDeleted == false), token);

            if (product == null)
            {
                return NotFound(new BaseResponse(404, "Product Not Found", null, null));
            }

            return Ok(new BaseResponse(200, "Product Retrieved Successfully", null, product));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest productRequest, CancellationToken token)
        {
            var product = await _dbContext.Products.FindAsync(id, token);

            if (product == null)
            {
                return NotFound(new BaseResponse(404, "Product Not Found", null, null));
            }

            product.Name = productRequest.Name;
            product.Description = productRequest.Description;
            product.Price = productRequest.Price;
            product.Quantity = productRequest.Quantity;
            product.UpdatedAt = DateTime.UtcNow;

            try
            {
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync(token);

                return Ok(new BaseResponse(200, "Product Updated Successfully", null, product));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse(500, "Internal Server Error", ex.Message, null));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken token)
        {
            var product = await _dbContext.Products.FindAsync(id, token);

            if (product == null)
            {
                return NotFound(new BaseResponse(404, "Product Not Found", null, null));
            }

            try
            {
                product.IsDeleted = true;
                await _dbContext.SaveChangesAsync(token);

                return Ok(new BaseResponse(200, "Product Deleted Successfully", null, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse(500, "Internal Server Error", ex.Message, null));
            }
        }

        [HttpGet("products-track")]
        public async Task<IActionResult> GetProducts(CancellationToken token)
        {
            var products = await _dbContext.Products.AsNoTracking().Where(p => p.IsDeleted == false).Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                quantity = p.Quantity
            }).ToListAsync(token);

            return Ok(new BaseResponse(200, "Products Retrieved Successfully", null, products));
        }
    }
}
