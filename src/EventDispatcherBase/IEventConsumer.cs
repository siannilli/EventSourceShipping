using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventDispatcherBase
{
    public delegate void EventReceived(DispatchEvent @event);

    public interface IEventConsumer: IDisposable
    {
        event EventReceived ReceivedEventHandler;
        void AckReceived(Guid eventId);
        void Quit();

        void StartReceiving(string queueName);

    }
}
