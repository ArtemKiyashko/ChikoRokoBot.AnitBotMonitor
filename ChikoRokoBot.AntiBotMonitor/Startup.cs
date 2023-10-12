using Azure.Identity;
using Azure.Storage.Queues;
using ChikoRokoBot.AntiBotMonitor.Clients;
using ChikoRokoBot.AntiBotMonitor.Interfaces;
using ChikoRokoBot.AntiBotMonitor.Managers;
using ChikoRokoBot.AntiBotMonitor.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ChikoRokoBot.AntiBotMonitor.Startup))]
namespace ChikoRokoBot.AntiBotMonitor
{
	public class Startup : FunctionsStartup
	{
        private IConfigurationRoot _functionConfig;
        private AntiBotMonitorOptions _antiBotMonitorOptions = new();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            _functionConfig = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            builder.Services.Configure<AntiBotMonitorOptions>(_functionConfig.GetSection(nameof(AntiBotMonitorOptions)));
            builder.Services.Configure<ChikoRokoClientOptions>(_functionConfig.GetSection(nameof(ChikoRokoClientOptions)));
            _functionConfig.GetSection(nameof(AntiBotMonitorOptions)).Bind(_antiBotMonitorOptions);

            builder.Services.AddScoped<IHttpClient, ChikoRokoClient>();
            builder.Services.AddScoped<INotificationManager, NotificationManager>();

            builder.Services.AddHttpClient<ChikoRokoClient>();

            builder.Services.AddAzureClients(clientBuilder => {
                clientBuilder.UseCredential(new DefaultAzureCredential());
                clientBuilder
                    .AddQueueServiceClient(_antiBotMonitorOptions.StorageAccount)
                    .ConfigureOptions((options) => { options.MessageEncoding = QueueMessageEncoding.Base64; });

                clientBuilder.AddTableServiceClient(_antiBotMonitorOptions.StorageAccount);
            });

            builder.Services.AddScoped<QueueClient>((factory) => {
                var service = factory.GetRequiredService<QueueServiceClient>();
                var client = service.GetQueueClient(_antiBotMonitorOptions.QueueName);
                client.CreateIfNotExists();
                return client;
            });
        }
    }
}

