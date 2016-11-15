using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedShippingDomainsObjects.ValueObjects;
using SharedShippingDomainsObjects.Enums;
using BaseDomainObjects.Events;

namespace SpotCharterDomain.Events
{
    public class DemurrageRateChanged : BaseDomainObjects.Events.Event<SpotCharterId>
    {
        public DemurrageRateChanged(SpotCharterId spotId, int version,  
            DemurrageRate rate) 
            : base(spotId, version)
        {
            this.Rate = rate;
        }


        public DemurrageRate Rate { get; private set; }
    }
}
