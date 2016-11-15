using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDomainObjects
{
    public interface IEvent<TAggregateIdentity>
    {
        Guid Id { get; }

        TAggregateIdentity AggregateId { get;}
        string EventName { get; }
        string Source { get; }        
        int Version { get; }

    }
}
