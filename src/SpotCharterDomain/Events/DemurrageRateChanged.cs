using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.Entities;
using SharedShippingDomainsObjects.ValueObjects;
using SharedShippingDomainsObjects.Enums;
using BaseDomainObjects.Events;

namespace SpotCharterDomain.Events
{
    public class DemurrageRateChanged : BaseDomainObjects.Events.Event<SpotCharterId>
    {
        public DemurrageRateChanged(SpotCharterId spotId, Login login, int version,  
            DemurrageRate rate) 
            : base(spotId, login, version)
        {
            this.Rate = rate;
        }


        public DemurrageRate Rate { get; private set; }
    }
}
