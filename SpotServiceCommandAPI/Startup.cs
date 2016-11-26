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
using SpotCharterDomain.Commands;
using SpotCharterViewModel;

namespace SpotServiceCommandAPI
{
    public class Startup
    {
        IEventDispatcher messageDispatcher = null;
        ISpotCharterCommandRepository commandRepository = null;

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
                var messageBroker = new SpotCharterInMemoryEventSourceRepository.InProcessEventDispatcher();
                messageDispatcher = messageBroker;

                commandRepository = new SpotCharterInMemoryEventSourceRepository.SpotCharterInMemoryCommandRepository(messageDispatcher);

            }
            else
            {
                // configuration settings for db and message broker
                var databaseConfig = Configuration.GetSection("EventSourceDatabase");
                var messageBrokerConfig = Configuration.GetSection("MessageBroker");

                messageDispatcher = new RabbitMQEventDispatcher.RabbitMQEventDispatcher(
                        host: messageBrokerConfig["host"],
                        vhost: messageBrokerConfig["vhost"],
                        port: int.Parse(messageBrokerConfig["port"]),
                        username: messageBrokerConfig["username"],
                        password: messageBrokerConfig["password"],
                        exchangeName: messageBrokerConfig["exchangeName"]);

                commandRepository = new SpotCharterEventSourceRepository(
                    database: databaseConfig["database"],
                    login: databaseConfig["login"],
                    password: databaseConfig["password"],
                    applicationName: databaseConfig["applicationName"],
                    host: databaseConfig["host"],
                    port: int.Parse(databaseConfig["port"]),
                    messageBroker: messageDispatcher
                    );

            }

        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            // defines service instance for command handlers and inject repository and message broker instances
            var spotService = new SpotCharterDomain.SpotCharterCommandHandler(commandRepository);

            // define singleton services for command handlers (all pointing to the same implementation instance)
            services.AddSingleton<ICommandHandler<SpotCharterDomain.Commands.CreateSpotCharter>>(spotService);
            services.AddSingleton<ICommandHandler<SpotCharterDomain.Commands.ChangeVessel>>(spotService);
            services.AddSingleton<ICommandHandler<SpotCharterDomain.Commands.ChangeCharterparty>>(spotService);
            services.AddSingleton<ICommandHandler<SpotCharterDomain.Commands.ChangeBillOfLading>>(spotService);
            services.AddSingleton<ICommandHandler<SpotCharterDomain.Commands.ChangeDemurrageRate>>(spotService);
            services.AddSingleton<ICommandHandler<SpotCharterDomain.Commands.ChangeLaycan>>(spotService);
            services.AddSingleton<ICommandHandler<SpotCharterDomain.Commands.ChangePortfolio>>(spotService);
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
