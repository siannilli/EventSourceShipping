using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    class SpotCharterDeleted : BaseDomainObjects.Events.Event<SpotCharterId>
    {
        public SpotCharterDeleted( SpotCharterId spotId, int version) : base(spotId, version)
        {

        }

    }
}
