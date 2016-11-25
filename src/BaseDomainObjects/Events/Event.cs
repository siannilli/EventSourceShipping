using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Entities;

namespace BaseDomainObjects.Events
{
    public class Event<TAggregateIdentity> : Entities.Entity<Guid>, IEvent<TAggregateIdentity>
    {

        public Event(TAggregateIdentity aggregateId, Login login)
            : this(aggregateId, login, Guid.NewGuid())
        {

        }

        public Event(TAggregateIdentity aggregateId, Login login, int version)
            : this(aggregateId, login, Guid.NewGuid(), version)
        {

        }

        public Event(TAggregateIdentity aggregateId, Login login, Guid eventId, int version = 1, string source = null)
            : base(eventId)
        {
            this.AggregateId = aggregateId;
            this.Version = version;
            this.Source = source;
            this.Login = login;

            this.Timestamp = DateTime.UtcNow;
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

        public DateTime Timestamp
        {
            get; private set;
        }

        public Login Login
        {
            get; private set;
        }
    }
}
