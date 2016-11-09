using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.Entities;
using BaseDomainObjects.Exceptions;

namespace BaseDomainObjects.Aggregates
{
    public class EventSourcedAggregate<TIdentity> : Entity<TIdentity>, IEventSourcedAggregate<TIdentity>
    {
        private readonly Dictionary<Type, Action<IEvent>> handlers = new Dictionary<Type, Action<IEvent>>();
        int version = 0;
        
        IList<IEvent> pendingEvents = new List<IEvent>();    


        public EventSourcedAggregate(TIdentity id)
            : base(id)
        {

        }
        public EventSourcedAggregate(TIdentity id, IEnumerable<IEvent> events)
            : base(id)
        {
            this.ReplayEvents(events);
        }
               
        public void Handles<TEvent>(Action<TEvent> eventHandler) where TEvent : IEvent
        {
            this.handlers[typeof(TEvent)] = @event => eventHandler((TEvent)@event);
        } 

        TIdentity IEventSourcedAggregate<TIdentity>.Id
        {
            get
            {
                return this.Id;
            }
        }

        protected void ReplayEvents(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                var handler = this.handlers[@event.GetType()];
                handler.Invoke(@event);
            }

            this.version = events.Max(e => e.Version);
        }

        private void PlayEvent(IEvent @event)
        {
            var handler = this.handlers[@event.GetType()];
            handler.Invoke(@event);
            // this.version = @event.Version; // Version update only when persisted, to avoid increments of version for each update
            this.pendingEvents.Add(@event);
        }

        protected void UpdateAggregate(IEvent @event)
        {
            if (@event.Version <= this.version)
                throw new InvalidAggregateVersionException();

            this.PlayEvent(@event);                                     
        } 

        public int Version { get { return this.version; } }

        IEnumerable<IEvent> IEventSourcedAggregate<TIdentity>.Events
        {
            get
            {
                return this.pendingEvents;
            }
        }

        int IEventSourcedAggregate<TIdentity>.Version
        {
            get
            {
                return this.version + (int)this.pendingEvents.Count();
            }
        }

        public override bool Equals(object obj)
        {
            return (obj != null 
                && obj is EventSourcedAggregate<TIdentity>
                && this.Id.Equals(((IEventSourcedAggregate<TIdentity>)obj).Id))
                && this.version.Equals(((IEventSourcedAggregate<TIdentity>)obj).Version)
                || base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

    }
}
