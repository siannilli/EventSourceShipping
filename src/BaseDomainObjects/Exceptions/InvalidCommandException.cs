using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseDomainObjects.Exceptions
{
    public class InvalidCommandException: Exception
    {
        public InvalidCommandException()
            : base("The command contains one or more invalid data")
        {

        }
        public InvalidCommandException(string message)
            : base(message)            
        {

        }
    }
}
