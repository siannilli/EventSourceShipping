using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EventDispatcherBase;
using SpotCharterDomain;

namespace SpotCharterViewUpdater
{
    public class WorkerProcess
    {
        private readonly IEventConsumer consumer;
        private readonly ISpotCharterQueryRepository repository;

        public WorkerProcess(IEventConsumer consumer)
        {
            // this.repository = repository;
            this.consumer = consumer;
            this.consumer.EventReceivedHandler += (e) =>
            {                
                System.Console.WriteLine(e.Payload);
                consumer.AckReceived(e.Id);
            };
        }

        public void Quit()
        {
            this.consumer.Quit();
        }
    }
}
