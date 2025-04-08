using System.Text.Json.Serialization;

namespace A2Algo.Inventory.Data.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTimeOffset SaleDate { get; set; } = DateTimeOffset.Now;
        [JsonIgnore]

        public ICollection<SaleDetail>? SaleDetails { get; set; }
    }
}
