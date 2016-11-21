using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterViewModel
{
    public interface ISpotCharterUpdateViewRepository
    {
        SpotCharterView GetById(SpotCharterId id);
        void Save(SpotCharterView spot);
        void Remove(SpotCharterView spot);
        void Remove(SpotCharterId id);
    }
}
