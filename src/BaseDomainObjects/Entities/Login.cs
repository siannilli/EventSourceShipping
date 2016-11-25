using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseDomainObjects.Entities
{
    public class Login: Entity<string>
    {
        public Login(string name)
            : base(name)
        {
            
        }
    }
}
