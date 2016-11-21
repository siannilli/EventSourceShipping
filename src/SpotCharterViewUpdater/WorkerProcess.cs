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
        
        // this structure avoids multiple updates of an instance when getting several events for the same version
        // even if the service restarts, the update will happen just one time for the same version

        private readonly Dictionary<SpotCharterId, int> lastVersion = new Dictionary<SpotCharterId, int>(); 

        public WorkerProcess(IEventConsumer consumer,
            ISpotCharterCommandRepository source,
            ISpotCharterUpdateViewRepository destination)
        {

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
                    if (!lastVersion.Keys.Contains(spotId) || lastVersion[spotId] < e.Version)
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
                                Console.WriteLine("Spot {0} updated to version {1}", spotId, spot.Version);
                                break;
                        }
                        Console.WriteLine("[{3:HH:mm:ss}] Event {0}\tSpot Id {1} Version {2} processed.", e.EventName, spotId, e.Version, DateTime.Now);                    
                        lastVersion[spotId] = e.Version;

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

        public void StartConsuming(string queueName)
        {
            this.consumer.StartReceiving(queueName);
        }

        public void Quit()
        {
            this.consumer.Quit();
        }
    }
}
