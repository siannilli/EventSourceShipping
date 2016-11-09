using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects.Commands;
using SharedShippingDomainsObjects.ValueObjects;

namespace SpotCharterServices.Commands
{
    public class ChangeBillOfLading: Command
    {
        public ChangeBillOfLading(SpotCharterId spotId,
            DateTime blDate,
            CargoQuantity blQuantity,
            string docReference)
            : base(Guid.NewGuid())
        {
            this.SpotCharterId = spotId;
            this.BLDate = blDate;
            this.BLQuantity = blQuantity;
            this.DocumentReference = docReference;
        }

        public DateTime BLDate { get; private set; }
        public CargoQuantity BLQuantity { get; private set; }
        public string DocumentReference { get; private set; }
        public SpotCharterId SpotCharterId { get; private set; }
    }
}
