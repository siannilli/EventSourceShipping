using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.Commands;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterServices.Commands
{
    public class ChangePortfolio: Command
    {
        public ChangePortfolio(): base(Guid.NewGuid()) { }
        public ChangePortfolio(SpotCharterId spotId, PortfolioId portfolioId)
            : base(Guid.NewGuid())
        {
            this.SpotCharterId = spotId;
            this.PortfolioId = portfolioId;
        }

        public PortfolioId PortfolioId { get; set; }
        public SpotCharterId SpotCharterId { get; set; }
    }
}
