
using Play.Common.MassTransit;
using Play.Common.MongoDb;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

namespace Play.Inventory.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var configuration = builder.Configuration;

            builder.Services.AddMongoDb(configuration)
                .AddMongoRepository<InventoryItem>("inventoryitems")
                .AddMongoRepository<CatalogItem>("catalogitems")
                .AddMassTransitWithRabbitMq(configuration);

            AddCatalogClient(builder);

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static void AddCatalogClient(WebApplicationBuilder builder)
        {
            Random jitterer = new Random();

            builder.Services.AddHttpClient<CatalogClient>(client =>
            {
                client.BaseAddress = new Uri(
                    builder.Configuration["ServiceClients:Catalog"]
                    ?? throw new Exception("Services:Catalog is empty"));
            })
            .AddTransientHttpErrorPolicy(policy =>
                policy.Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(
                        retryCount: 5,
                        sleepDurationProvider: retryAttempt =>
                            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                            TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
                        onRetry: (outcome, timespan, retryAttempt, context) =>
                        {
                            var serviceProvider = builder.Services.BuildServiceProvider();
                            serviceProvider.GetService<ILogger<CatalogClient>>()?
                                .LogWarning(
                                    $"Delaying for {timespan.TotalSeconds} seconds, " +
                                    $"then making retry {retryAttempt}");
                        }
                    )
            )
            .AddTransientHttpErrorPolicy(policy =>
                policy.Or<TimeoutRejectedException>()
                    .AdvancedCircuitBreakerAsync(
                        failureThreshold: 0.5,
                        samplingDuration: TimeSpan.FromSeconds(15),
                        minimumThroughput: 3,
                        durationOfBreak: TimeSpan.FromSeconds(15),
                        onBreak: async (outcome, breakDelay, context) =>
                        {
                            var serviceProvider = builder.Services.BuildServiceProvider();
                            serviceProvider.GetService<ILogger<CatalogClient>>()?
                                .LogWarning($"Opening the circuit for {breakDelay.TotalSeconds} seconds...");
                        },
                        onReset: (context) =>
                        {
                            var serviceProvider = builder.Services.BuildServiceProvider();
                            serviceProvider.GetService<ILogger<CatalogClient>>()?
                                .LogWarning("Closing the circuit...");
                        }
                    )
            )
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
        }
    }
}
