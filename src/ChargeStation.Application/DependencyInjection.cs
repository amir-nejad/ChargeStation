﻿using ChargeStation.Application.Interfaces;
using ChargeStation.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ChargeStation.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddScoped<IChargeStationService, ChargeStationService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IConnectorService, ConnectorService>();

            return services;
        }
    }
}