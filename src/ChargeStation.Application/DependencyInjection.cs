using ChargeStation.Application.Interfaces;
using ChargeStation.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;

namespace ChargeStation.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(dispose: true);
            });

            services.AddScoped<IChargeStationService, ChargeStationService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IConnectorService, ConnectorService>();

            return services;
        }
    }
}
