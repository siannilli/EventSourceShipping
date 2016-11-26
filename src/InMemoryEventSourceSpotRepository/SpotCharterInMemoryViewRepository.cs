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

            if (this.consumer != null)
                this.consumer.ReceivedEventHandler += Consumer_ReceivedEventHandler;

            this.commandRepository = commandRepository;

            if (this.commandRepository == null) // run in a separate process, fill repository with some example data
                this.FillRepositoryWithSampleCharters(10);

        }

        private void FillRepositoryWithSampleCharters(int numberOfInstances)
        {
            for (int i = 0; i < numberOfInstances; i++)
            {
                var spotId = new SpotCharterId(Guid.NewGuid());
                this.repository.Add(spotId, new SpotCharterView
                {
                    BillOfLading = new BillOfLading(DateTime.Now, new CargoQuantity("MT", 45000), null),
                    CharterpartyDate = DateTime.Now,
                    CharterpartyId = new CounterpartyId(Guid.NewGuid()),
                    CharterpartyName = $"Charterparty{i}",
                    Laycan = new DateRange(DateTime.Now, DateTime.Now.AddDays(5)),
                    MinimumQuantity = new CargoQuantity("MT", 40000),
                    VesselId = new VesselId(Guid.NewGuid()),
                    VesselName = $"Vessel{i}",
                    PortfolioId = new PortfolioId(Guid.NewGuid()),
                    PortfolioDescription = $"Portfolio{i}",
                    Version = 1,
                    Id = spotId,
                    LastUpdate = DateTime.Now,
                    DemurrageRate = new DemurrageRate(0, 0, 72, new CostAmount(new Currency("USD", "US Dollars", "$"), 25000), SharedShippingDomainsObjects.Enums.DemurrageRateTimeUnit.Day),

                });
            }
        }

        private void Consumer_ReceivedEventHandler(DispatchEvent @event)
        {
            
            dynamic eventPayload = JsonConvert.DeserializeObject(@event.Payload);
            SpotCharterId spotId = new SpotCharterId(Guid.Parse(eventPayload.AggregateId.Value.ToString()));

            if (this.commandRepository != null)
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

        public IQueryable<SpotCharterView> ChartersMissingFreightRate()
        {
            throw new NotImplementedException();
        }

        public IQueryable<SpotCharterView> ChartersMissingDemurrageTerms()
        {
            throw new NotImplementedException();
        }

        public IQueryable<SpotCharterView> ChartersMissingPortfolio()
        {
            throw new NotImplementedException();
        }

        public IQueryable<SpotCharterView> ScheduledCharters()
        {
            throw new NotImplementedException();
        }
    }
}
