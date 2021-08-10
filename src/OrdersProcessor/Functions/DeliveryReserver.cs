using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrdersProcessor.Configuration;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using OrdersProcessor.Contracts.Models;
using OrdersProcessor.Models.Cosmos;

namespace OrdersProcessor.Functions
{
    public class DeliveryReserver
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IOptionsSnapshot<CosmosDbSettings> _cosmosDbOptions;
        private readonly ILogger _logger;

        public DeliveryReserver(
            CosmosClient cosmosClient,
            IOptionsSnapshot<CosmosDbSettings> cosmosDbOptions,
            ILogger<DeliveryReserver> logger)
        {
            _cosmosClient = cosmosClient;
            _cosmosDbOptions = cosmosDbOptions;
            _logger = logger;
        }

        [FunctionName("DeliveryReserver")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "delivery/reserve")] HttpRequest request)
        {
            _logger.LogInformation("DeliveryReserver triggered!");

            using (var reader = new StreamReader(request.Body))
            {
                var json = await reader.ReadToEndAsync();
                var order = JsonConvert.DeserializeObject<DeliveryOrderDto>(json);
                var orderEntity = GetDeliveryOrderEntity(order);

                var container = await GetContainerAsync();
                var response = await container.CreateItemAsync(orderEntity);

                return new OkObjectResult($"Delivery Reserved! Cosmos Response: {response.StatusCode}");
            }
        }

        private static DeliveryOrderEntity GetDeliveryOrderEntity(DeliveryOrderDto orderDto)
        {
            return new DeliveryOrderEntity
            {
                OrderId = orderDto.OrderId,
                OrderItems = orderDto.OrderItems,
                ShipToAddress = orderDto.ShipToAddress,
                Total = orderDto.Total,
            };
        }

        private async Task<Container> GetContainerAsync()
        {
            var settings = _cosmosDbOptions.Value;

            Database database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(settings.DatabaseName);

            return await database.CreateContainerIfNotExistsAsync(settings.ContainerName, $"/{nameof(DeliveryOrderEntity.OrderId)}", settings.RequestUnits);
        }
    }
}
