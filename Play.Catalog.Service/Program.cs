
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
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

            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            builder.Services.Configure<MongoSettings>(options =>
            {
                options.MongoDbConnection = builder.Configuration.GetConnectionString("MongoDbConnection")
                    ?? throw new Exception("MongoDbConnection is not found");
                options.DatabaseName = builder.Configuration["MongoSettings:DatabaseName"]
                    ?? throw new Exception("DatabaseName is not found");
            });

            builder.Services.AddSingleton<IMongoDatabase>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
                var client = new MongoClient(settings.MongoDbConnection);
                return client.GetDatabase(settings.DatabaseName);
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
