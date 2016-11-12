using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventDispatcherBase
{
    public interface IEventDispatcherRepository
    {
        bool GetNextEventToDispatch(out Guid eventId, out string payload);
        void CommitDispatchedEvent(Guid eventId);        
    }
}
