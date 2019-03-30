using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.AppConfiguration
{
    public static class AppSignalRConfiguration
    {
        public static IServiceCollection AddSignalRConfiguration(this IServiceCollection services)
        {
            services.AddSignalR(opt =>
            {
                opt.KeepAliveInterval = TimeSpan.FromSeconds(15);
            });
            return services;
        }
    }
}
