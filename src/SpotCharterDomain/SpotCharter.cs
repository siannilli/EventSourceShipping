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
using SpotCharterDomain.Commands;

namespace SpotCharterDomain
{
    public class SpotCharter : EventSourcedAggregate<SpotCharterId>
    {
        #region Costructors

        private SpotCharter()
            : base(new SpotCharterId(Guid.Empty))
        {
            // declares all the (domain) events handled
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

        public SpotCharter(CreateSpotCharter command)
            : this()
        {                           
            this.UpdateAggregate(new SpotCharterCreated(command.SpotCharterId, command.Login, command.CharterpartyDate, command.CharterpartyId, command.CharterpartyName, command.VesselId, command.VesselName, command.MinimumQuantity));                
        }

        public SpotCharter(IEnumerable<IEvent<SpotCharterId>> events)
            :this()
        {
            SpotCharterCreated firstEvent = events.FirstOrDefault(e => e is SpotCharterCreated) as SpotCharterCreated;
            if (firstEvent == null)
                throw new InvalidOperationException("Missing creation event"); 

            this.Id = firstEvent.AggregateId;
            this.ReplayEvents(events);
        }

        #endregion

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


        #region Aggregate Command handlers

        public void ChangePortfolio(ChangePortfolio command)
        {
            UpdateAggregate(new PortfolioChanged(this.Id, command.Login,  command.Version + 1, command.PortfolioId, command.PortfolioName));
        }

        public void ChangeDemurrageRate(ChangeDemurrageRate command)
        {
            this.UpdateAggregate(new DemurrageRateChanged(this.Id, command.Login, command.Version + 1  , command.DemurrageRate));
        }

        public void ChangeVessel(ChangeVessel command)
        {
            this.UpdateAggregate(new VesselChanged(this.Id, command.Login,  command.Version + 1, command.VesselId, command.VesselName));
        }

        public void ChangeCharterparty(ChangeCharterparty command)
        {
            this.UpdateAggregate(new CharterpartyChanged(this.Id, command.Login, command.Version + 1, command.CharterpartyId, command.CharterpartyName));
        }

        public void ChangeBillOfLading(ChangeBillOfLading command)
        {
            this.UpdateAggregate(new BillOfLadingChanged(this.Id, command.Login, command.Version + 1, command.BillOfLading));
        }
        public void ChangeFreightRate(ChangeFreightRate command)
        {
            throw new NotImplementedException();
        }

        public void ChangeLaycan(ChangeLaycan command)
        {
            this.UpdateAggregate(new LaycanChanged(this.Id, command.Login, command.Version + 1, command.Laycan));
        }

        #endregion

        #region Domain event handlers

        private void OnSpotCharterCreated(SpotCharterCreated @event)
        {
            this.Id = @event.AggregateId;
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
            this.BillOfLading = @event.BillOfLading;
        }


        private void OnCharterpartyChanged(CharterpartyChanged @event)
        {
            this.CharterpartyId = @event.CharterpartyId;
            this.CharterpartyName = @event.CurrentName;
        }

        private void OnLaycanChanged(LaycanChanged @event)
        {
            this.Laycan = @event.Laycan;
        }

        #endregion

    }
}
