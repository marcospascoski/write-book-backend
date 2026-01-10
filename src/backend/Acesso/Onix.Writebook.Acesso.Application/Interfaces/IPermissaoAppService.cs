using Onix.Framework.Infra.Data.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels.Permissao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.Interfaces
{
    public interface IPermissaoAppService
    {
        Task<long> Cadastrar(PermissaoViewModel permissaoViewModel);
        Task Alterar(PermissaoViewModel permissaoViewModel);
        Task<PermissaoViewModel> PesquisarPorId(long id);
        Task AlterarStatus(PermissaoAlterarStatusViewModel<long> alterarStatusViewModel);
        Task<IPagedItems<PermissaoViewModel>> Paginar(FiltroPermissaoViewModel model);

    }
}
