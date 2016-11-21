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

using Newtonsoft.Json;

namespace SpotCharterInMemoryEventSourceRepository
{    

    public class SpotCharterInMemoryEventSourceRepository: ISpotCharterCommandRepository    
    {
        class EventInstance
        {
            public Type Type { get; set; }
            public string JsonString { get; set; }
        }

        private readonly IDictionary<SpotCharterId, IEnumerable<EventInstance>> repository = new Dictionary<SpotCharterId, IEnumerable<EventInstance>>();

        SpotCharter IEventSourceCommandRepository<SpotCharter, SpotCharterId>.Get(SpotCharterId id)
        {

            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                ContractResolver = new JsonNet.PrivateSettersContractResolvers.PrivateSetterContractResolver()
            };

            if (!this.repository.ContainsKey(id))
                return null;

            var eventStream = this.repository[id];
            return new SpotCharter(eventStream.Select(json => JsonConvert.DeserializeObject(json.JsonString, json.Type, settings) as IEvent<SpotCharterId>).ToArray());
        }

        void IEventSourceCommandRepository<SpotCharter, SpotCharterId>.Save(SpotCharter instance)
        {
            SpotCharterId id = instance.Id;
            IList<EventInstance> eventStream = this.repository.ContainsKey(id) ? this.repository[id].ToList() : new List<EventInstance>();

            IEventSourcedAggregate<SpotCharterId> spotEventsInterface = instance;

            foreach (var @event in spotEventsInterface.Events)
            {
                eventStream.Add(new EventInstance { JsonString = JsonConvert.SerializeObject(@event), Type = @event.GetType() });
            }

            this.repository[id] = eventStream;  
        }
    }
}
