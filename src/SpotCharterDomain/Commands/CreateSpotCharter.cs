using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects;
using BaseDomainObjects.Commands;
using SpotCharterDomain;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Commands
{
    public class CreateSpotCharter : BaseSpotCharterCommand
    {
        public DateTime CharterpartyDate { get; set; }
        public CounterpartyId CharterpartyId { get; set; }
        public string CharterpartyName { get; set; }
        public CargoQuantity MinimumQuantity { get; set; }
        public VesselId VesselId { get; set; }
        public string VesselName { get; set; }
    }
}
