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

        public DateTime CharterpartyDate { get; private set; }
        public CounterpartyId CharterpartyId { get; private set; }
        public string CharterpartyName { get; private set; }
        public CargoQuantity MinimumQuantity { get; private set; }
        public VesselId VesselId { get; private set; }
        public string VesselName { get; private set; }
    }
}
