using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects;
using BaseDomainObjects.Aggregates;
using SharedShippingDomainsObjects.ValueObjects;
using SharedShippingDomainsObjects.Entities;
using SharedShippingDomainsObjects.Enums;

using SpotCharterDomain.Events;

namespace SpotCharterDomain
{
    public class SpotCharter : EventSourcedAggregate<SpotCharterId>
    {

        private SpotCharter()
            : base(new SpotCharterId(Guid.Empty))
        {
            this.Handles<BillOfLadingChanged>(OnBillOfLadingChanged);
            this.Handles<DemurrageRateChanged>(OnDemurrageRateChanged);
            this.Handles<FreightRateChanged>(OnFreightRateChanged);
            this.Handles<PortfolioChanged>(OnPortfolioChanged);
            this.Handles<SpotCharterCreated>(OnSpotCharterCreated);
            this.Handles<SpotCharterDeleted>(OnSpotCharterDeleted);
            this.Handles<VesselChanged>(OnVesselChanged);
            this.Handles<CharterpartyChanged>(OnCharterpartyChanged);
            this.Handles<LaycanChanged>(OnLaycanChanged);
        }



        public SpotCharter(SpotCharterId id)
            : this()
        {
            this.Id = id;
        }

        public SpotCharter(DateTime charterpartyDate, CounterpartyId charterpartyId, string charterpartyName, VesselId vesselId, string vesselName,CargoQuantity minimumQuantity)
            : this()
        {
            
            this.UpdateAggregate(new SpotCharterCreated(Guid.NewGuid(), 1, new SpotCharterId(Guid.NewGuid()), charterpartyDate, charterpartyId, charterpartyName, vesselId, vesselName, minimumQuantity));                
        }

        public SpotCharter(IEnumerable<IEvent> events)
            :this()
        {
            SpotCharterCreated firstEvent = events.FirstOrDefault(e => e is SpotCharterCreated) as SpotCharterCreated;
            if (firstEvent == null)
                throw new InvalidOperationException("Missing creation event");

            this.Id = firstEvent.SpotCharterId;
            this.ReplayEvents(events);
        }

        public string Code { get; private set;}
        public DateTime? CharterpartyDate { get; private set; }

        public DateRange Laycan { get; private set; }

        public VesselId VesselId { get; private set; }
        public string VesselName { get; private set; }        

        public CounterpartyId CharterpartyId { get; private set; }
        public string CharterpartyName { get; private set; }
        
        public BillOfLading BillOfLading { get; private set; }

        public CargoQuantity MinimumQuantity { get; private set; }

        public ValueObjects.FreightRate FreightRate { get; private set; }

        public DemurrageRate DemurrageRate { get; private set; }

        public PortfolioId PortfolioId { get; private set; }
        public string PortfolioDescription { get; private set; }

        #region Aggregate Actions

        public void ChangePortfolio(PortfolioId newPortfolio)
        {
            this.UpdateAggregate(new PortfolioChanged(Guid.NewGuid(), this.Version + 1, this.Id, newPortfolio));
        }

        public void ChangeDemurrageRate(double laytimeLoad, 
            double laytimeDischarge, 
            double laytimeTotal,
            CostAmount priceUnit,
            DemurrageRateTimeUnit interval)
        {
            this.UpdateAggregate(new DemurrageRateChanged(Guid.NewGuid(), this.Version + 1 , this.Id, new DemurrageRate(laytimeLoad, laytimeDischarge, laytimeTotal, priceUnit, interval)));
        }

        public void ChangeVessel(VesselId vesselId, string vesselName)
        {
            this.UpdateAggregate(new VesselChanged(Guid.NewGuid(), this.Version + 1, this.Id, vesselId, vesselName));
        }

        public void ChangeCharterparty(CounterpartyId counterpartyId, string counterpartyName)
        {
            this.UpdateAggregate(new CharterpartyChanged(Guid.NewGuid(), this.Version + 1, this.Id, counterpartyId, counterpartyName));
        }

        public void ChangeBillOfLading(DateTime date, CargoQuantity quantity, string documentReference)
        {
            this.UpdateAggregate(new BillOfLadingChanged(Guid.NewGuid(), this.Version + 1, this.Id, date, quantity, documentReference));
        }

        public void  ChangeFreightRate(decimal flat, decimal worldScale, Enums.OverageType overageType, decimal overageValue)
        {
            this.UpdateAggregate(new FreightRateChanged(Guid.NewGuid(), this.Id, this.Version + 1,
                new ValueObjects.FreightRate(flat, worldScale, new ValueObjects.Overage(overageType, overageValue))));
        }

        public void ChangeFreightRate(decimal lumpsum)
        {
            this.UpdateAggregate(new FreightRateChanged(Guid.NewGuid(), this.Id, this.Version + 1, new ValueObjects.FreightRate(lumpsum)));
        }

        public void ChangeFreightRate(decimal price, string uom, Enums.OverageType overageType, decimal overageValue)
        {
            this.UpdateAggregate(new FreightRateChanged(Guid.NewGuid(), this.Id, this.Version + 1, new ValueObjects.FreightRate(price, uom, new ValueObjects.Overage(overageType, overageValue))));
        }

        public void ChangeLaycan(DateTime from, DateTime to)
        {
            this.UpdateAggregate(new LaycanChanged(Guid.NewGuid(), this.Version + 1, this.Id, new DateRange(from, to)));
        }

        #endregion

        #region Domain event handlers

        private void OnSpotCharterCreated(SpotCharterCreated @event)
        {
            this.Id = @event.SpotCharterId;
            this.CharterpartyDate = @event.CharterpartyDate;
            this.CharterpartyId = @event.CounterpartyId;
            this.CharterpartyName = @event.CounterpartyCurrentName;
            this.VesselId = @event.VesselId;
            this.VesselName = @event.VesselCurrentName;
            this.MinimumQuantity  = @event.MinimumQuantity;
        } 
        
        private void OnVesselChanged(VesselChanged @event)
        {
            this.VesselId = @event.VesselId;
            this.VesselName = @event.CurrentName;
        }

        private void OnDemurrageRateChanged(DemurrageRateChanged @event)
        {
            this.DemurrageRate = @event.Rate;
        }
    
        private void OnFreightRateChanged(FreightRateChanged @event)
        {
            this.FreightRate = @event.FreightRate;
        }

        private void OnSpotCharterDeleted(SpotCharterDeleted @event)
        {
            throw new NotImplementedException();
        }

        private void OnPortfolioChanged(PortfolioChanged @event)
        {
            this.PortfolioId = @event.PorfolioId;
        }

        private void OnBillOfLadingChanged(BillOfLadingChanged @event)
        {
            this.BillOfLading = new BillOfLading(@event.Date, @event.Quantity, @event.DocumentReference);
        }


        private void OnCharterpartyChanged(CharterpartyChanged @event)
        {
            this.CharterpartyId = @event.CharterpartyId;
            this.CharterpartyName = @event.CurrentName;
        }

        private void OnLaycanChanged(LaycanChanged @event)
        {
            this.Laycan = @event.Laycan; ;
        }

        #endregion

    }
}
