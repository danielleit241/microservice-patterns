
using Play.Common.MongoDb;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;

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
            });

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
