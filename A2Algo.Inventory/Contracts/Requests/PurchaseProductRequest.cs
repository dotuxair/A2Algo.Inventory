namespace A2Algo.Inventory.Contracts.Requests
{
    public class PurchaseProductRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
