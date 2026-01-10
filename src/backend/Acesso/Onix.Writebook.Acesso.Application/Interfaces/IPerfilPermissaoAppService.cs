using Onix.Writebook.Acesso.Application.ViewModels.PerfilPermissao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.Interfaces
{
    public interface IPerfilPermissaoAppService
    {
        Task<long> Cadastrar(PerfilPermissaoViewModel perfilPermissaoViewModel);
        Task Remover(PerfilPermissaoViewModel perfilPermissaoViewModel);
        Task<PerfilPermissaoViewModel> PesquisarPorId(long id);
    }
}
