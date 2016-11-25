using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects;
using BaseDomainObjects.Commands;

using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Commands
{
    public class ChangeVessel: BaseSpotCharterCommand
    {
        public string VesselName { get; set; }
        public VesselId VesselId { get; set; }

    }
}
