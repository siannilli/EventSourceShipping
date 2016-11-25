using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseDomainObjects.Entities
{
    public class User: Login
    {
        public User(string name)
            : base(name)
        {

        }

        public User(string name, string email)
            :this(name)
        {
            this.Email = email;
        }

        public string Email { get; private set; }
    }
}
