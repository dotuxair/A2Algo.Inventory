using System.Text.Json.Serialization;

namespace A2Algo.Inventory.Data.Entities
{
    public class Purchase
    {
        public int Id { get; set; }
        public DateTimeOffset PurchaseDate { get; set; } = DateTimeOffset.Now;
        public required string SupplierName { get; set; }

        [JsonIgnore]
        public ICollection<PurchaseDetail>? PurchaseDetails { get; set; }
    }
}
