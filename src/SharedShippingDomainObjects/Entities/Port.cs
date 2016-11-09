using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedShippingDomainsObjects.ValueObjects;
using BaseDomainObjects.Entities;

namespace SharedShippingDomainsObjects.Entities
{
    class Port: Entity<PortId>
    {
        public string UNTACD { get; private set; }
        public string Name { get; private set; }
        public string Country { get; private set; }
        public GeographicPoint Position { get; private set; }

        public Port(PortId id, string code, string name, string country, GeographicPoint point)
            : base(id)
        {
            this.UNTACD = code;
            this.Name = name;
            this.Country = country;
            this.Position = point;
        }

    }
}
