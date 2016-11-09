using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.ValueObjects;
namespace SharedShippingDomainsObjects.ValueObjects
{
    public class UniqueIdentifier: ValueObject<Guid>
    {
        public UniqueIdentifier(Guid id)
        {
            this.Value = id;
        }

        public Guid Value { get; private set; }

        public override string ToString()
        {
            return this.Value.ToString() ;
        }
    }
}
