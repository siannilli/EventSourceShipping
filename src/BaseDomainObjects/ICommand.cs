using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDomainObjects
{
    public interface ICommand
    {
        Guid Id { get; }                

        int Version { get; set; }

        Entities.Login Login { get; set; }

    }
}
