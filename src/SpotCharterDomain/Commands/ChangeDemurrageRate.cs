using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.Commands;
using SharedShippingDomainsObjects.Enums;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Commands
{
    public class ChangeDemurrageRate: BaseSpotCharterCommand
    {

        public DemurrageRate DemurrageRate { get; set; }
    }
}
