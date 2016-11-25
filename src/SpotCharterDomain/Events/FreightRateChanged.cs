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
    public class FreightRateChanged: Event<SpotCharterId>
    {
        public FreightRateChanged(SpotCharterId spotId, Login login, int version, 
            ValueObjects.FreightRate freightRate)
            : base(spotId, login, version)
        {
            this.FreightRate  = freightRate;
        }

        public ValueObjects.FreightRate FreightRate { get; private set; }

    }
}
