using System;
using Xunit;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomainObjectTests
{
    public class ValueObjectTests
    {
        [Fact]
        public void TestCargoQuantityEquality() 
        {
            var qty1 = new CargoQuantity("MT", 10000);
            var qty2 = new CargoQuantity("MT", 10000);
            var qty3 = new CargoQuantity("M3", 10000);
            var qty4 = new CargoQuantity("MT", 10001);

            Assert.Equal<CargoQuantity>(qty1, qty2);

            Assert.NotEqual(qty1, qty3);
            Assert.NotEqual(qty2, qty4);
        }

        [Fact]
        public void TestSpotCharterIdEquality()
        {
            var spotId = new SpotCharterId(Guid.NewGuid());
            var spotId1 = new SpotCharterId(spotId.Value);

            Assert.NotEqual(spotId, null);

            Assert.False(Object.ReferenceEquals(spotId, spotId1));
            Assert.Equal(spotId, spotId1);

            var spotId2 = new SpotCharterId(Guid.NewGuid());

            Assert.NotEqual(spotId, spotId2);

        }

        [Fact]
        public void TestNotOperatorOverload()
        {
            SpotCharterId spotId1 = null;
            SpotCharterId spotId2 = null;

            Assert.True(spotId1 == spotId2);
            Assert.False(spotId1 != spotId2);

            Assert.Equal(spotId1, spotId2);

            spotId1 = new SpotCharterId(Guid.NewGuid());
            spotId2 = new SpotCharterId(Guid.NewGuid());

            Assert.False(spotId1 == spotId2);
            Assert.True(spotId1 != spotId2);

            Assert.NotEqual(spotId1, spotId2);

            spotId1 = new SpotCharterId(Guid.NewGuid());
            spotId2 = null;

            Assert.False(spotId1 == spotId2);
            Assert.True(spotId1 != spotId2);

            Assert.NotEqual(spotId1, spotId2);

            spotId1 = new SpotCharterId(Guid.Empty);
            spotId2 = new SpotCharterId(Guid.Empty);

            Assert.True(spotId1 == spotId2);
            Assert.False(spotId1 != spotId2);

            Assert.Equal(spotId1, spotId2);


        }


    }
}
