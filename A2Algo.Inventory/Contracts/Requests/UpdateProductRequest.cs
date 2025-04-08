using System.ComponentModel.DataAnnotations;

namespace A2Algo.Inventory.Contracts.Requests
{
        public class UpdateProductRequest
        {
            [Required]
            public string Name { get; set; } = string.Empty;
            [Required]
            public string Description { get; set; } = string.Empty;
            [Required]
            public int Quantity { get; set; }
            [Required]
            public decimal Price { get; set; }
        }
}
