using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Events;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    public class FreightRateChanged: Event
    {
        public FreightRateChanged(Guid eventId, SpotCharterId spotId, int version, ValueObjects.FreightRate freightRate)
            : base(eventId, version)
        {
            this.SpotCharterId = spotId;
        }

        public SpotCharterId SpotCharterId { get; private set; }
        public ValueObjects.FreightRate FreightRate { get; private set; }

    }
}
