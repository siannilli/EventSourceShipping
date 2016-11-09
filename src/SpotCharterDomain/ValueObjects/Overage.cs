using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedShippingDomainsObjects.ValueObjects;
using BaseDomainObjects.ValueObjects;

namespace SpotCharterDomain.ValueObjects
{
    public class Overage: ValueObject<Overage>
    {
        public Enums.OverageType Type { get; private set; }
        public decimal OverageValue { get; private set; }

        public Overage(Enums.OverageType type, decimal value)
        {
            this.Type = type;
            this.OverageValue = value;            
        }
    }
}
