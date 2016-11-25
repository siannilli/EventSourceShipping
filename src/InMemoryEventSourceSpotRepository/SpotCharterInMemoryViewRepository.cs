using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SharedShippingDomainsObjects.ValueObjects;
using SpotCharterInMemoryEventSourceRepository;
using SpotCharterViewModel;

using AutoMapper;
using SpotCharterDomain;
using EventDispatcherBase;
using Newtonsoft.Json;

namespace SpotCharterInMemoryEventSourceRepository
{
    public class SpotCharterInMemoryViewRepository : ISpotCharterQueryRepository, ISpotCharterUpdateViewRepository
    {
        private readonly IDictionary<SpotCharterId, SpotCharterView> repository = new Dictionary<SpotCharterId, SpotCharterView>();
        private readonly SpotCharterDomain.ISpotCharterCommandRepository commandRepository;
        private readonly IEventConsumer consumer;

        public SpotCharterInMemoryViewRepository(IEventConsumer consumer,  ISpotCharterCommandRepository  commandRepository)
        {
            // Define mapping rule between Command and Query models
            Mapper.Initialize(cfg => {
                cfg.CreateMap<SpotCharterDomain.ValueObjects.FreightRate, string>().ConstructUsing((rate, ctx) => rate?.ToString() ?? null);
                cfg.CreateMap<SpotCharter, SpotCharterView>()
                    .AfterMap((src, dest) => { dest.LastUpdate = DateTime.Now; });
            });
            this.consumer = consumer;
            this.consumer.ReceivedEventHandler += Consumer_ReceivedEventHandler;
            this.commandRepository = commandRepository;
        }

        private void Consumer_ReceivedEventHandler(DispatchEvent @event)
        {
            dynamic eventPayload = JsonConvert.DeserializeObject(@event.Payload);
            SpotCharterId spotId = new SpotCharterId(Guid.Parse(eventPayload.AggregateId.Value.ToString()));

            this.repository[spotId] = Mapper.Map<SpotCharterView>(this.commandRepository.Get(spotId));
        }

        public IQueryable<SpotCharterView> Find()
        {
            return this.repository.Values.AsQueryable();
        }

        public IQueryable<SpotCharterView> Find(Expression<Func<SpotCharterView, bool>> predicate)
        {
            return this.Find().Where(predicate);
        }

        public SpotCharterView GetById(SpotCharterId id)
        {
            SpotCharterView ret = null;
            this.repository.TryGetValue(id, out ret);

            return ret;
        }

        public SpotCharterView GetBySpotCharterId(SpotCharterId spotId)
        {
            return this.GetById(spotId);
        }

        public void Remove(SpotCharterId id)
        {
            this.repository.Remove(id); 
        }

        public void Remove(SpotCharterView spot)
        {
            this.Remove(spot.Id);
        }

        public void Save(SpotCharterView spot)
        {
            this.repository[spot.Id] = spot;
        }
    }
}
