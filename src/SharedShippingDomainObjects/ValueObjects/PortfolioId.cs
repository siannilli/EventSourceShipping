using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedShippingDomainsObjects.ValueObjects
{
    public class PortfolioId: UniqueIdentifier
    {
        public PortfolioId(Guid value)
            :base(value)
        {
         
        }
    }
}
