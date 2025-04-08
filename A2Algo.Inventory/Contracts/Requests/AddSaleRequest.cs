namespace A2Algo.Inventory.Contracts.Requests
{
    public class AddSaleRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public List<SaleProductRequest> Products { get; set; } = [];
    }
}
