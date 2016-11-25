using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EventDispatcherBase;

namespace SpotCharterInMemoryEventSourceRepository
{
    public class InProcessEventDispatcher : IEventConsumer, IEventDispatcher
    {
        private readonly List<DispatchEvent> events = new List<DispatchEvent>();

        public event EventReceived ReceivedEventHandler;

        public InProcessEventDispatcher()
        {

        }

        public void AckReceived(Guid eventId)
        {
            var @event = events.FirstOrDefault(e => e.Id.Equals(eventId));
            if (@event != null)
                this.events.Remove(@event);
        }

        public void Dispose()
        {
            this.events.Clear();
        }

        public void Publish(DispatchEvent @event)
        {
            this.events.Add(@event);
            if (this.ReceivedEventHandler != null)
                this.ReceivedEventHandler.Invoke(@event);
        }

        public void Quit()
        {
            throw new NotImplementedException();
        }

        public void StartConsumingEvents(string queueName)
        {
            throw new NotImplementedException();
        }
    }
}
