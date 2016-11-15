using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain
{
    public interface ISpotCharterUpdateViewRepository
    {
        SpotCharter GetById(SpotCharterId id);
        void Save(SpotCharter spot);
        void Remove(SpotCharter spot);
        void Remove(SpotCharterId id);
    }
}
