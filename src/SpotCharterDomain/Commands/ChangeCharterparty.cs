using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.Commands;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Commands
{
    public class ChangeCharterparty: BaseSpotCharterCommand
    {
        public CounterpartyId CharterpartyId { get; set; }
        public string CharterpartyName { get; set; }
    }
}
