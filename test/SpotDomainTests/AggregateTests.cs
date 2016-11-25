using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using BaseDomainObjects;
using SharedShippingDomainsObjects.ValueObjects;

using SpotCharterDomain;
using SpotCharterDomain.Events;
using BaseDomainObjects.Entities;

namespace SpotCharterDomainObjectTests
{

    using TestMethodAttribute = FactAttribute;
    public class AggregateTests
    {

        Login login = new Login("stefano");
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
            var spot = new SpotCharterDomain.SpotCharter(new SpotCharterDomain.Commands.CreateSpotCharter()
            {
                Login = login,
                CharterpartyDate = DateTime.Now,
                CharterpartyId = cpId1,
                CharterpartyName = counterparty1,
                VesselId = vesselId,
                VesselName = vesselName,
                MinimumQuantity = minimumQuantityStart
            });

            spot.ChangeLaycan(new SpotCharterDomain.Commands.ChangeLaycan() { Login = login, Laycan = laycan });
            spot.ChangeDemurrageRate(new SpotCharterDomain.Commands.ChangeDemurrageRate() { Login = login, DemurrageRate = demurrageRate });

            spot.ChangeCharterparty(new SpotCharterDomain.Commands.ChangeCharterparty() { Login = login, CharterpartyId = cpId2, CharterpartyName = counterparty2 });

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
            new SpotCharterCreated(spotId, login, DateTime.Now, cpId1, counterparty1, vesselId, vesselName, minimumQuantityStart),
            new LaycanChanged(spotId, login, 2, laycan ),
            new DemurrageRateChanged(spotId, login, 3, demurrageRate),
            new CharterpartyChanged(spotId, login, 4, cpId2, counterparty2),
        };

            var spot = new SpotCharterDomain.SpotCharter(eventStream);

            Assert.Equal(spot.DemurrageRate, demurrageRate);
            Assert.Equal(spot.VesselName, vesselName);
            Assert.Equal(spot.Laycan, laycan);

            Assert.Equal(spot.Version, 4);

        }

    }
}
