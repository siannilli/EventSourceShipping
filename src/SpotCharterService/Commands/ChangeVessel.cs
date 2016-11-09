using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects;
using BaseDomainObjects.Commands;

using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterServices.Commands
{
    public class ChangeVessel: Command
    {
        public ChangeVessel(SpotCharterId spotId, VesselId vesselId, string vesselName)
            : base(Guid.NewGuid())
        {
            this.SpotCharterId = spotId;
            this.VesselId = vesselId;
            this.Name = vesselName;
        }

        public string Name { get; private set; }
        public SpotCharterId SpotCharterId { get; private set; }
        public VesselId VesselId { get; private set; }
    }
}
