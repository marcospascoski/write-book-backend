using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.ViewModels.Perfil
{
    public struct PerfilAlterarStatusViewModel<T>
    {
        public T Id { get; set; }
        public string Status { get; set; }
    }
}
