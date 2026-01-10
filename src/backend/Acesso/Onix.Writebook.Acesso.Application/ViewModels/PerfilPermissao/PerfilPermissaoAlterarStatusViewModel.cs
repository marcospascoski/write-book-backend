using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.ViewModels.PerfilPermissao
{
    public struct PerfilPermissaoAlterarStatusViewModel<T>
    {
        public long Id { get; set; }
        public string Status { get; set; }
    }
}
