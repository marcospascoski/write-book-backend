using AutoMapper;
using Microsoft.Extensions.Localization;
using Onix.Framework.Infra.Data.Implementation;
using Onix.Framework.Infra.Data.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels.Perfil;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.Services
{
    public class PerfilAppService(
        INotificationContext notificationContext,
        IAcessosUnitOfWork acessoUnitOfWork,
        IPerfilRepository perfilRepository,
        IPerfilValidator perfilValidator,
        IMapper mapper,
        IStringLocalizer<Core.Resources.TextResource> stringLocalizer
    ) : IPerfilAppService
    {
       

        public async Task<long> Cadastrar(PerfilViewModel perfilViewModel)
        {
            var prototype = mapper.Map<Perfil>(perfilViewModel);
            var perfil = Perfil.Factory.Create(prototype);
            if (await perfilValidator.IsValid(perfil))
            {
                await perfilRepository.Cadastrar(perfil);
                await acessoUnitOfWork.CommitAsync();
                notificationContext.AddSuccess(stringLocalizer.GetString("SucessoCriarPerfil"));
                return perfil.Id;
            }
            return default;
        }

        public async Task Alterar(PerfilViewModel perfilViewModel)
        {
            var perfil = await perfilRepository.PesquisarPorIdAsync(perfilViewModel.Id);
            if (perfil == null)
            {
                var perfilString = stringLocalizer.GetString("Perfil");
                notificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", perfilString));
                return;
            }
            var perfilAlterarDados = mapper.Map<Perfil>(perfilViewModel);
            perfil.AlterarDados(perfilAlterarDados);
            if (await perfilValidator.IsValid(perfil))
            {
                perfilRepository.Alterar(perfil);
                await acessoUnitOfWork.CommitAsync();
                notificationContext.AddSuccess(stringLocalizer.GetString("SucessoAlterarDadosPerfil"));
            }
        }

        public async Task AlterarStatus(PerfilAlterarStatusViewModel<long> alterarStatusViewModel)
        {
            var perfil = await perfilRepository.PesquisarPorIdAsync(alterarStatusViewModel.Id);
            if (perfil == null)
            {
                var perfilString = stringLocalizer.GetString("Perfil");
                notificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", perfilString));
                return;
            }
            var status = mapper.Map<EStatusEntidade>(alterarStatusViewModel.Status);
            perfil.AlterarStatus(status);
            if (await perfilValidator.IsValid(perfil))
            {
                perfilRepository.Alterar(perfil);
                await acessoUnitOfWork.CommitAsync();
                notificationContext.AddSuccess(stringLocalizer.GetString("SucessoAlterarDadosPerfil"));
            }
        }
        public async Task<IPagedItems<PerfilViewModel>> Paginar(FiltroPerfilViewModel model)
        {
            var pagedItems = await perfilRepository.Paginar(model, model.Texto);
            return mapper.Map<PagedItems<PerfilViewModel>>(pagedItems);
        }

        public async Task<PerfilViewModel> PesquisarPorId(long id)
        {
            var perfil = await perfilRepository.PesquisarPorIdAsync(id);
            var perfilViewModel = mapper.Map<PerfilViewModel>(perfil);
            return perfilViewModel;
        }
    }
}
