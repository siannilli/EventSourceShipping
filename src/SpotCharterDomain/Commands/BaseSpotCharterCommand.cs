using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BaseDomainObjects.Commands;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Commands
{
    public abstract class BaseSpotCharterCommand: Command
    {
        public SpotCharterId SpotCharterId { get; set; }
    }
}
