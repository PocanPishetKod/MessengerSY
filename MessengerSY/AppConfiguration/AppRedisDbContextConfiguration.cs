using MessengerSY.Data.Context;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.AppConfiguration
{
    public static class AppRedisDbContextConfiguration
    {
        public static IServiceCollection AddRedisDbContext(this IServiceCollection services)
        {
            services.AddSingleton<IMessengerRedisDbContext, MessengerRedisDbContext>();

            return services;
        }
    }
}
