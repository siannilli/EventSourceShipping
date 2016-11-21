using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.Commands;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterServices.Commands
{
    public class ChangeLaycan: Command
    {

        public ChangeLaycan(): base(Guid.NewGuid()) { }
        public ChangeLaycan(SpotCharterId spotId, DateTime from, DateTime to)
            : base(Guid.NewGuid())
        {
            this.SpotCharterId = spotId;
            this.From = from;
            this.To = to;
        }

        public DateTime From { get; set; }
        public SpotCharterId SpotCharterId { get; set; }
        public DateTime To { get; set; }
    }
}
