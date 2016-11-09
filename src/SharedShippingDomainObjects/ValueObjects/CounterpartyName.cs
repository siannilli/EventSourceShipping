using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.ValueObjects;

namespace SharedShippingDomainsObjects.ValueObjects
{
    public class CounterpartyName: ValueObject<CounterpartyName>
    {
        public CounterpartyName(string name)
        {
            this.Name = name;            
        }

        public string Name { get; private set; }
    }
}
