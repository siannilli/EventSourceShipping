using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDomainObjects
{
    public interface IRepository<TAggregate, TIdentity>
    {
        TAggregate Get(TIdentity id);
        void Add(TAggregate aggregate);
        void Save(TAggregate aggregate);

        IEnumerable<TAggregate> Find(IEnumerable<QueryClause> query);
    }
}
