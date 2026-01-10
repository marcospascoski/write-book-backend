using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.ViewModels.Permissao
{
    public struct PermissaoAlterarStatusViewModel<T>
    {
        public T Id { get; set; }
        public string Status { get; set; }
    }
}
