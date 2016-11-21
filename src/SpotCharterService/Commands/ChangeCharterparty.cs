using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.Commands;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterServices.Commands
{
    public class ChangeCharterparty: Command
    {
        public ChangeCharterparty()
            : base(Guid.NewGuid()) { }
        public ChangeCharterparty(SpotCharterId spotId, CounterpartyId newChartepartyId, string newCharterpartyName)
            : base(Guid.NewGuid())
        {
            this.SpotCharterId = spotId;
            this.CharterpartyId = newChartepartyId;
            this.Name = newCharterpartyName;
        }

        public CounterpartyId CharterpartyId { get; set; }
        public string Name { get; set; }
        public SpotCharterId SpotCharterId { get; set; }
    }
}
