using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.AppConfiguration
{
    public static class SMSConfiguration
    {
        public static IServiceCollection AddSMSProviderConfiguration(this IServiceCollection services)
        {
            services.AddHttpClient("sms.ru", options =>
                {
                    options.BaseAddress = new Uri(@"https://sms.ru/");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(2));

            return services;
        }
    }
}
