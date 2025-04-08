using System.Text.Json.Serialization;

namespace A2Algo.Inventory.Data.Entities
{
    public class SaleDetail
    {
        public int Id { get; set; }
        public int SaleId { get; set; } 
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        [JsonIgnore]

        public Sale? Sale { get; set; }
        [JsonIgnore]

        public Product? Product { get; set; }
    }
}
