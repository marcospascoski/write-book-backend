using Onix.Writebook.Acesso.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Interfaces
{
    public interface IPerfilValidator
    {
        Task<bool> IsValid(Perfil perfil);
    }
}
