using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventDispatcherBase
{
    public interface IEventBroadcaster
    {
        void Publish(object payload);
    }
}
