using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessengerSY.AppConfiguration;
using MessengerSY.Core.JwtAuthOptions;
using MessengerSY.Hubs;
using MessengerSY.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MessengerSY
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIISConfiguration();
            services.AddRedisDbContext();
            services.AddSignalRConfiguration();
            services.AddSwaggerConfiguration();
            services.AddDbContextConfiguration(Configuration);
            services.AddMemoryCache();
            services.AddAppServicesConfiguration();
            services.AddSMSProviderConfiguration();
            services.AddAuthConfiguration();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMiddleware<SetNameIdentifier>();
            app.UseMiddleware<JwtBlockValidator>();
            app.UseSignalR(routes =>
            {
                routes.MapHub<Main>("/hubs/main");
            });
            app.UseMvc();
        }
    }
}
