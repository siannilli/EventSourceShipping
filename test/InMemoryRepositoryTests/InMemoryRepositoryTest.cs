using System;
using Xunit;
using SharedShippingDomainsObjects.ValueObjects;
using SpotCharterDomain;
using System.Linq;

using SpotCharterInMemoryEventSourceRepository;

namespace InMemoryRepositoryTests
{
    using TestMethodAttribute = FactAttribute;

    public class Tests
    {
        BaseDomainObjects.Entities.Login login = new BaseDomainObjects.Entities.Login("stefano");
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

        private SpotCharter GetNewSpotCharterInstance()
        {
            var spot = new SpotCharterDomain.SpotCharter(new SpotCharterDomain.Commands.CreateSpotCharter() { SpotCharterId = spotId, CharterpartyDate = DateTime.Now, CharterpartyId = cpId1, CharterpartyName = counterparty1, VesselId = vesselId, VesselName = vesselName, MinimumQuantity = minimumQuantityStart, Login = login });

            spot.ChangeLaycan(new SpotCharterDomain.Commands.ChangeLaycan() { Laycan = laycan, Login = login });
            spot.ChangeDemurrageRate(new SpotCharterDomain.Commands.ChangeDemurrageRate() { Login = login, DemurrageRate = demurrageRate });

            spot.ChangeCharterparty( new SpotCharterDomain.Commands.ChangeCharterparty() { Login = login, CharterpartyId = cpId2, CharterpartyName = counterparty2 });

            return spot;
        }

        [TestMethod]
        public void TestInMemoryRepository()
        {
            var spot = GetNewSpotCharterInstance();
            ISpotCharterCommandRepository repository = new SpotCharterInMemoryCommandRepository();

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

        [Fact]
        public void TestCommandQuery()
        {
            var spot = GetNewSpotCharterInstance();
            var messageBroker = new SpotCharterInMemoryEventSourceRepository.InProcessEventDispatcher();

            var cmdRepository = new SpotCharterInMemoryCommandRepository(messageBroker);
            ISpotCharterCommandRepository commandRepository = cmdRepository;
            SpotCharterViewModel.ISpotCharterQueryRepository queryRepository = new SpotCharterInMemoryViewRepository(messageBroker, cmdRepository);

            commandRepository.Save(spot);

            var spotView = queryRepository.GetBySpotCharterId(spot.Id);

            Assert.NotNull(spotView);
            Assert.Equal(spot.VesselId, spotView.VesselId);
            Assert.Equal(spot.Laycan, spotView.Laycan);

        }

    }
}
