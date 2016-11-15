using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Events;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    public class PortfolioChanged: Event<SpotCharterId>
    {
        public PortfolioChanged(SpotCharterId spotId, int version, PortfolioId portfolioId)
            : base(spotId, version)
        {
            this.PorfolioId = portfolioId;
        }

        public PortfolioId PorfolioId { get; private set; }
    }
}
