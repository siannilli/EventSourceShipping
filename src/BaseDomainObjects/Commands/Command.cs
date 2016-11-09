using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects;

namespace BaseDomainObjects.Commands
{
    public abstract class Command : ICommand 
    {   
        
        public Command()
        {
            this.Id = Guid.NewGuid();
        }
        
        public Command(Guid id)
        {
            this.Id = id;
        }     

        public Guid Id{get; private set;}
                
        Guid ICommand.Id
        {
            get
            {
                return this.Id;
            }
        }

    }
}
