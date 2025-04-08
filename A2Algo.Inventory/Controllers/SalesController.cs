using A2Algo.Inventory.Contracts.Requests;
using A2Algo.Inventory.Contracts.Responses;
using A2Algo.Inventory.Data;
using A2Algo.Inventory.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace A2Algo.Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly InventoryDbContext _dbContext;

        public SalesController(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddSalesProduct([FromBody] AddSaleRequest saleRequest, CancellationToken token)
        {
            if (saleRequest.Products is null || saleRequest.Products.Count == 0)
            {
                return BadRequest(new BaseResponse(400, "Bad Request", "At least one product should be selected.", null));
            }
            using var transaction = await _dbContext.Database.BeginTransactionAsync(token);
            try
            {
                var sale = new Sale
                {
                    SaleDate = DateTimeOffset.Now,
                    CustomerName = saleRequest.CustomerName
                };

                await _dbContext.Sales.AddAsync(sale, token);
                await _dbContext.SaveChangesAsync(token);

                foreach (var productDetail in saleRequest.Products)
                {
                    var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productDetail.ProductId && !p.IsDeleted, token);
                    if (product == null || product.Quantity < productDetail.Quantity)
                        return BadRequest(new BaseResponse(400, "Insufficient stock or product not found.", null, null));

                    product.Quantity -= productDetail.Quantity;

                    var saleDetail = new SaleDetail
                    {
                        SaleId = sale.Id,
                        ProductId = product.Id,
                        Quantity = productDetail.Quantity,
                        Price = product.Price,
                    };

                    await _dbContext.SaleDetails.AddAsync(saleDetail, token);
                }

                await _dbContext.SaveChangesAsync(token);
                await transaction.CommitAsync(token);

                return Ok(new BaseResponse(200, "Sale recorded successfully", null, sale));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(token);
                return StatusCode(500, new BaseResponse(500, "Internal Server Error", ex.Message, null));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewSales(CancellationToken token)
        {
            var sales = await _dbContext.Sales
                .Include(s => s.SaleDetails)
                .AsNoTracking()
                .ToListAsync(token);


            var responseModel = sales.Select(s => new
            {
                Id = s.Id,
                CustomerName = s.CustomerName,
                SaleDate = s.SaleDate,
                TotalAmount = s.SaleDetails!.Sum(pd => pd.Quantity * pd.Price)
            });

            return Ok(new BaseResponse(200, "Sales Retrieved Successfully", null, responseModel));
        }

        [HttpGet("{id:int}/products")]
        public async Task<IActionResult> GetProductsBySaleId(int id, CancellationToken token)
        {
            var saleDetails = await _dbContext.SaleDetails
                .Include(sd => sd.Product)
                .Where(sd => sd.SaleId == id)
                .AsNoTracking()
                .ToListAsync(token);

            if (!saleDetails.Any())
            {
                return NotFound(new BaseResponse(404, "Sale or Products Not Found", null, null));
            }

            var products = saleDetails.Select(sd => new
            {
                Id = sd.ProductId,
                sd.Product!.Name,
                sd.Product.Description,
                sd.Quantity,
                sd.Price,
            });

            return Ok(new BaseResponse(200, "Products Retrieved Successfully", null, products));
        }
    }
}
