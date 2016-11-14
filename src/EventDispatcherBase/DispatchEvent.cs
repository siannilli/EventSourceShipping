using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventDispatcherBase
{
    public class DispatchEvent
    {

        public Guid Id { get; set; }
        public string EventName { get; set; }
        public DateTime Timestamp { get; set; }
        public int Version { get; set; }
        public string Source { get; set; }
        public string Payload { get; set; }

    }
}
