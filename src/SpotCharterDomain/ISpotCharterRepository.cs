using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotCharterDomain
{
    public interface ISpotCharterRepository: BaseDomainObjects.IEventSourceRepository<SpotCharter, SharedShippingDomainsObjects.ValueObjects.SpotCharterId>
    {

    }
}
