using Play.Catalog.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.MongoDb;

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

            builder.Services
                .AddMongoDb(builder.Configuration)
                .AddMongoRepository<Item>("catalogitems")
                .AddMassTransitWithRabbitMq(builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
