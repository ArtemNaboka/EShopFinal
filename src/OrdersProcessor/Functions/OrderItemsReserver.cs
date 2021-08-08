using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.ServiceBus;
using OrdersProcessor.Contracts.Models;

namespace OrdersProcessor.Functions
{
    public class OrderItemsReserver
    {
        private readonly ILogger _logger;

        public OrderItemsReserver(ILogger<OrderItemsReserver> logger)
        {
            _logger = logger;
        }

        [FunctionName("OrderItemsReserver")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders/reserve")] HttpRequest request,
            [Blob("order-reserves", Connection = "StorageConnection")] CloudBlobContainer blobContainer)
        {
            _logger.LogInformation("OrderItemsReserver triggered!");

            using (var reader = new StreamReader(request.Body))
            {
                var json = await reader.ReadToEndAsync();
                var order = JsonConvert.DeserializeObject<ReservingOrderDto>(json);

                await blobContainer.CreateIfNotExistsAsync();

                await SaveOrderJsonAsync($"order-{order.Id}.json", json, blobContainer);
            }

            return new OkObjectResult("Order Reserved!");
        }

        [FunctionName("ServiceBusOrderItemsReserver")]
        public async Task RunFromServiceBus(
            [ServiceBusTrigger("%TopicName%", "%Subscription%")] Message message,
            [Blob("%ReserverContainer%", Connection = "StorageConnection")] CloudBlobContainer blobContainer)
        {
            _logger.LogInformation("OrderItemsReserver triggered!");

            var orderJson = Encoding.UTF8.GetString(message.Body);
            var order = JsonConvert.DeserializeObject<ReservingOrderDto>(orderJson);

            await blobContainer.CreateIfNotExistsAsync();

            await SaveOrderJsonAsync($"order-{order.Id}.json", orderJson, blobContainer);
        }

        private async Task SaveOrderJsonAsync(string fileName, string content, CloudBlobContainer blobContainer)
        {
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var blob = blobContainer.GetBlockBlobReference(fileName);
            await blob.UploadFromStreamAsync(memoryStream);
        }
    }
}
