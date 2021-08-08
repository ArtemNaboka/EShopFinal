using OrdersProcessor.Contracts.Models;

namespace OrdersProcessor.Models.Cosmos
{
    public class DeliveryOrderEntity : CosmosEntity
    {
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public ShipAddressDto ShipToAddress { get; set; }
        public OrderItemDto[] OrderItems { get; set; }
    }
}
