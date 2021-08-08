namespace OrdersProcessor.Contracts.Models
{
    public class OrderItemDto
    {
        public int CatalogItemId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
