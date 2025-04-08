using System.Text.Json.Serialization;

namespace A2Algo.Inventory.Data.Entities
{
    public class PurchaseDetail
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; } 
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        [JsonIgnore]
        public Purchase? Purchase { get; set; }
        [JsonIgnore]

        public Product? Product { get; set; }
    }
}
