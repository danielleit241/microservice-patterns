using Play.Common.Jwt;
using Play.Common.MongoDb;
using Play.Identity.Service.Entities;
using Play.Identity.Service.Settings;

namespace Play.Identity.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddJwtBearerAuthentication(builder.Configuration);

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
            builder.Services.AddScoped<JwtTokenService>();

            var configuration = builder.Configuration;

            builder.Services.AddMongoDb(configuration)
                    .AddMongoRepository<User>("playusers");

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.AddApplicationAuthenAndAuthor();

            app.MapControllers();

            app.Run();
        }
    }
}
