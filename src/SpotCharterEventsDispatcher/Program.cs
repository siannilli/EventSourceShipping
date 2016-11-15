using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SpotCharterDomain;
using EventDispatcherBase;
using Shipping.Repositories;
using RabbitMQEventDispatcher;

namespace SpotCharterEventsDispatcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IEventDispatcherRepository repository = new SpotCharterEventSourceRepository("SpotCharters", "spot_user", "spot_user", applicationName: "EventDispatcher", host: "sql-db");
            using (IEventDispatcher dispatcher = new RabbitMQEventDispatcher.RabbitMQEventDispatcher("message-broker", "/test", username: "siannilli", password: "siannilli", exchangeName: "chartering.spot"))
            {
                Guid eventId = Guid.Empty;
                DispatchEvent eventToDispatch = null;

                try
                {
                    while (repository.GetNextEventToDispatch(out eventId, out eventToDispatch))
                    {
                        dispatcher.Publish(eventToDispatch);
                        repository.CommitDispatchedEvent(eventId);
                    }

                }
                catch (Exception ex)
                {
                    System.Console.Error.WriteLine(ex);
                }

            }
        }
    }
}
