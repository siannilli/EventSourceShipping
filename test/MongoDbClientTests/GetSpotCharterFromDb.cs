using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;

using SpotCharterViewModel;
using Shipping.Repositories;
using SharedShippingDomainsObjects.ValueObjects;

namespace Tests
{
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
        public void GetFirstDocument() 
        {
            var repo = new SpotCharterQueryRepository(host: "localdev", database: "spotService" , username: "spotService", password: "spotService");            

            var doc = repo.Find().Take(1).FirstOrDefault();

            Assert.NotNull(doc);

        }

        [Fact]
        public void InsertAndRetrieve()
        {
            var repo = new SpotCharterQueryRepository(host: "localdev", database: "spotService", username: "spotService", password: "spotService");

            var doc = GetSpotCharter();
            repo.Save(doc);

            var doc1 = repo.GetBySpotCharterId(doc.Id);

            Assert.Equal(doc1.Id, doc.Id);
            Assert.NotEqual(doc.Version, 0);
        }

        [Fact]
        public void InsertDocument()
        {
            var repo = new SpotCharterQueryRepository(host: "doc-db", database: "spotService", username: "spotService", password: "spotService");

            var doc = GetSpotCharter();

            repo.Save(doc);
            Assert.NotNull(doc.Id);
        }


        [Fact]
        public void DeleteDocument()
        {
            var repo = new SpotCharterQueryRepository(host: "localdev", database: "spotService", username: "spotService", password: "spotService");
            var spot = GetSpotCharter();

            repo.Save(spot);
            var doc = repo.GetById(spot.Id);

            repo.Remove(doc);            

        }

    }
}
