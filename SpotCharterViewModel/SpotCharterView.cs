using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BaseDomainObjects;
using BaseDomainObjects.Entities;
using SharedShippingDomainsObjects.Enums;
using SharedShippingDomainsObjects.ValueObjects; 

namespace SpotCharterViewModel
{
    public class SpotCharterView
    {
        public SpotCharterId Id { get; set; }
        public VesselId VesselId { get; set; }
        public string VesselName { get; set; }
        public CounterpartyId CharterpartyId { get; set; }
        public string CharterpartyName { get; set; }
        public DateTime? CharterpartyDate { get; set; }
        public DateRange Laycan { get; set; }
        public PortfolioId PortfolioId { get; set; }
        public string PortfolioDescription {get; set;}

        public string Freight { get; set; }

        public DemurrageRate DemurrageRate { get; set; }
        public BillOfLading BillOfLading { get; set; }
        public CargoQuantity MinimumQuantity { get; set; }
        
        public int Version { get; set; }
        public DateTime LastUpdate { get; set; }

    }
}
