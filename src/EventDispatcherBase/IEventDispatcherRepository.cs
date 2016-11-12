using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseDomainObjects.Events;

namespace EventDispatcherBase
{
    public interface IEventDispatcherRepository
    {
        bool GetNextEventToDispatch(out Guid eventId, out DispatchEvent @event);
        void CommitDispatchedEvent(Guid eventId);        
    }
}
