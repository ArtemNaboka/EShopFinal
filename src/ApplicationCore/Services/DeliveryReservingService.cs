using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Newtonsoft.Json;
using OrdersProcessor.Contracts.Models;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public class DeliveryReservingService : IDeliveryReservingService
    {
        private readonly HttpClient _reservingClient;

        public DeliveryReservingService(HttpClient reservingClient)
        {
            _reservingClient = reservingClient;
        }

        public async Task ReserveAsync(Order order)
        {
            var orderDto = new DeliveryOrderDto
            {
                OrderId = order.Id,
                Total = order.Total(),
                ShipToAddress = new ShipAddressDto
                {
                    City = order.ShipToAddress.City,
                    Country = order.ShipToAddress.Country,
                    State = order.ShipToAddress.State,
                    Street = order.ShipToAddress.Street,
                    ZipCode = order.ShipToAddress.ZipCode
                },
                OrderItems = order.OrderItems.Select(i => new OrderItemDto
                {
                    CatalogItemId = i.ItemOrdered.CatalogItemId,
                    ProductName = i.ItemOrdered.ProductName,
                    Quantity = i.Units
                })
                .ToArray()
            };

            var content = new StringContent(JsonConvert.SerializeObject(orderDto));
            var response = await _reservingClient.PostAsync("delivery/reserve", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
