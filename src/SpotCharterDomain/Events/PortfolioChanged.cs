using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Events;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    public class PortfolioChanged: Event
    {
        public PortfolioChanged(Guid eventId, int version, SpotCharterId spotId, PortfolioId portfolioId)
            : base(eventId, version)
        {
            this.PorfolioId = portfolioId;
            this.SpotCharterId = spotId;
        }

        public PortfolioId PorfolioId { get; private set; }
        public SpotCharterId SpotCharterId { get; private set; }
    }
}
