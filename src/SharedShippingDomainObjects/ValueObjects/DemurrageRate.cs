using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedShippingDomainsObjects.Enums;
using BaseDomainObjects.ValueObjects;

namespace SharedShippingDomainsObjects.ValueObjects
{
    public class DemurrageRate: ValueObject<DemurrageRate>
    {

        public double LoadHoursLaytime { get; private set; }
        public double DischargeHoursLaytime { get; private set; }
        public double TotalHoursLaytime { get; private set; }

        public CostAmount Price { get; private set; }

        public DemurrageRateTimeUnit TimeUnit { get; private set; }

        public DemurrageRate(double laytimeLoad, double laytimeDischarge, double laytimeTotal, CostAmount price, DemurrageRateTimeUnit timeUnit)
        {
            this.LoadHoursLaytime = laytimeLoad;
            this.DischargeHoursLaytime = laytimeDischarge;
            this.TotalHoursLaytime = laytimeTotal;
            this.Price = price;
            this.TimeUnit = timeUnit;
        }
    }
}
