using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDomainObjects.Events
{
    public class Event : Entities.Entity<Guid>, IEvent
    {
        public Event(Guid eventId, int version)
            : base(eventId)
        {
            this.Id = eventId;
            this.Version = version;
        }

        public Event(Guid eventId, int version, string source)
            : this(eventId, version)
        {
            this.Source = source;
        }
        
        public int Version { get; private set; }

        string IEvent.EventName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        Guid IEvent.Id
        {
            get
            {
                return this.Id;
            }

        }

        int IEvent.Version
        {
            get
            {
                return this.Version;
            }
        }

        string IEvent.Source
        {
            get
            {
                return this.Source;
            }
        }

        public string Source { get; private set; }
    }
}
