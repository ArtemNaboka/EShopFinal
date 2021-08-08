namespace OrdersProcessor.Configuration
{
    public class CosmosDbSettings
    {
        public string Uri { get; set; }
        public string Key { get; set; }
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
        public int RequestUnits { get; set; }
    }
}
