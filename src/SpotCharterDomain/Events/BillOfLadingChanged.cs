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
    public class BillOfLadingChanged: Event<SpotCharterId>
    {

        public BillOfLadingChanged(SpotCharterId spotId, Login login, int version, 
           BillOfLading bl)
            :base(spotId, login, version)
        {

            this.BillOfLading = bl;
        }


        public BillOfLading BillOfLading{ get; private set; }

    }
}
