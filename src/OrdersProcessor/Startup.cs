using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersProcessor.Configuration;
using System.IO;

[assembly: FunctionsStartup(typeof(OrdersProcessor.Startup))]

namespace OrdersProcessor
{
    public class Startup : FunctionsStartup
    {
        private const string CosmosDbSection = "CosmosDB";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            using var provider = builder.Services.BuildServiceProvider();
            var configuration = provider.GetService<IConfiguration>();

            if (configuration != null)
            {
                var cosmosSection = configuration.GetSection(CosmosDbSection);
                var cosmosSettings = new CosmosDbSettings();
                cosmosSection.Bind(cosmosSettings);

                builder.Services.Configure<CosmosDbSettings>(cosmosSection);

                builder.Services.AddSingleton(_ => new CosmosClient(cosmosSettings.Uri, cosmosSettings.Key));
            }
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            if (context.EnvironmentName == "Development")
            {
                builder.ConfigurationBuilder.AddJsonFile(
                    Path.Combine(context.ApplicationRootPath, "appsettings.json"),
                    optional: true,
                    reloadOnChange: false);
            }
        }
    }
}
