using MessengerSY.Core;
using MessengerSY.Core.JwtAuthOptions;
using MessengerSY.Core.RefreshTokenOptions;
using MessengerSY.Core.SmsProvider;
using MessengerSY.Data.Repository;
using MessengerSY.Services.ChatService;
using MessengerSY.Services.JwtService;
using MessengerSY.Services.OnlineStatusService;
using MessengerSY.Services.RefreshTokenService;
using MessengerSY.Services.SmsService;
using MessengerSY.Services.UserProfileService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.AppConfiguration
{
    public static class AppServicesConfiguration
    {
        public static IServiceCollection AddAppServicesConfiguration(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            services.AddSingleton<IJwtInfo>(new JwtInfo(Environment.GetEnvironmentVariable("ISSUER"),
                Environment.GetEnvironmentVariable("AUDIENCE"),
                Convert.ToInt32(Environment.GetEnvironmentVariable("JWT_TOKEN_LIFETIME_MINUTES")),
                StringGenerator.GenerateString(30),
                Environment.GetEnvironmentVariable("JWT_ALGORITHM")));

            services.AddSingleton<IRefreshTokenInfo>(new RefreshTokenInfo(Convert.ToInt32(Environment.GetEnvironmentVariable("MINIMUM_REFRESH_TOKEN_SIZE")),
                Convert.ToInt32(Environment.GetEnvironmentVariable("REFRESH_TOKEN_LIFITIME_DAYS")),
                Convert.ToInt32(Environment.GetEnvironmentVariable("EXTEND_TIME_HOURS"))));

            services.AddSingleton<ISmsProviderInfo>(
                new SmsProviderInfo(Environment.GetEnvironmentVariable("SMS_API_KEY")));

            services.AddTransient<IJwtService, JwtTokenService>();
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<ISmsSenderService, SmsSenderService>();
            services.AddTransient<IUserProfileService, UserProfileService>();
            services.AddTransient<IOnlineStatusService, OnlineStatusService>();
            services.AddTransient<IChatService, ChatService>();

            return services;
        }
    }
}
