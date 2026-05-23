using System.Collections.Generic;

namespace InventoryApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int CategoryId { get; set; }

        // Navigation
        public Category? Category { get; set; }
        public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();
    }
}
