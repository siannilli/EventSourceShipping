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
    public class VesselChanged : Event
    {
        public VesselChanged(Guid eventId, int version, SpotCharterId spotId, VesselId vesselId, string name)
            : base(eventId, version)
        {
            this.SpotCharterId = spotId;
            this.VesselId = vesselId;
            this.CurrentName = name;
        }

        public string CurrentName { get; private set; }
        public SpotCharterId SpotCharterId { get; private set; }
        public VesselId VesselId { get; private set; }
    }
}
