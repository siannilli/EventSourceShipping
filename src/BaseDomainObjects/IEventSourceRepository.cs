using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseDomainObjects
{
    public interface IEventSourceRepository<T, TIdentity> where T : IEventSourcedAggregate<TIdentity>
    {
        T Get(TIdentity id);
        void Save(T instance);
    }
}