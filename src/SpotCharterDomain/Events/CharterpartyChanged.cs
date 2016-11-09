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
    public class CharterpartyChanged: Event
    {
        public CharterpartyChanged(Guid eventId, int version, SpotCharterId spotId, CounterpartyId charterpartyId, string name)
            : base(eventId, version)
        {
            this.SpotCharterId = spotId;
            this.CharterpartyId = charterpartyId;
            this.CurrentName = name;
        }

        public CounterpartyId CharterpartyId { get; private set; }
        public string CurrentName { get; private set; }
        public SpotCharterId SpotCharterId { get; private set; }
    }
}
