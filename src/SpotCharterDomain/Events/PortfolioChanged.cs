using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Events;
using BaseDomainObjects.Entities;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Events
{
    public class PortfolioChanged: Event<SpotCharterId>
    {
        public PortfolioChanged(SpotCharterId spotId, Login login, int version, PortfolioId portfolioId, string portfolioName)
            : base(spotId, login, version)
        {
            this.PorfolioId = portfolioId;
            this.PortfolioName = PortfolioName;
        }

        public PortfolioId PorfolioId { get; private set; }
        public string PortfolioName { get; private set; }
    }
}
