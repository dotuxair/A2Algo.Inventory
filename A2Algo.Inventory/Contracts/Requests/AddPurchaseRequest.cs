namespace A2Algo.Inventory.Contracts.Requests
{
    public class AddPurchaseRequest
    {
        public string SupplierName { get; set; } = default!;
        public List<PurchaseProductRequest> PurchaseProducts { get; set; } = [];
    }
}
