using Microsoft.Azure.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Newtonsoft.Json;
using OrdersProcessor.Contracts.Models;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public class OrderReservingService : IOrderReservingService
    {
        private readonly HttpClient _reservingClient;
        private readonly ITopicClient _topicClient;

        public OrderReservingService(
            HttpClient reservingClient,
            ITopicClient topicClient)
        {
            _reservingClient = reservingClient;
            _topicClient = topicClient;
        }

        public async Task ReserveAsync(Order order)
        {
            var json = GetJsonDto(order);
            var content = new StringContent(json);
            var response = await _reservingClient.PostAsync("orders/reserve", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendReservationAsync(Order order)
        {
            var json = GetJsonDto(order);
            var message = new Message(Encoding.UTF8.GetBytes(json));

            await _topicClient.SendAsync(message);
        }

        private static string GetJsonDto(Order order)
        {
            var orderDto = new ReservingOrderDto
            {
                Id = order.Id,
                Items = order.OrderItems.Select(i => new OrderItemDto
                {
                    CatalogItemId = i.ItemOrdered.CatalogItemId,
                    ProductName = i.ItemOrdered.ProductName,
                    Quantity = i.Units
                })
                .ToArray()
            };

            return JsonConvert.SerializeObject(orderDto);
        }
    }
}
