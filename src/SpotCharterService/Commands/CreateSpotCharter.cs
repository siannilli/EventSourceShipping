using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects;
using BaseDomainObjects.Commands;
using SpotCharterDomain;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterServices.Commands
{
    public class CreateSpotCharter : Command
    {
        public CreateSpotCharter() : base(Guid.NewGuid()) { }
        public CreateSpotCharter(
            DateTime charterpartyDate, 
            CounterpartyId charterpartyId, 
            string charterpartyName, 
            VesselId vesselId,
            string vesselName,
            string vessel,
            CargoQuantity minimumQuantity)
            : base(Guid.NewGuid())
        {
            this.CharterpartyDate = charterpartyDate;
            this.CharterpartyId = charterpartyId;
            this.CharterpartyName = charterpartyName;
            this.VesselId = vesselId;
            this.VesselName = vesselName;
            this.MinimumQuantity = minimumQuantity;          

        }

        public DateTime CharterpartyDate { get; set; }
        public CounterpartyId CharterpartyId { get; set; }
        public string CharterpartyName { get; set; }
        public CargoQuantity MinimumQuantity { get; set; }
        public VesselId VesselId { get; set; }
        public string VesselName { get; set; }
    }
}
