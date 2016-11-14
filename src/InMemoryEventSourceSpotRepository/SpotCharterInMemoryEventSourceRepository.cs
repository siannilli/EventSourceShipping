using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects;
using BaseDomainObjects.Events;
using BaseDomainObjects.Aggregates;
using SharedShippingDomainsObjects.ValueObjects;
using SpotCharterDomain;

namespace SpotCharterInMemoryEventSourceRepository
{
    public class SpotCharterInMemoryEventSourceRepository: ISpotCharterCommandRepository    
    {
        public readonly IDictionary<SpotCharterId, IEnumerable<IEvent>> repository = new Dictionary<SpotCharterId, IEnumerable<IEvent>>();

        SpotCharter IEventSourceCommandRepository<SpotCharter, SpotCharterId>.Get(SpotCharterId id)
        {
            if (!this.repository.ContainsKey(id))
                return null;

            var eventStream = this.repository[id];
            return new SpotCharter(eventStream.ToArray());
        }

        void IEventSourceCommandRepository<SpotCharter, SpotCharterId>.Save(SpotCharter instance)
        {
            SpotCharterId id = instance.Id;
            IList<IEvent> eventStream = this.repository.ContainsKey(id) ? this.repository[id].ToList() : new List<IEvent>();

            IEventSourcedAggregate<SpotCharterId> spotEventsInterface = instance;

            foreach (var @event in spotEventsInterface.Events)
            {
                eventStream.Add(@event);
            }

            this.repository[id] = eventStream;  
        }
    }
}
