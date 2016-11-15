using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDomainObjects
{
    public interface IEventSourcedAggregate<TIdentity>
    {
        TIdentity Id { get; }
        IEnumerable<IEvent<TIdentity>> Events { get; }
        int Version { get; }
    }
}
