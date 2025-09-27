using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDB
{
    public static class Extensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration Configuration)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var mongosettings = Configuration.GetSection(nameof(MongoSettings)).Get<MongoSettings>()
                    ?? throw new ArgumentNullException(nameof(MongoSettings));
                var connectionStrings = Configuration.GetSection(nameof(ConnectionStrings)).Get<ConnectionStrings>()
                    ?? throw new ArgumentNullException(nameof(ConnectionStrings));

                var client = new MongoClient(connectionStrings.MongoDb);
                return client.GetDatabase(mongosettings.DatabaseName);
            });

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string CollectionName) where T : IEntity
        {
            ArgumentException.ThrowIfNullOrEmpty(CollectionName);

            services.AddScoped<IRepository<T>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                return new MongoRepository<T>(database, CollectionName);
            });

            return services;
        }
    }
}
