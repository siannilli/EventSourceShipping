using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedShippingDomainsObjects.ValueObjects
{
    public class PortId: UniqueIdentifier
    {
        public PortId(Guid value)
            : base(value)
        {
            
        }
    }
}
