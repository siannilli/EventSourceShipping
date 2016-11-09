using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.ValueObjects;

namespace SharedShippingDomainsObjects.ValueObjects
{
    public class BillOfLading: ValueObject<BillOfLading>
    {
        public BillOfLading(DateTime date, CargoQuantity quantity, string documentReference)
        {
            this.Date = date;
            this.Quantity = quantity;
            this.DocReference = documentReference;            

        }

        public DateTime Date { get; private set; }
        public CargoQuantity Quantity { get; private set; }
        public string DocReference { get; private set; }
    }
}
