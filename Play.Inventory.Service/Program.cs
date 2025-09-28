
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

            builder.Services.AddMongoDb(builder.Configuration)
                .AddMongoRepository<InventoryItem>("inventoryitems");

            builder.Services.AddHttpClient<CatalogClient>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ServiceClients:Catalog"] ?? throw new Exception("Services:Catalog is empty"));
            })
                .AddTransientHttpErrorPolicy(policy =>
                    policy.Or<TimeoutRejectedException>()
                        .WaitAndRetryAsync
                        (
                            5,
                            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            onRetry: (outcome, timespan, retryAttempt) =>
                            {
                                var serviceProvider = builder.Services.BuildServiceProvider();
                                serviceProvider.GetService<ILogger<CatalogClient>>()?
                                    .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
                            }
                        )
                    )
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

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
    }
}
