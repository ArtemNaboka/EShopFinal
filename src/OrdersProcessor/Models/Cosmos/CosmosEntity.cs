using Newtonsoft.Json;
using System;

namespace OrdersProcessor.Models.Cosmos
{
    public abstract class CosmosEntity
    {
        [JsonProperty("id")]
        public string Id { get; } = Guid.NewGuid().ToString();
    }
}
