using Onix.Framework.Infra.Data.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Application.ViewModels.Registrar;
using System;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.Interfaces
{
    public interface IUsuarioAppService
    {
        Task<Guid> CadastrarAsync(RegistrarUsuarioViewModel model);
        Task Alterar(UsuarioViewModel usuarioViewModel);
        Task<UsuarioViewModel> ObterPorEmailAsync(string email);
        Task<UsuarioViewModel> PesquisarPorId(Guid id);
        Task<bool> CreditarRecompensa(Guid usuarioId, decimal valor);
        Task<bool> AlterarPerfilAsync(Guid usuarioId, UsuarioViewModel updates);
        Task AlterarStatus(UsuarioAlterarStatusViewModel alterarStatusViewModel);
        Task<IPagedItems<UsuarioViewModel>> Paginar(FiltroUsuarioViewModel model);
    }
}
