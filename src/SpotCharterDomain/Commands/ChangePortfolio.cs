using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.Commands;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterDomain.Commands
{
    public class ChangePortfolio: BaseSpotCharterCommand
    {
        public PortfolioId PortfolioId { get; set; }
        public string PortfolioName { get; set; }
    }
}
