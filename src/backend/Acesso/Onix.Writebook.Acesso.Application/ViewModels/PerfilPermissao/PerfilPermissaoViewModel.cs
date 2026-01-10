using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.ViewModels.PerfilPermissao
{
    public class PerfilPermissaoViewModel
    {
        public long Id { get; set; }
        public long PerfilId { get; set; }
        public long PermissaoId { get; set; }
        public string Status { get; set; }
    }
}
