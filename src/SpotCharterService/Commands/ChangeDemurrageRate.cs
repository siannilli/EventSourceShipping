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

        public ChangeDemurrageRate(): base(Guid.NewGuid()) { }

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

        public double LaytimeDischarge { get; set; }
        public double LaytimeLoad { get; set; }
        public double LaytimeTotal { get; set; }
        public CostAmount Rate { get; set; }
        public SpotCharterId SpotCharterId { get; set; }
        public DemurrageRateTimeUnit TimeUnit { get; set; }
    }
}
