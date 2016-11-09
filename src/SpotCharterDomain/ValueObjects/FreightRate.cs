using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.ValueObjects;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.ValueObjects
{
    public class FreightRate: ValueObject<FreightRate>
    {
        public FreightRate(decimal flat, decimal worldscale, Overage overage)
        {
            this.FreightCalculation = Enums.FreigthCalculation.WorldScale;
            this.Flat = flat;
            this.WorldScale = worldscale;            
        }

        public FreightRate (decimal lumpsum)
        {
            this.FreightCalculation = Enums.FreigthCalculation.Lumpsum;
            this.Lumpsum = lumpsum;
        }

        public FreightRate(decimal price, string uom, Overage overage)
        {
            this.FreightCalculation = Enums.FreigthCalculation.Calculated;
            this.UnitPrice = price;
            this.UnitOfMeasure = uom; 
        }
 
        public Enums.FreigthCalculation FreightCalculation { get; private set; }        
        public Currency Currency { get; private set; }
        public decimal Flat { get; private set; }
        public decimal WorldScale { get; private set; }
        public decimal Lumpsum { get; private set; }
        public decimal UnitPrice { get; private set; }
        public string UnitOfMeasure { get; private set; }

        public Overage Overage { get; private set; }
    }
}
