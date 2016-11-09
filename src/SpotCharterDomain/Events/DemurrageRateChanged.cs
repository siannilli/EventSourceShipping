using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedShippingDomainsObjects.ValueObjects;
using SharedShippingDomainsObjects.Enums;
namespace SpotCharterDomain.Events
{
    public class DemurrageRateChanged : BaseDomainObjects.Events.Event
    {
        public DemurrageRateChanged(Guid eventId, int version,  SpotCharterId spotId,
            DemurrageRate rate) 
            : base(eventId, version)
        {
            this.SpotCharterId = spotId;
            this.Rate = rate;
        }


        public DemurrageRate Rate { get; private set; }
        public SpotCharterId SpotCharterId { get; private set; }
    }
}
