using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using BaseDomainObjects;
using SharedShippingDomainsObjects.ValueObjects;

using SpotCharterDomain;
using SpotCharterDomain.Events;

namespace SpotDomainTests
{

    using TestMethodAttribute = FactAttribute;
    public class AggregateTests
    {
        SpotCharterId spotId = new SpotCharterId(Guid.NewGuid());

        CounterpartyId cpId1 = new CounterpartyId(Guid.NewGuid());
        CounterpartyId cpId2 = new CounterpartyId(Guid.NewGuid());

        string counterparty1 = "VITOL";
        string counterparty2 = "FRATELLI SERPENTI";

        VesselId vesselId = new VesselId(Guid.NewGuid());
        string vesselName = "Santa maria";

        CargoQuantity minimumQuantityStart = new CargoQuantity("MT", 85000);

        DateRange laycan = new DateRange(DateTime.Now.AddDays(3).Date, DateTime.Now.Date);
        DemurrageRate demurrageRate = new DemurrageRate(0, 0, 72, new CostAmount(new Currency("USD", "US DOLLAR", "$"), 25000), SharedShippingDomainsObjects.Enums.DemurrageRateTimeUnit.Day);

        private SpotCharterDomain.SpotCharter GetSpotCharter()
        {
            var spot = new SpotCharterDomain.SpotCharter(DateTime.Now, cpId1, counterparty1, vesselId, vesselName, minimumQuantityStart);

            spot.ChangeLaycan(laycan.From, laycan.To);
            spot.ChangeDemurrageRate(demurrageRate.LoadHoursLaytime, demurrageRate.DischargeHoursLaytime, demurrageRate.TotalHoursLaytime, demurrageRate.Price, demurrageRate.TimeUnit);

            spot.ChangeCharterparty(cpId2, counterparty2);

            return spot;

        }

        [TestMethod]
        public void CreateAggregateFromUpdates()
        {
            var spot = GetSpotCharter();

            Assert.Equal(spot.DemurrageRate, demurrageRate);
            Assert.Equal(spot.VesselName, vesselName);
            Assert.Equal(spot.Laycan, laycan);

            Assert.Equal(spot.Version, 0);

        }


        [TestMethod]
        public void CreateAggregateFromEventStream()
        {
            var eventStream = new List<IEvent<SpotCharterId>>()
        {
            new SpotCharterCreated( spotId, 1, DateTime.Now, cpId1, counterparty1, vesselId, vesselName, minimumQuantityStart),
            new LaycanChanged(spotId, 2, laycan ),
            new DemurrageRateChanged(spotId, 3, demurrageRate),
            new CharterpartyChanged(spotId,  4, cpId2, counterparty2),
        };

            var spot = new SpotCharterDomain.SpotCharter(eventStream);

            Assert.Equal(spot.DemurrageRate, demurrageRate);
            Assert.Equal(spot.VesselName, vesselName);
            Assert.Equal(spot.Laycan, laycan);

            Assert.Equal(spot.Version, 4);

        }

    }
}
