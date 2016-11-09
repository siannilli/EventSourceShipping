using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Events;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    public class LaycanChanged: Event
    {
        public LaycanChanged(Guid eventId, int version, SpotCharterId spotId, DateRange laycan)
            : base(eventId, version)
        {
            this.SpotCharterId = spotId;
            this.Laycan = laycan;
        }

        public DateRange Laycan { get; private set; }
        public SpotCharterId SpotCharterId { get; private set; }
    }
}
