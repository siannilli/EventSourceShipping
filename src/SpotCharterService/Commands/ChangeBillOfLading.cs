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

        public ChangeBillOfLading() : base(Guid.NewGuid()) { }
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

        public DateTime BLDate { get; set; }
        public CargoQuantity BLQuantity { get; set; }
        public string DocumentReference { get; set; }
        public SpotCharterId SpotCharterId { get; set; }
    }
}
