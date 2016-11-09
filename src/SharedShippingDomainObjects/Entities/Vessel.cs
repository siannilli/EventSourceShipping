using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedShippingDomainsObjects.ValueObjects;
using BaseDomainObjects.Entities;

namespace SharedShippingDomainsObjects.Entities
{
    class Vessel: Entity<VesselId>
    {
        public string IMONumber { get; private set; }
        public string Name { get; private set; }
        public decimal? DeadweightTonnage { get; private set; }
        public decimal? GrossTonnage { get; private set; }
        public uint? Year { get; private set; }
        public string Flag { get; private set; }
        public string Owner { get; private set; }

        public Vessel(VesselId id, string imo, string name)
            : base(id)
        {
            this.IMONumber = imo;
            this.Name = name;
        }

        public Vessel(VesselId id, string imo, string name, string flag, string owner)
            : this(id, imo, name)
        {
            this.Flag = flag;
            this.Owner = owner;
        }

        public Vessel(VesselId id, string imo, string name, string flag, string owner, decimal dwt, decimal gwt)
            : this(id, imo, name, flag, owner)
        {
            this.DeadweightTonnage = dwt;
            this.GrossTonnage = gwt;
        }

        public Vessel(VesselId id, string imo, string name, decimal dwt, decimal gwt)
            : this(id, imo, name)
        {
            this.DeadweightTonnage = dwt;
            this.GrossTonnage = gwt;
        }

        public override string ToString()
        {
            return $"{this.Name}";
        }
    }
}
