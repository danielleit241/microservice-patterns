using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;
using System.Reflection;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(configure =>
            {
                configure.AddConsumers(Assembly.GetEntryAssembly());

                configure.UsingRabbitMq((context, configurator) =>
                {
                    var rabbitMqSettings = configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>()
                        ?? throw new ArgumentNullException(nameof(RabbitMQSettings));
                    configurator.Host(rabbitMqSettings.Host);
                    configurator.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
