using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDomainObjects.Events
{
    public class Event<TAggregateIdentity> : Entities.Entity<Guid>, IEvent<TAggregateIdentity>
    {

        public Event(TAggregateIdentity aggregateId)
            : this(aggregateId, Guid.NewGuid())
        {

        }

        public Event(TAggregateIdentity aggregateId, int version)
            : this(aggregateId, Guid.NewGuid(), version)
        {

        }

        public Event(TAggregateIdentity aggregateId, Guid eventId)
            : this(aggregateId, eventId, 1)
        {

        }

        public Event(TAggregateIdentity aggregateId, Guid eventId, int version)
            : this(aggregateId, eventId, version, null)
        {

        }

        public Event(TAggregateIdentity aggregateId, Guid eventId, int version, string source)
            : base(eventId)
        {
            this.AggregateId = aggregateId;
            this.Version = version;
            this.Source = source;
        }
        
        public int Version { get; private set; }

        public string EventName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public string Source
        {
            get;
            private set;
        }

        public TAggregateIdentity AggregateId
        {
            get; private set;
        }

    }
}
