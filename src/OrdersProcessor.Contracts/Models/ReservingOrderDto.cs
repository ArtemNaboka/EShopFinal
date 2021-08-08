using System.Collections.Generic;

namespace OrdersProcessor.Contracts.Models
{
    public class ReservingOrderDto
    {
        public int Id { get; set; }
        public IReadOnlyCollection<OrderItemDto> Items { get; set; }
    }
}
