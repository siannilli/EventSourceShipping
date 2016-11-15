using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Events;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    public class LaycanChanged: Event<SpotCharterId>
    {
        public LaycanChanged(SpotCharterId spotId, int version, 
            DateRange laycan)
            : base(spotId, version)
        {
            this.Laycan = laycan;
        }

        public DateRange Laycan { get; private set; }
    }
}
