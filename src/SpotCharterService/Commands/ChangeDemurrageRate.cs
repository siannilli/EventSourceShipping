using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.Commands;
using SharedShippingDomainsObjects.Enums;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterServices.Commands
{
    public class ChangeDemurrageRate: Command
    {

        public ChangeDemurrageRate(SpotCharterId spotId, 
            double laytimeLoad,
            double laytimeDischarge,
            double laytimeTotal,
            CostAmount price,
            DemurrageRateTimeUnit timeUnit)
            : base(Guid.NewGuid())
        {
            this.SpotCharterId = spotId;
            this.LaytimeLoad = laytimeLoad;
            this.LaytimeDischarge = laytimeDischarge;
            this.LaytimeTotal = laytimeTotal;
            this.Rate = price;
            this.TimeUnit = timeUnit;
        }

        public double LaytimeDischarge { get; private set; }
        public double LaytimeLoad { get; private set; }
        public double LaytimeTotal { get; internal set; }
        public CostAmount Rate { get; private set; }
        public SpotCharterId SpotCharterId { get; private set; }
        public DemurrageRateTimeUnit TimeUnit { get; private set; }
    }
}
