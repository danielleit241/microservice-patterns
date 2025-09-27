
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton(ServiceProvider =>
            {
                var connectionString = builder.Configuration.GetConnectionString("MongoDb");
                var mongoClient = new MongoDB.Driver.MongoClient(connectionString);
                return mongoClient.GetDatabase("Catalog");
            });

            builder.Services.AddScoped<IItemRepository, ItemsRepository>();

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
