using System;
using Xunit;

using SpotCharterDomain;
using SharedShippingDomainsObjects.ValueObjects;
using Shipping.Repositories;
using EventDispatcherBase;

namespace PostgresSQLRepositoryTests
{
    using TestMethodAttribute = FactAttribute;

    public class PostgresSQLRepositoryTests 
    {

        dynamic dbConfig = new
        {
            host = Environment.GetEnvironmentVariable("SQLDB_HOST") ?? "sql-localdev",
            port = int.Parse(Environment.GetEnvironmentVariable("SQLDB_PORT") ?? "5432"),
            database = Environment.GetEnvironmentVariable("SQLDB_DATABASE") ?? "spotService",
            username = Environment.GetEnvironmentVariable("SQLDB_AUTH_USERNAME") ?? "spotService",
            password = Environment.GetEnvironmentVariable("SQLDB_AUTH_PASSWORD") ?? "spotService"
        };

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

        ISpotCharterCommandRepository repository;
        IEventDispatcherRepository eventsDispatchRepository;

        public PostgresSQLRepositoryTests()
        {
            SpotCharterEventSourceRepository concreteRepository = new SpotCharterEventSourceRepository
                    ("SpotCharters", "spot_user", "spot_user", applicationName: "SpotRepositoryEventTest", host: dbConfig.host, port: dbConfig.port );
            repository = concreteRepository;
            eventsDispatchRepository = concreteRepository;
        }

        private SpotCharterDomain.SpotCharter GetSpotCharter()
        {
            var spot = new SpotCharterDomain.SpotCharter(new SpotCharterDomain.Commands.CreateSpotCharter() { CharterpartyDate = DateTime.Now, CharterpartyId = cpId1, CharterpartyName = counterparty1, VesselId = vesselId, VesselName = vesselName, MinimumQuantity = minimumQuantityStart, Login = login });

            spot.ChangeLaycan( new SpotCharterDomain.Commands.ChangeLaycan() { Login = login, Laycan = laycan });
            spot.ChangeDemurrageRate( new SpotCharterDomain.Commands.ChangeDemurrageRate() { Login = login, DemurrageRate = demurrageRate });

            spot.ChangeCharterparty( new SpotCharterDomain.Commands.ChangeCharterparty() { Login = login, CharterpartyId = cpId2, CharterpartyName = counterparty2 });

            return spot;

        }


        [TestMethod()]
        public void CreateAndRetrieve()
        {
            var spot = GetSpotCharter();

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


        [TestMethod()]
        public void CreateAndRetrieveAndPublishMessages()
        {
            var spot = GetSpotCharter();
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


        [TestMethod]
        public void CreateAndUpdate()
        {
            var spot = GetSpotCharter();

            repository.Save(spot);

            var spotV1 = repository.Get(spot.Id);
            spotV1.ChangeVessel( new SpotCharterDomain.Commands.ChangeVessel() { Login = login, VesselId = new VesselId(Guid.NewGuid()), VesselName = "Pinta" });

            repository.Save(spotV1);

            var spotV2 = repository.Get(spot.Id);
            Assert.Equal(2, spotV2.Version);
            Assert.Equal(spotV2.VesselName, "Pinta");
        }

        [TestMethod]
        public void GetMessageToDispatch()
        {


            Guid eventId = Guid.Empty;
            DispatchEvent @event = null;

            var result = eventsDispatchRepository.GetNextEventToDispatch(out eventId, out @event);

            Assert.True(result);
            Assert.NotSame(eventId, Guid.Empty);
            Assert.NotSame(@event, null);              

        }

        [TestMethod]
        public void GetMessageAndCommitDispatch()
        {

            Guid eventId = Guid.Empty;
            DispatchEvent payload = null;

            var result = eventsDispatchRepository.GetNextEventToDispatch(out eventId, out payload);
            Assert.True(result);

            eventsDispatchRepository.CommitDispatchedEvent(eventId);
            Assert.True(true);

        }
    }
}
