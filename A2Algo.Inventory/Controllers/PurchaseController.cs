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
    public class PurchaseController : ControllerBase
    {
        private readonly InventoryDbContext _dbContext;

        public PurchaseController(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddPurchase([FromBody] AddPurchaseRequest purchaseRequest, CancellationToken token)
        {
            if (purchaseRequest.PurchaseProducts is null ||purchaseRequest.PurchaseProducts.Count == 0)
            {
                return BadRequest(new BaseResponse(404, "Bad Request", "At least one product should be selected.", null));
            }
            using var transaction = await _dbContext.Database.BeginTransactionAsync(token);
            try
            {
                var purchase = new Purchase
                {
                    SupplierName = purchaseRequest.SupplierName,
                    PurchaseDate = DateTimeOffset.Now,
                };

                await _dbContext.Purchases.AddAsync(purchase, token);
                await _dbContext.SaveChangesAsync(token);

                foreach (var detail in purchaseRequest.PurchaseProducts)
                {
                    var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == detail.ProductId && !p.IsDeleted, token);
                    if (product == null)
                        return BadRequest(new BaseResponse(400, "Product not found.", null, null));

                    product.Quantity += detail.Quantity;
                    product.Price = detail.Price;

                    var purchaseDetail = new PurchaseDetail
                    {
                        PurchaseId = purchase.Id,
                        ProductId = product.Id,
                        Quantity = detail.Quantity,
                        Price = detail.Price,
                    };

                    await _dbContext.PurchaseDetails.AddAsync(purchaseDetail, token);
                }

                await _dbContext.SaveChangesAsync(token);
                await transaction.CommitAsync(token);

                return Ok(new BaseResponse(200, "Purchase recorded successfully", null, null));
            }
            catch 
            {
                await transaction.RollbackAsync(token);
                return StatusCode(500, new BaseResponse(500, "Internal Server Error", null, null));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewPurchasesWithTotalAmount(CancellationToken token)
        {
            var purchases = await _dbContext.Purchases
                .Include(p => p.PurchaseDetails) 
                .AsNoTracking()
                .ToListAsync(token);

            var purchaseViewModels = purchases.Select(p => new
            {
                p.Id, 
                p.PurchaseDate,
                p.SupplierName,
                TotalAmount = p.PurchaseDetails!.Sum(pd => pd.Quantity * pd.Price)
            }).ToList();

            return Ok(new BaseResponse(200, "Purchases Retrieved Successfully with Total Amount", null, purchaseViewModels));
        }


        [HttpGet("{id:int}/products")]
        public async Task<IActionResult> GetProductsByPurchaseId(int id, CancellationToken token)
        {
            var purchaseDetails = await _dbContext.PurchaseDetails
                .Include(pd => pd.Product)
                .Where(pd => pd.PurchaseId == id)
                .AsNoTracking()
                .ToListAsync(token);

            if (!purchaseDetails.Any())
            {
                return NotFound(new BaseResponse(404, "Purchase or Products Not Found", null, null));
            }

            var products = purchaseDetails.Select(pd => new
            {
                Id = pd.ProductId,
                pd.Product!.Name,
                pd.Product.Description,
                pd.Quantity,
                pd.Price,
            });

            return Ok(new BaseResponse(200, "Products Retrieved Successfully", null, products));
        }
    }
}
