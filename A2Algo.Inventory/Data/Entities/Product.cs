using System.ComponentModel.DataAnnotations;

namespace A2Algo.Inventory.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Quantity { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<SaleDetail>? SaleDetails { get; set; }
        public ICollection<PurchaseDetail>? PurchaseDetails { get; set; }
    }
}
