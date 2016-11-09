using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDomainObjects.Entities
{
    public abstract class Entity<T>
    {
        public T Id { get; protected set; }

        public Entity(T id)
        {
            this.Id = id;
        }

        public override bool Equals(object obj)
        {
            return (obj != null
                && obj is Entity<T>
                && this.Id.Equals(((Entity<T>)obj).Id)) || base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
