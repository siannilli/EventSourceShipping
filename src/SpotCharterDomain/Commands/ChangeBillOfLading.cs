using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.Commands;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Commands
{
    public class ChangeBillOfLading: BaseSpotCharterCommand
    {
        public BillOfLading BillOfLading { get; set; }

    }
}
