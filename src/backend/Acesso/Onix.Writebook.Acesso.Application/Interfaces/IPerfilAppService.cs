using Onix.Framework.Infra.Data.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels.Perfil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.Interfaces
{
    public interface IPerfilAppService
    {
        Task<long> Cadastrar(PerfilViewModel perfilViewModel);
        Task Alterar(PerfilViewModel perfilViewModel);
        Task<PerfilViewModel> PesquisarPorId(long id);
        Task AlterarStatus(PerfilAlterarStatusViewModel<long> alterarStatusViewModel);
        Task<IPagedItems<PerfilViewModel>> Paginar(FiltroPerfilViewModel model);

    }
}
