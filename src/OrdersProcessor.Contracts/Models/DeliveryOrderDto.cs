namespace OrdersProcessor.Contracts.Models
{
    public class DeliveryOrderDto
    {
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public ShipAddressDto ShipToAddress { get; set; }
        public OrderItemDto[] OrderItems { get; set; }
    }
}
