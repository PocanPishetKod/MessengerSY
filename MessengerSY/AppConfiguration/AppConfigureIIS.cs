using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.AppConfiguration
{
    public static class AppConfigureIIS
    {
        public static IServiceCollection AddIISConfiguration(this IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            return services;
        }
    }
}
