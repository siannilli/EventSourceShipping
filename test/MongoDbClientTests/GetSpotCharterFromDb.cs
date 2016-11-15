using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;

using Shipping.Repositories;


namespace Tests
{
    public class Tests
    {
        private SpotCharterDomain.SpotCharter GetContract()
        {
            var repo = new SpotCharterEventSourceRepository("SpotCharters", "spot_user", "spot_user", host: "sql-db");
            return repo.Get(new SharedShippingDomainsObjects.ValueObjects.SpotCharterId(Guid.Parse("cafbcb44-5176-463f-bf06-e1b519795fab")));
        }

        [Fact]
        public void GetFirstDocument() 
        {
            var repo = new SpotCharterQueryRepository(host: "doc-db", database: "spotService", username: "spotService", password: "spotService");
            var spot = GetContract();

            var doc = repo.GetById(spot.Id);

            Assert.NotNull(doc);

            Assert.Equal(doc.Id, spot.Id);

        }

        [Fact]
        public void InsertDocument()
        {
            var repo = new SpotCharterQueryRepository(host: "doc-db", database: "spotService", username: "spotService", password: "spotService");

            var doc = GetContract();

            repo.Save(doc);
            Assert.NotNull(doc.Id);
        }

        [Fact]
        public void UpdateDocument()
        {
            var repo = new SpotCharterQueryRepository(host: "doc-db", database: "spotService", username: "spotService", password: "spotService");
            var spot = GetContract();

            var doc = repo.GetById(spot.Id);
            doc.ChangeBillOfLading(DateTime.Now, new SharedShippingDomainsObjects.ValueObjects.CargoQuantity("BBL", 40000), "HHHHH");

            repo.Save(doc);

        }

        [Fact]
        public void DeleteDocument()
        {
            var repo = new SpotCharterQueryRepository(host: "doc-db", database: "spotService", username: "spotService", password: "spotService");
            var spot = GetContract();

            var doc = repo.GetById(spot.Id);

            repo.Remove(doc);            

        }

    }
}
