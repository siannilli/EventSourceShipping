using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.ValueObjects;

namespace SharedShippingDomainsObjects.ValueObjects
{
    public class CounterpartyId: UniqueIdentifier
    {
        public CounterpartyId(Guid value)
            : base(value)
        {
            
        }        
        
    }
}
