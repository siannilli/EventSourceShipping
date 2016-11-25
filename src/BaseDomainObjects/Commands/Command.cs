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
            this.Version = 1;
        }
        
        public Command(Guid id)
        {
            this.Id = id;
        }     

        public Guid Id{get; private set;}

        public int Version
        {
            get;
            set;
        }

        Guid ICommand.Id
        {
            get
            {
                return this.Id;
            }
        }

        public Entities.Login Login { get; set; }

    }
}
