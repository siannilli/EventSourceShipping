using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Events;
using BaseDomainObjects.Entities;
using SharedShippingDomainsObjects.Entities;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    public class VesselChanged : Event<SpotCharterId>
    {
        public VesselChanged(SpotCharterId spotId, Login login, int version, VesselId vesselId, string name)
            : base(spotId, login, version)
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
