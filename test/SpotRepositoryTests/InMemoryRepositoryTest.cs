using System;
using Xunit;
using SharedShippingDomainsObjects.ValueObjects;
using SpotCharterDomain;

namespace Tests
{
    using TestMethodAttribute = FactAttribute;

    public class Tests
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

        private SpotCharter GetSpotCharter()
        {
            var spot = new SpotCharterDomain.SpotCharter(DateTime.Now, cpId1, counterparty1, vesselId, vesselName, minimumQuantityStart);

            spot.ChangeLaycan(laycan.From, laycan.To);
            spot.ChangeDemurrageRate(demurrageRate.LoadHoursLaytime, demurrageRate.DischargeHoursLaytime, demurrageRate.TotalHoursLaytime, demurrageRate.Price, demurrageRate.TimeUnit);

            spot.ChangeCharterparty(cpId2, counterparty2);

            return spot;

        }

        [TestMethod]
        public void TestInMemoryRepository()
        {
            var spot = GetSpotCharter();
            ISpotCharterRepository repository = new SpotCharterInMemoryEventSourceRepository.SpotCharterInMemoryEventSourceRepository();

            repository.Save(spot);

            var spot1 = repository.Get(spot.Id);

            Assert.NotNull(spot1);
            Assert.Equal(spot.Id, spot1.Id);
            Assert.Equal(spot.CharterpartyId, spot1.CharterpartyId);
            Assert.Equal(spot.CharterpartyName, spot1.CharterpartyName);
            Assert.Equal(spot.VesselId, spot1.VesselId);
            Assert.Equal(spot.VesselName, spot1.VesselName);
            Assert.NotEqual(spot.Version, spot1.Version);

            Assert.Equal(spot.Version, 0);
            Assert.NotEqual(spot1.Version, 4);

        }

    }
}
