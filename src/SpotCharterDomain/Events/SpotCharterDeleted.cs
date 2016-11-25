using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.Entities;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    class SpotCharterDeleted : BaseDomainObjects.Events.Event<SpotCharterId>
    {
        public SpotCharterDeleted( SpotCharterId spotId, Login login, int version) : base(spotId, login, version)
        {

        }

    }
}
