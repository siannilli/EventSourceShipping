using System;
using Xunit;

using SharedShippingDomainsObjects.ValueObjects;
using SpotCharterDomain.Events;

using EventDispatcherBase;
using RabbitMQEventDispatcher;
using Newtonsoft.Json;

namespace Tests
{
    public class DispatchBasicEventTest
    {
        [Fact]
        public void DispatchSpotCreated()
        {

            using (IEventDispatcher dispatcher = new RabbitMQEventDispatcher.RabbitMQEventDispatcher("message-broker",
                exchangeName: "chartering.spot",
                username: "siannilli",
                password: "siannilli",
                vhost: "/test"
                ))
            {

                var spotCreated = new SpotCharterCreated(Guid.NewGuid(), 1, new SpotCharterId(Guid.NewGuid()), DateTime.Now, new CounterpartyId(Guid.NewGuid()), "Counterparty", new VesselId(Guid.NewGuid()), "Vessel", new CargoQuantity("MT", 5000));
                var freight = new FreightRateChanged(Guid.NewGuid(), new SpotCharterId(Guid.NewGuid()), 1, new SpotCharterDomain.ValueObjects.FreightRate(100000));

                BaseDomainObjects.IEvent @event = spotCreated;

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
