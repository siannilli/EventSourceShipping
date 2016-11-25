using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;

using SpotCharterViewModel;
using Shipping.Repositories;
using SharedShippingDomainsObjects.ValueObjects;

namespace MongoDBClientTests
{
    public class Tests
    {

        dynamic config = new
        {
            host = Environment.GetEnvironmentVariable("DOCDB_HOST") ??  "docdb-localdev",
            port = int.Parse(Environment.GetEnvironmentVariable("DOCDB_HOST_PORT") ?? "27017"),
            database = Environment.GetEnvironmentVariable("DOCDB_COLLECTION") ?? "spotService",
            username = Environment.GetEnvironmentVariable("DOCDB_AUTH_USERNAME") ?? "spotService",
            password = Environment.GetEnvironmentVariable("DOCDB_AUTH_PASSWORD") ?? "spotService"
        };
       

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

        private SpotCharterViewModel.SpotCharterView GetSpotCharter()
        {
            return new SpotCharterView
            {
                Id = spotId,
                CharterpartyName = counterparty1,
                CharterpartyId = cpId1,
                VesselName = vesselName, 
                Laycan = laycan,
                VesselId = vesselId,
                Version = 1,
                LastUpdate = DateTime.Now,
            };

        }
       

        [Fact]
        public void InsertAndDelete()
        {
            var repo = new SpotCharterQueryRepository(host: config.host, database: config.database, username: config.username, password: config.password );
            var spot = GetSpotCharter();

            repo.Save(spot);
            var doc = repo.GetById(spot.Id);

            repo.Remove(doc);            

        }

    }
}
