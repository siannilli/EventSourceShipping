using System;
using Xunit;

using SharedShippingDomainsObjects.ValueObjects;
using SpotCharterDomain.Events;

using EventDispatcherBase;
using RabbitMQEventDispatcher;
using Newtonsoft.Json;

namespace RabbitMQDispatcherTests
{
    public class DispatchBasicEventTest
    {

        dynamic rabbitConfig = new
        {
            host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "msgbrk-localdev",
            port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_HOST_PORT") ?? "5672"),
            vhost = Environment.GetEnvironmentVariable("RABBITMQ_VHOST") ?? "/dev",
            exchange = Environment.GetEnvironmentVariable("RABBITMQ_EXCHANGE") ?? "chartering.spot",
            username = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME"),
            password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")
        };

        [Fact]
        public void DispatchSpotCreated()
        {

            using (IEventDispatcher dispatcher = new RabbitMQEventDispatcher.RabbitMQEventDispatcher(
                host: rabbitConfig.host, 
                exchangeName: rabbitConfig.exchange,
                username: rabbitConfig.username ,
                password: rabbitConfig.password,
                vhost: rabbitConfig.vhost,
                port: rabbitConfig.port
                ))
            {

                var spotCreated = new SpotCharterCreated(new SpotCharterId(Guid.NewGuid()), new BaseDomainObjects.Entities.Login("stefano"), DateTime.Now, new CounterpartyId(Guid.NewGuid()), "Counterparty", new VesselId(Guid.NewGuid()), "Vessel", new CargoQuantity("MT", 5000));
                var freight = new FreightRateChanged(new SpotCharterId(Guid.NewGuid()), new BaseDomainObjects.Entities.Login("stefano"), 1, new SpotCharterDomain.ValueObjects.FreightRate(100000));

                BaseDomainObjects.IEvent<SpotCharterId> @event = spotCreated;

                var dispatchEvent = new DispatchEvent()
                {
                    EventName = @event.EventName,
                    Id = @event.Id,
                    Source = @event.Source,
                    Version = @event.Version,
                    Timestamp = spotCreated.CharterpartyDate,
                    Payload = JsonConvert.SerializeObject(spotCreated),
                };

                dispatcher.Publish(dispatchEvent);

                @event = freight;
                dispatcher.Publish(new DispatchEvent()
                {
                    EventName = @event.EventName,
                    Id = @event.Id,
                    Source = @event.Source,
                    Version = @event.Version,
                    Timestamp = DateTime.Now,
                    Payload = JsonConvert.SerializeObject(freight),

                });
            }
        }
    }
}
