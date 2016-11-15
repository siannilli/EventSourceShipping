using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Events;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    public class BillOfLadingChanged: Event<SpotCharterId>
    {
        public BillOfLadingChanged(SpotCharterId sourceId, int version, 
            DateTime date,
            CargoQuantity quantity,
            string documentReference)
            :base(sourceId, version)
        {
            this.Date = date;
            this.Quantity = quantity;
        }

        public DateTime Date { get; private set; }
        public string DocumentReference { get; private set; }
        public CargoQuantity Quantity { get; private set; }

    }
}
