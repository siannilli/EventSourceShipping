using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Entities;
using SharedShippingDomainsObjects.ValueObjects;

namespace SharedShippingDomainsObjects.Entities
{
    class Counterparty: Entity<CounterpartyId>
    {
        public string Name { get; private set; }

        public Counterparty(CounterpartyId id, string name)
            : base(id)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return $"{this.Name}";
        }
    }
}
