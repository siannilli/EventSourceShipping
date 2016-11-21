using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain
{
    public interface ISpotCharterCommandRepository: IEventSourceCommandRepository<SpotCharter, SpotCharterId>
    {

    }
}
