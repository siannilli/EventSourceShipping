using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.ValueObjects;

namespace SharedShippingDomainsObjects.ValueObjects
{
    public class VesselName: ValueObject<VesselName>
    {
        public VesselName(string name, string imo)
        {
            this.Name = name;
            this.Imo = imo;            
        }

        public string Imo { get; private set; }
        public string Name { get; private set; }
    }
}
