using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SpotCharterViewModel;
using Shipping.Repositories;
using SharedShippingDomainsObjects.ValueObjects;

using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;
using System.Security.Claims;

namespace SpotServiceQueryAPI
{
    public class Startup
    {
        ISpotCharterQueryRepository queryRepository;
        private SecurityKey jwtSigningKey;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            if (env.IsDevelopment())
            {
                queryRepository = new SpotCharterInMemoryEventSourceRepository.SpotCharterInMemoryViewRepository(null, null);
            }
            else
            {
                queryRepository = new SpotCharterQueryRepository(
                    host: Configuration.GetSection("DocumentStore")["host"],
                    database: Configuration.GetSection("DocumentStore")["database"],
                    username: Configuration.GetSection("DocumentStore")["login"],
                    password: Configuration.GetSection("DocumentStore")["password"]);            
            }
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            // Add singleton repository instance for document database with view model instances
            services.AddSingleton<ISpotCharterQueryRepository>((provider) => queryRepository);
           
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();            

            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();

            app.UseJwtBearerAuthentication(new JwtBearerOptions()
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtSigningKey,

                    ValidateIssuer = true,
                    ValidIssuer = "user-management",

                    ValidateLifetime = true,

                }
            });

            if (env.IsDevelopment()) // fixed authentication
            {
                // add a middleware that setup a generic authentication claim
                app.Use(async (context, next) =>
                {
                    var identity = new GenericIdentity("devuser", ClaimsIdentity.DefaultIssuer);
                    var principal = new GenericPrincipal(identity, new string[] { "charterer" });

                    context.User = new ClaimsPrincipal(principal);

                    await next.Invoke();
                });
            }


            app.UseMvc();
        }
    }
}
