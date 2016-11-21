using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EventDispatcherBase;
using SpotCharterDomain;
using SpotCharterViewModel;
using Newtonsoft.Json;
using SharedShippingDomainsObjects.ValueObjects;

using AutoMapper;

namespace SpotCharterViewUpdater
{
    public class WorkerProcess
    {
        private readonly IEventConsumer consumer;
        private readonly ISpotCharterCommandRepository source;
        private readonly ISpotCharterUpdateViewRepository destination;
        
        // this structure avoids multiple updates of an instance when getting multiple events for the same version
        // if the service restarts, the update will happen one time for the same entity version
        private readonly Dictionary<SpotCharterId, int> lastVersionsDictionary = new Dictionary<SpotCharterId, int>(); 

        public WorkerProcess(IEventConsumer consumer,
            ISpotCharterCommandRepository source,
            ISpotCharterUpdateViewRepository destination)
        {

            // Define mapping rule between Command and Query models
            Mapper.Initialize(cfg => {
                cfg.CreateMap<SpotCharterDomain.ValueObjects.FreightRate, string>().ConstructUsing((rate, ctx) => rate?.ToString() ?? null);
                cfg.CreateMap<SpotCharter, SpotCharterView>()
                    .AfterMap((src, dest) => { dest.LastUpdate = DateTime.Now; });
            });

            this.source = source;
            this.destination = destination;

            this.consumer = consumer;

            this.consumer.ReceivedEventHandler += (e) =>
            {
                try
                {
                    dynamic eventPayload = JsonConvert.DeserializeObject(e.Payload);
                    var spotId = new SpotCharterId(Guid.Parse(eventPayload.AggregateId.Value.ToString()));

                    // check if last version has been updated already         
                    if (!lastVersionsDictionary.Keys.Contains(spotId) || lastVersionsDictionary[spotId] < e.Version)
                    {
                        switch (e.EventName)
                        {
                            case "SpotCharterDeleted":
                                destination.Remove(spotId);
                                Console.WriteLine("Spot {0} removed.", spotId);
                                break;
                            default:
                                var spot = source.Get(spotId);

                                var spotView = Mapper.Map<SpotCharterView>(spot);

                                destination.Save(spotView);
                                lastVersionsDictionary[spotId] = spot.Version; // put the version from the entity, not from the processing event

                                Console.WriteLine("Spot {0} updated to version {1}", spotId, spot.Version);
                                break;
                        }

                        Console.WriteLine("[{3:HH:mm:ss}] Event {0}\tSpot Id {1} Version {2} processed.", e.EventName, spotId, e.Version, DateTime.Now);                    

                    }
                    else
                    {
                        Console.WriteLine("SpotId : {0} Version {1} already updated. Skipping.", spotId, e.Version);
                    }
                    consumer.AckReceived(e.Id);

                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            };
        }

        public void Start(string queueName)
        {
            this.consumer.StartConsumingEvents(queueName);
        }

        public void Quit()
        {
            this.consumer.Quit();
        }
    }
}
