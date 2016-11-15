﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects;
using BaseDomainObjects.Events;
using SharedShippingDomainsObjects.Entities;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    public class SpotCharterCreated : Event<SpotCharterId>
    {
        public SpotCharterCreated(SpotCharterId spotId,
            int version,
                        
            DateTime charterpartyDate,
            CounterpartyId charterpartyId,
            string charterpartyCurrentName,
            VesselId vesselId,
            string vesselCurrentName, 
            CargoQuantity minimumQuantity) 
            : base(spotId, version)
        {
            this.CharterpartyDate = charterpartyDate;
            this.CounterpartyId = charterpartyId;
            this.CounterpartyCurrentName = charterpartyCurrentName;
            this.VesselId = vesselId;
            this.VesselCurrentName = vesselCurrentName;
            this.MinimumQuantity = minimumQuantity;
        }

        public DateTime CharterpartyDate { get; private set; }
        public CounterpartyId CounterpartyId { get; private set; }
        public string CounterpartyCurrentName { get; private set; }
        public VesselId VesselId { get; private set; }
        public CargoQuantity MinimumQuantity { get; private set; }        
        public string VesselCurrentName { get; private set; }
    }
}
