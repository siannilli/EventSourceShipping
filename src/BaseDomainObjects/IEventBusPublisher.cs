using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDomainObjects
{
    public interface IEventBusPublisher
    {
        void Publish(IEvent @event);
    }
}
