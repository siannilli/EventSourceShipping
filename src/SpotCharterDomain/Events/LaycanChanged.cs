using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Events;
using BaseDomainObjects.Entities;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    public class LaycanChanged: Event<SpotCharterId>
    {
        public LaycanChanged(SpotCharterId spotId, Login login, int version, 
            DateRange laycan)
            : base(spotId, login, version)
        {
            this.Laycan = laycan;
        }

        public DateRange Laycan { get; private set; }
    }
}
