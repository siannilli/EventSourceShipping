using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SpotCharterDomain;
using BaseDomainObjects;
using BaseDomainObjects.Commands;
using Shipping.Repositories;
using EventDispatcherBase;
using RabbitMQEventDispatcher;
using SpotCharterServices.Commands;

namespace SpotServiceCommandAPI
{
    public class Startup
    {
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
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            var databaseConfig = Configuration.GetSection("EventSourceDatabase");
            var messageBroker = Configuration.GetSection("MessageBroker");

            // Add reference to EventSourceRepository (and message broker)
            var spotService = new SpotCharterServices.SpotCharterCommandHandler(new SpotCharterEventSourceRepository(
                database: databaseConfig["database"],
                login: databaseConfig["login"],
                password: databaseConfig["password"],
                applicationName: databaseConfig["applicationName"],
                host: databaseConfig["host"],
                port: int.Parse(databaseConfig["port"]),
                messageBroker: new RabbitMQEventDispatcher.RabbitMQEventDispatcher(
                    host: messageBroker["host"], 
                    vhost: messageBroker["vhost"],
                    port: int.Parse(messageBroker["port"]), 
                    username: messageBroker["username"], 
                    password: messageBroker["password"],
                    exchangeName: messageBroker["exchangeName"])                 
                ));

            // define singleto services for command handlers
            services.AddSingleton<ICommandHandler<SpotCharterServices.Commands.CreateSpotCharter>>(spotService);
            services.AddSingleton<ICommandHandler<SpotCharterServices.Commands.ChangeVessel>>(spotService);
            services.AddSingleton<ICommandHandler<SpotCharterServices.Commands.ChangeCharterparty>>(spotService);
            services.AddSingleton<ICommandHandler<SpotCharterServices.Commands.ChangeBillOfLading>>(spotService);
            services.AddSingleton<ICommandHandler<SpotCharterServices.Commands.ChangeDemurrageRate>>(spotService);
            services.AddSingleton<ICommandHandler<SpotCharterServices.Commands.ChangeLaycan>>(spotService);
            services.AddSingleton<ICommandHandler<SpotCharterServices.Commands.ChangePortfolio>>(spotService);
            // services.AddSingleton<ICommandHandler<SpotCharterServices.Commands.ChangeFreightRate>>(spotService);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseMvc();
        }
    }
}
