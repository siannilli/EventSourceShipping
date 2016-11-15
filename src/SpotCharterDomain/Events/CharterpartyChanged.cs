using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Events;
using SharedShippingDomainsObjects.Entities;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    public class CharterpartyChanged: Event<SpotCharterId>
    {
        public CharterpartyChanged(SpotCharterId spotId, int version, 
            CounterpartyId charterpartyId, string name)
            : base(spotId, version)
        {
            this.CharterpartyId = charterpartyId;
            this.CurrentName = name;
        }

        public CounterpartyId CharterpartyId { get; private set; }
        public string CurrentName { get; private set; }
    }
}
