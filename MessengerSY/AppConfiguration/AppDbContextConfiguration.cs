using MessengerSY.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.AppConfiguration
{
    public static class AppDbContextConfiguration
    {
        public static IServiceCollection AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MessengerDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("MessengerConnection"));
                    options.UseLazyLoadingProxies();
                });

            return services;
        }
    }
}
