using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.ValueObjects;

namespace SharedShippingDomainsObjects.ValueObjects
{
    public class CargoQuantity: ValueObject<CargoQuantity>
    {
        public string UnitOfMeasure { get; private set; }
        public decimal Quantity { get; private set; }

        public CargoQuantity(string uom, decimal quantity)
        {
            this.UnitOfMeasure = uom;
            this.Quantity = quantity;            
        }
    }
}
